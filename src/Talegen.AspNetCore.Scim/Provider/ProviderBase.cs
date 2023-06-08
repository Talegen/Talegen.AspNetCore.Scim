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

namespace Talegen.AspNetCore.Scim.Provider
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Protocol;
    using Schema;
    using Service;

    /// <summary>
    /// This class implements the base implementation of a provider.
    /// </summary>
    public abstract class ProviderBase : IProvider
    {
        /// <summary>
        /// Contains the bulk feature support.
        /// </summary>
        private static readonly Lazy<BulkRequestsFeature> BulkFeatureSupport = new Lazy<BulkRequestsFeature>(() => BulkRequestsFeature.CreateUnsupportedFeature());

        /// <summary>
        /// Contains the support types.
        /// </summary>
        private static readonly Lazy<IReadOnlyCollection<TypeScheme>> TypeSchema = new Lazy<IReadOnlyCollection<TypeScheme>>(() => Array.Empty<TypeScheme>());

        /// <summary>
        /// Contains the service configuration.
        /// </summary>
        private static readonly Lazy<ServiceConfigurationBase> ServiceConfiguration = new Lazy<ServiceConfigurationBase>(() => new Core2ServiceConfiguration(ProviderBase.BulkFeatureSupport.Value, false, true, false, true, false));

        /// <summary>
        /// Contains supported types.
        /// </summary>
        private static readonly Lazy<IReadOnlyCollection<Core2ResourceType>> Types = new Lazy<IReadOnlyCollection<Core2ResourceType>>(() => Array.Empty<Core2ResourceType>());

        /// <inheritdoc />
        public virtual bool AcceptLargeObjects { get; set; }

        /// <inheritdoc />
        public virtual ServiceConfigurationBase Configuration => ProviderBase.ServiceConfiguration.Value;

        ////public virtual IEventTokenHandler EventHandler
        ////{
        ////    get;
        ////    set;
        ////}

        /// <inheritdoc />
        public virtual IReadOnlyCollection<IExtension> Extensions => null;

        /// <inheritdoc />
        public virtual IResourceJsonDeserializingFactory<GroupBase> GroupDeserializationBehavior => null;

        /// <inheritdoc />
        public virtual ISchematizedJsonDeserializingFactory<PatchRequest2> PatchRequestDeserializationBehavior => null;

        /// <inheritdoc />
        public virtual IReadOnlyCollection<Core2ResourceType> ResourceTypes => ProviderBase.Types.Value;

        /// <inheritdoc />
        public virtual IReadOnlyCollection<TypeScheme> Schema => ProviderBase.TypeSchema.Value;

        ////public virtual Action<IAppBuilder, HttpConfiguration> StartupBehavior
        ////{
        ////    get
        ////    {
        ////        return null;
        ////    }
        ////}

        /// <inheritdoc />
        public virtual IResourceJsonDeserializingFactory<Core2UserBase> UserDeserializationBehavior => null;

        /// <summary>
        /// This method implements a create resource operation asynchronously.
        /// </summary>
        /// <param name="resource">Contains the resource.</param>
        /// <param name="correlationIdentifier">Contains the correlation identifier.</param>
        /// <returns>Returns a <see cref="Resource" /> object.</returns>
        public abstract Task<Resource> CreateAsync(Resource resource, string correlationIdentifier);

        /// <inheritdoc />
        public virtual async Task<Resource> CreateAsync(IRequest<Resource> request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (request.Payload == null)
            {
                throw new ArgumentException(Scim.Schema.Properties.Resources.ExceptionInvalidRequest);
            }

            if (string.IsNullOrWhiteSpace(request.CorrelationIdentifier))
            {
                throw new ArgumentException(Scim.Schema.Properties.Resources.ExceptionInvalidRequest);
            }

            Resource result = await this.CreateAsync(request.Payload, request.CorrelationIdentifier).ConfigureAwait(false);
            return result;
        }

        /// <summary>
        /// This method implements a delete resource operation asynchronously.
        /// </summary>
        /// <param name="resourceIdentifier">Contains the resource identifier.</param>
        /// <param name="correlationIdentifier">Contains the correlation identifier.</param>
        /// <returns>Returns an async task object.</returns>
        public abstract Task DeleteAsync(IResourceIdentifier resourceIdentifier, string correlationIdentifier);

        /// <inheritdoc />
        public virtual async Task DeleteAsync(IRequest<IResourceIdentifier> request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (request.Payload == null)
            {
                throw new ArgumentException(Scim.Schema.Properties.Resources.ExceptionInvalidRequest);
            }

            if (string.IsNullOrWhiteSpace(request.CorrelationIdentifier))
            {
                throw new ArgumentException(Scim.Schema.Properties.Resources.ExceptionInvalidRequest);
            }

            await this.DeleteAsync(request.Payload, request.CorrelationIdentifier).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public virtual async Task<QueryResponseBase> PaginateQueryAsync(IRequest<IQueryParameters> request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            IReadOnlyCollection<Resource> resources = await this.QueryAsync(request);
            QueryResponseBase result = new QueryResponse(resources);
            result.TotalResults = result.ItemsPerPage = resources.Count;
            result.StartIndex = resources.Any() ? 1 : null;
            return result;
        }

        /// <inheritdoc />
        public virtual async Task<BulkResponse2> ProcessAsync(IRequest<BulkRequest2> request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (request.Request == null)
            {
                throw new ArgumentException(Scim.Schema.Properties.Resources.ExceptionInvalidRequest);
            }

            Queue<IBulkOperationContext> operations = request.EnqueueOperations();
            BulkResponse2 result = await this.ProcessAsync(operations);
            return result;
        }

        /// <summary>
        /// This method is used to process a bulk operation asynchronously.
        /// </summary>
        /// <param name="operation">Contains the bulk operation context.</param>
        /// <exception cref="ArgumentNullException">Exception is thrown if an operation is not specified.</exception>
        /// <exception cref="ArgumentException">Exception is thrown if the operation or operation method are not specified.</exception>
        public virtual async Task ProcessAsync(IBulkOperationContext operation)
        {
            if (operation == null)
            {
                throw new ArgumentNullException(nameof(operation));
            }

            if (operation.TryPrepare())
            {
                if (operation.Method == null)
                {
                    throw new ArgumentException(Scim.Schema.Properties.Resources.ExceptionInvalidOperation);
                }

                if (operation.Operation == null)
                {
                    throw new ArgumentException(Scim.Schema.Properties.Resources.ExceptionInvalidOperation);
                }

                BulkResponseOperation response = new BulkResponseOperation(operation.Operation.Identifier)
                {
                    Method = operation.Method
                };

                if (operation.Method == HttpMethod.Delete)
                {
                    IBulkOperationContext<IResourceIdentifier> context = (IBulkOperationContext<IResourceIdentifier>)operation;
                    await this.DeleteAsync(context.Request);
                    response.Status = HttpStatusCode.NoContent;
                }
                else if (operation.Method == HttpMethod.Get)
                {
                    switch (operation)
                    {
                        case IBulkOperationContext<IResourceRetrievalParameters> retrievalContext:
                            response.Response = await this.RetrieveAsync(retrievalContext.Request);
                            break;

                        default:
                            IBulkOperationContext<IQueryParameters> queryContext = (IBulkOperationContext<IQueryParameters>)operation;
                            response.Response = await this.QueryAsync(queryContext.Request);
                            break;
                    }

                    response.Status = HttpStatusCode.OK;
                }
                else if (ProtocolExtensions.PatchMethod == operation.Method)
                {
                    IBulkOperationContext<IPatch> context = (IBulkOperationContext<IPatch>)operation;
                    await this.UpdateAsync(context.Request);
                    response.Status = HttpStatusCode.OK;
                }
                else if (HttpMethod.Post == operation.Method)
                {
                    IBulkOperationContext<Resource> context = (IBulkOperationContext<Resource>)operation;
                    Resource output = await this.CreateAsync(context.Request);
                    response.Status = HttpStatusCode.Created;
                    response.Location = output.GetResourceIdentifier(context.BulkRequest.BaseResourceIdentifier);
                }
                else
                {
                    string exceptionMessage = string.Format(CultureInfo.InvariantCulture, Scim.Schema.Properties.Resources.ExceptionMethodNotSupportedTemplate, operation.Method);

                    ErrorResponse error = new ErrorResponse
                    {
                        Status = HttpStatusCode.BadRequest,
                        Detail = exceptionMessage
                    };

                    response.Response = error;
                    response.Status = HttpStatusCode.BadRequest;
                }

                operation.Complete(response);
            }
        }

        /// <summary>
        /// This method is used to process a queue of bulk operations asynchronously.
        /// </summary>
        /// <param name="operations">Contains the queue of bulk operation contexts.</param>
        /// <returns>Returns a <see cref="BulkResponse2" /> object.</returns>
        /// <exception cref="ArgumentNullException">Exception is thrown if operations are not specified.</exception>
        public virtual async Task<BulkResponse2> ProcessAsync(Queue<IBulkOperationContext> operations)
        {
            if (operations == null)
            {
                throw new ArgumentNullException(nameof(operations));
            }

            BulkResponse2 result = new BulkResponse2();
            int countFailures = 0;

            while (operations.Any())
            {
                bool addOperation;
                IBulkOperationContext operation = operations.Dequeue();
                await this.ProcessAsync(operation).ConfigureAwait(false);

                switch (operation)
                {
                    case IBulkUpdateOperationContext updateOperation:
                        addOperation = updateOperation.Parent == null;
                        break;

                    default:
                        addOperation = true;
                        break;
                }

                if (addOperation)
                {
                    result.AddOperation(operation.Response);
                }

                if (operation.Response.IsError())
                {
                    checked
                    {
                        countFailures++;
                    }
                }

                if (operation.BulkRequest.Payload.FailOnErrors.HasValue &&
                    operation.BulkRequest.Payload.FailOnErrors.Value < countFailures)
                {
                    break;
                }
            }

            return result;
        }

        /// <summary>
        /// This method is used to query resources using query parameters asynchronously.
        /// </summary>
        /// <param name="parameters">Contains the query parameters.</param>
        /// <param name="correlationIdentifier">Contains the correlation identifier.</param>
        /// <returns>Returns an array of <see cref="Resource" /> objects found.</returns>
        /// <exception cref="NotImplementedException">This is currently not implemented.</exception>
        public virtual Task<Resource[]> QueryAsync(IQueryParameters parameters, string correlationIdentifier)
        {
            // TODO: implement the ability to query with IQueryParameters
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public virtual async Task<Resource[]> QueryAsync(IRequest<IQueryParameters> request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (request.Payload == null)
            {
                throw new ArgumentException(Scim.Schema.Properties.Resources.ExceptionInvalidRequest);
            }

            if (string.IsNullOrWhiteSpace(request.CorrelationIdentifier))
            {
                throw new ArgumentException(Scim.Schema.Properties.Resources.ExceptionInvalidRequest);
            }

            Resource[] result = await this.QueryAsync(request.Payload, request.CorrelationIdentifier).ConfigureAwait(false);
            return result;
        }

        /// <summary>
        /// This method is used to replace a resource asynchronously.
        /// </summary>
        /// <param name="resource">Contains the resource.</param>
        /// <param name="correlationIdentifier">Contains the correlation identifier.</param>
        /// <returns>Returns a <see cref="Resource" /> object.</returns>
        /// <exception cref="NotSupportedException">This is currently not implemented.</exception>
        public virtual Task<Resource> ReplaceAsync(Resource resource, string correlationIdentifier)
        {
            // TODO: implement the ability to replace a resource.
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public virtual async Task<Resource> ReplaceAsync(IRequest<Resource> request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (request.Payload == null)
            {
                throw new ArgumentException(Scim.Schema.Properties.Resources.ExceptionInvalidRequest);
            }

            if (string.IsNullOrWhiteSpace(request.CorrelationIdentifier))
            {
                throw new ArgumentException(Scim.Schema.Properties.Resources.ExceptionInvalidRequest);
            }

            Resource result = await this.ReplaceAsync(request.Payload, request.CorrelationIdentifier).ConfigureAwait(false);
            return result;
        }

        /// <summary>
        /// This method implements retrieval of a resource asynchronously.
        /// </summary>
        /// <param name="parameters">Contains the parameters to find the resource.</param>
        /// <param name="correlationIdentifier">Contains the correlation identifier.</param>
        /// <returns>Returns a <see cref="Resource" /> object.</returns>
        public abstract Task<Resource> RetrieveAsync(IResourceRetrievalParameters parameters, string correlationIdentifier);

        /// <inheritdoc />
        public virtual async Task<Resource> RetrieveAsync(IRequest<IResourceRetrievalParameters> request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (request.Payload == null)
            {
                throw new ArgumentException(Scim.Schema.Properties.Resources.ExceptionInvalidRequest);
            }

            if (string.IsNullOrWhiteSpace(request.CorrelationIdentifier))
            {
                throw new ArgumentException(Scim.Schema.Properties.Resources.ExceptionInvalidRequest);
            }

            Resource result = await this.RetrieveAsync(request.Payload, request.CorrelationIdentifier).ConfigureAwait(false);
            return result;
        }

        /// <summary>
        /// This method implements the patch updating of a resource.
        /// </summary>
        /// <param name="patch">Contains the patch.</param>
        /// <param name="correlationIdentifier">Contains the correlation identifier.</param>
        /// <returns>Returns an async task object.</returns>
        public abstract Task UpdateAsync(IPatch patch, string correlationIdentifier);

        /// <inheritdoc />
        public virtual async Task UpdateAsync(IRequest<IPatch> request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (request.Payload == null)
            {
                throw new ArgumentException(Scim.Schema.Properties.Resources.ExceptionInvalidRequest);
            }

            if (string.IsNullOrWhiteSpace(request.CorrelationIdentifier))
            {
                throw new ArgumentException(Scim.Schema.Properties.Resources.ExceptionInvalidRequest);
            }

            await this.UpdateAsync(request.Payload, request.CorrelationIdentifier).ConfigureAwait(false);
        }
    }
}