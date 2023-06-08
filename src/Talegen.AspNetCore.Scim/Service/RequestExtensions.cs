/*
 *
 * Copyright (c) Talegen, LLC.  All rights reserved.
 * Copyright (c) Microsoft Corporation.  All rights reserved.
 *
 * Licensed under the MIT License;
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at https://mit-license.org/
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
*/

namespace Talegen.AspNetCore.Scim.Service
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using Protocol;
    using Schema;

    public static class RequestExtensions
    {
        /// <summary>
        /// Contains a segment separator character.
        /// </summary>
        private const string SegmentSeparator = "/";

        /// <summary>
        /// Contains a segment interface prefix.
        /// </summary>
        private const string SegmentInterface = SegmentSeparator + SchemaConstants.PathInterface + SegmentSeparator;

        /// <summary>
        /// Contains segment separators.
        /// </summary>
        private static readonly Lazy<char[]> SegmentSeparators = new(() => SegmentSeparator.ToArray());

        /// <summary>
        /// This method is used to get a base resource identifier from within a HTTP Request message.
        /// </summary>
        /// <param name="request">Contains the request.</param>
        /// <returns>Returns a Uri of the resource identifier.</returns>
        /// <exception cref="ArgumentException">Exception is thrown if there is no request Uri defined.</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "False analysis of the 'this' parameter of an extension method")]
        public static Uri GetBaseResourceIdentifier(this HttpRequestMessage request)
        {
            Uri result;

            if (request.RequestUri == null)
            {
                throw new ArgumentException(Schema.Properties.Resources.ExceptionInvalidRequest);
            }

            string lastSegment = request.RequestUri.AbsolutePath.Split(SegmentSeparators.Value, StringSplitOptions.RemoveEmptyEntries).Last();

            if (string.Equals(lastSegment, SchemaConstants.PathInterface, StringComparison.OrdinalIgnoreCase))
            {
                result = request.RequestUri;
            }
            else
            {
                string resourceIdentifier = request.RequestUri.AbsoluteUri;

                int indexInterface = resourceIdentifier.LastIndexOf(SegmentInterface, StringComparison.OrdinalIgnoreCase);

                if (indexInterface < 0)
                {
                    throw new ArgumentException(Schema.Properties.Resources.ExceptionInvalidRequest);
                }

                string baseResource = resourceIdentifier.Substring(0, indexInterface);
                result = new Uri(baseResource, UriKind.Absolute);
            }

            return result;
        }

        /// <summary>
        /// This method is used to try getting a request identifier from within the HTTP request message.
        /// </summary>
        /// <param name="request">Contains the request.</param>
        /// <param name="requestIdentifier">Contains the output request identifier.</param>
        /// <returns>Contains a value indicating whether the request identifier is found.</returns>
        public static bool TryGetRequestIdentifier(this HttpRequestMessage request, out string requestIdentifier)
        {
            request?.Headers.TryGetValues("client-id", out IEnumerable<string> _);
            requestIdentifier = Guid.NewGuid().ToString();
            return true;
        }

        /// <summary>
        /// This method is used to add a relationship operation.
        /// </summary>
        /// <param name="context">Contains the bulk update operation context.</param>
        /// <param name="creations">Contains the bulk operation context creations.</param>
        /// <exception cref="ArgumentNullException">Exception is thrown if arguments are not specified.</exception>
        /// <exception cref="ArgumentException">Exception is thrown if method or operation are not specified.</exception>
        /// <exception cref="HttpResponseException">Exception is thrown if the request is bad.</exception>
        private static void Relate(this IBulkUpdateOperationContext context, IEnumerable<IBulkCreationOperationContext> creations)
        {
            if (creations == null)
            {
                throw new ArgumentNullException(nameof(creations));
            }

            if (context.Method == null)
            {
                throw new ArgumentException(Schema.Properties.Resources.ExceptionInvalidContext);
            }

            if (context.Operation == null)
            {
                throw new ArgumentException(Schema.Properties.Resources.ExceptionInvalidContext);
            }

            try
            {
                dynamic operationDataJson = JsonConvert.DeserializeObject(context.Operation.Data.ToString());
                IReadOnlyCollection<PatchOperation2Combined> patchOperations = operationDataJson.Operations.ToObject<List<PatchOperation2Combined>>();
                PatchRequest2 patchRequest = new PatchRequest2(patchOperations);

                foreach (IBulkCreationOperationContext creation in creations)
                {
                    if (creation.Operation == null)
                    {
                        throw new InvalidOperationException(Schema.Properties.Resources.ExceptionInvalidOperation);
                    }

                    if (string.IsNullOrWhiteSpace(creation.Operation.Identifier))
                    {
                        throw new InvalidOperationException(Schema.Properties.Resources.ExceptionInvalidOperation);
                    }

                    if (patchRequest.References(creation.Operation.Identifier))
                    {
                        creation.AddDependent(context);
                        context.AddDependency(creation);
                    }
                }
            }
            catch
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }
        }

        /// <summary>
        /// This method is used to enlist operation.
        /// </summary>
        /// <param name="request">Contains the request.</param>
        /// <param name="operation">Contains the operation.</param>
        /// <param name="operations">Contains operation contexts.</param>
        /// <param name="creations">Contains operation creations.</param>
        /// <param name="updates">Contains operation updates.</param>
        /// <exception cref="ArgumentNullException">Exception is thrown if a parameter is not specified.</exception>
        /// <exception cref="ArgumentException">Exception is thrown when method is not specified.</exception>
        /// <exception cref="HttpResponseException">Exception is thrown if the request is bad.</exception>
        private static void Enlist(this IRequest<BulkRequest2> request, BulkRequestOperation operation, List<IBulkOperationContext> operations,
            List<IBulkCreationOperationContext> creations, List<IBulkUpdateOperationContext> updates)
        {
            if (operation == null)
            {
                throw new ArgumentNullException(nameof(operation));
            }

            if (operations == null)
            {
                throw new ArgumentNullException(nameof(operations));
            }

            if (creations == null)
            {
                throw new ArgumentNullException(nameof(creations));
            }

            if (updates == null)
            {
                throw new ArgumentNullException(nameof(updates));
            }

            if (operation.Method == null)
            {
                throw new ArgumentException(Schema.Properties.Resources.ExceptionInvalidOperation);
            }

            if (operation.Method == HttpMethod.Post)
            {
                IBulkCreationOperationContext context = new BulkCreationOperationContext(request, operation);
                context.Relate(updates);

                (IBulkOperationContext item, int index) firstDependent =
                    operations
                        .Select((item, index) => (item, index))
                        .Where(candidateItem => context.Dependents.Any(dependentItem => dependentItem == candidateItem.item))
                        .OrderBy(item => item.index)
                        .FirstOrDefault();

                if (firstDependent != default)
                {
                    operations.Insert(firstDependent.index, context);
                }
                else
                {
                    operations.Add(context);
                }

                creations.Add(context);
                operations.AddRange(context.Subordinates);
                updates.AddRange(context.Subordinates);
            }
            else if (operation.Method == HttpMethod.Delete)
            {
                IBulkOperationContext context = new BulkDeletionOperationContext(request, operation);
                operations.Add(context);
            }
            else if (operation.Method == ProtocolExtensions.PatchMethod)
            {
                IBulkUpdateOperationContext context = new BulkUpdateOperationContext(request, operation);
                context.Relate(creations);
                operations.Add(context);
                updates.Add(context);
            }
            else
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }
        }

        /// <summary>
        /// This method is used to enqueue operations.
        /// </summary>
        /// <param name="request">Contains the request.</param>
        /// <returns>Returns a queue of operation context.</returns>
        /// <exception cref="ArgumentNullException">Exception is thrown if request is not specified.</exception>
        /// <exception cref="ArgumentException">Exception is thrown if request payload is not specified.</exception>
        public static Queue<IBulkOperationContext> EnqueueOperations(this IRequest<BulkRequest2> request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (request.Payload == null)
            {
                throw new ArgumentException(Schema.Properties.Resources.ExceptionInvalidRequest);
            }

            List<IBulkCreationOperationContext> creations = new List<IBulkCreationOperationContext>();
            List<IBulkUpdateOperationContext> updates = new List<IBulkUpdateOperationContext>();
            List<IBulkOperationContext> operations = new List<IBulkOperationContext>();

            foreach (BulkRequestOperation operation in request.Payload.Operations)
            {
                request.Enlist(operation, operations, creations, updates);
            }

            Queue<IBulkOperationContext> result = new Queue<IBulkOperationContext>(operations.Count);

            foreach (IBulkOperationContext operation in operations)
            {
                result.Enqueue(operation);
            }

            return result;
        }

        /// <summary>
        /// This method is used to add a relationship operation.
        /// </summary>
        /// <param name="context">Contains the bulk update operation context.</param>
        /// <param name="updates">Contains the bulk operation updates.</param>
        /// <exception cref="ArgumentNullException">Exception is thrown if arguments are not specified.</exception>
        /// <exception cref="ArgumentException">Exception is thrown if method or operation are not specified.</exception>
        /// <exception cref="HttpResponseException">Exception is thrown if the request is bad.</exception>
        private static void Relate(this IBulkCreationOperationContext context, IEnumerable<IBulkUpdateOperationContext> updates)
        {
            if (updates == null)
            {
                throw new ArgumentNullException(nameof(updates));
            }

            if (context.Method == null)
            {
                throw new ArgumentException(Schema.Properties.Resources.ExceptionInvalidContext);
            }

            if (context.Operation == null)
            {
                throw new ArgumentException(Schema.Properties.Resources.ExceptionInvalidContext);
            }

            if (string.IsNullOrWhiteSpace(context.Operation.Identifier))
            {
                throw new ArgumentException(Schema.Properties.Resources.ExceptionInvalidOperation);
            }

            foreach (IBulkUpdateOperationContext update in updates)
            {
                switch (update.Operation.Data)
                {
                    case PatchRequest2 patchRequest:
                        if (patchRequest.References(context.Operation.Identifier))
                        {
                            context.AddDependent(update);
                            update.AddDependency(context);
                        }
                        break;

                    default:
                        throw new HttpResponseException(HttpStatusCode.BadRequest);
                }
            }
        }
    }
}