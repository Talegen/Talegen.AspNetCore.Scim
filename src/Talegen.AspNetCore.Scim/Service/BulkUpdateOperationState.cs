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
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using Protocol;

    /// <summary>
    /// This class implements a bulk update operation state.
    /// </summary>
    internal class BulkUpdateOperationState : BulkOperationStateBase<IPatch>, IBulkUpdateOperationState
    {
        /// <summary>
        /// Contains the collection of context dependencies.
        /// </summary>
        private readonly List<IBulkCreationOperationContext> dependencies;

        /// <summary>
        /// Contains a collection of dependency wrappers.
        /// </summary>
        private readonly IReadOnlyCollection<IBulkCreationOperationContext> dependenciesWrapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="BulkUpdateOperationState" /> class.
        /// </summary>
        /// <param name="request">Contains a request.</param>
        /// <param name="operation">Contains an operation.</param>
        /// <param name="context">Contains a context.</param>
        public BulkUpdateOperationState(IRequest<BulkRequest2> request, BulkRequestOperation operation, IBulkOperationContext<IPatch> context)
            : base(request, operation, context)
        {
            this.dependencies = new List<IBulkCreationOperationContext>();
            this.dependenciesWrapper = this.dependencies.AsReadOnly();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BulkUpdateOperationState" /> class.
        /// </summary>
        /// <param name="request">Contains a request.</param>
        /// <param name="operation">Contains an operation.</param>
        /// <param name="context">Contains a context.</param>
        /// <param name="parent">Contains a parent.</param>
        /// <exception cref="ArgumentNullException">Exception is thrown if the parent is not specified.</exception>
        public BulkUpdateOperationState(IRequest<BulkRequest2> request, BulkRequestOperation operation, IBulkOperationContext<IPatch> context, IBulkCreationOperationContext parent)
            : this(request, operation, context)
        {
            this.Parent = parent ?? throw new ArgumentNullException(nameof(parent));
        }

        /// <summary>
        /// Gets a collection of dependencies.
        /// </summary>
        public IReadOnlyCollection<IBulkCreationOperationContext> Dependencies => this.dependenciesWrapper;

        /// <summary>
        /// Gets the parent context.
        /// </summary>
        public IBulkCreationOperationContext Parent { get; }

        /// <summary>
        /// This method is used to add a dependency context.
        /// </summary>
        /// <param name="dependency">Contains the dependency context.</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public void AddDependency(IBulkCreationOperationContext dependency)
        {
            if (dependency == null)
            {
                throw new ArgumentNullException(nameof(dependency));
            }

            if (this.Context.State != this.Context.ReceivedState)
            {
                throw new InvalidOperationException(
                    Schema.Properties.Resources.ExceptionInvalidState);
            }

            this.dependencies.Add(dependency);
        }

        /// <summary>
        /// This method is used to complete a given operation.
        /// </summary>
        /// <param name="response">Contains the operation to complete.</param>
        /// <exception cref="ArgumentNullException">Exception is thrown if the response is not specified.</exception>
        /// <exception cref="InvalidOperationException">Exception is thrown if the context state is invalid.</exception>
        public override void Complete(BulkResponseOperation response)
        {
            if (response == null)
            {
                throw new ArgumentNullException(nameof(response));
            }

            if (this.Context.State != this.Context.ReceivedState && this.Context.State != this.Context.PreparedState)
            {
                throw new InvalidOperationException(Schema.Properties.Resources.ExceptionInvalidStateTransition);
            }

            IBulkOperationState<IPatch> completionState;

            completionState = response.Response is ErrorResponse ? this.Context.FaultedState : this.Context.ProcessedState;

            if (completionState == this)
            {
                this.Response = response;
                this.Context.State = this;

                if (this.Parent != null)
                {
                    this.Parent.Complete(response);
                }
            }
            else
            {
                completionState.Complete(response);
            }
        }

        /// <summary>
        /// This method is used to set the fault status code of the state.
        /// </summary>
        /// <param name="statusCode">Contains the status code.</param>
        /// <param name="errorType">Contains the optional error type.</param>
        private void Fault(HttpStatusCode statusCode, ErrorType? errorType = null)
        {
            ErrorResponse error = new ErrorResponse { Status = statusCode };

            if (errorType.HasValue)
            {
                error.ErrorType = errorType.Value;
            }

            BulkResponseOperation response = new BulkResponseOperation(this.Operation.Identifier) { Response = error };
            this.Complete(response);
        }

        /// <summary>
        /// This method is used to try and prepare the request.
        /// </summary>
        /// <param name="request">Contains the request.</param>
        /// <returns>Returns a value indicating whether the request was prepared.</returns>
        public override bool TryPrepareRequest(out IRequest<IPatch> request)
        {
            request = null;
            bool result = true;

            PatchRequest2 patchRequest;
            switch (this.Operation.Data)
            {
                case PatchRequest2 patchrequest2:
                    patchRequest = patchrequest2;
                    break;

                default:
                    dynamic operationDataJson = JsonConvert.DeserializeObject(Operation.Data.ToString());
                    IReadOnlyCollection<PatchOperation2Combined> patchOperations =
                        operationDataJson.Operations.ToObject<List<PatchOperation2Combined>>();
                    patchRequest = new PatchRequest2(patchOperations);
                    break;
            }

            IPatch patch = new Patch { PatchRequest = patchRequest };

            IRequest<IPatch> requestBuffer = new UpdateRequest(this.BulkRequest.Request, patch, this.BulkRequest.CorrelationIdentifier, this.BulkRequest.Extensions);

            Uri resourceIdentifier = null;

            if (this.Parent != null)
            {
                if (this.Parent.Response == null || this.Parent.Response.Location == null)
                {
                    this.Fault(HttpStatusCode.NotFound, ErrorType.noTarget);
                    result = false;
                }
                else
                {
                    resourceIdentifier = this.Parent.Response.Location;
                }
            }
            else
            {
                if (this.BulkRequest == null || this.BulkRequest.BaseResourceIdentifier == null)
                {
                    throw new InvalidOperationException(Schema.Properties.Resources.ExceptionInvalidState);
                }

                if (this.Operation.Path == null)
                {
                    this.Fault(HttpStatusCode.BadRequest);
                    result = false;
                }
                else
                {
                    resourceIdentifier = new Uri(this.BulkRequest.BaseResourceIdentifier, this.Operation.Path);
                }
            }

            if (!UniformResourceIdentifier.TryParse(resourceIdentifier, this.BulkRequest.Extensions, out IUniformResourceIdentifier parsedIdentifier)
                || parsedIdentifier == null || parsedIdentifier.Identifier == null)
            {
                this.Fault(HttpStatusCode.BadRequest);
                result = false;
            }
            else
            {
                requestBuffer.Payload.ResourceIdentifier = parsedIdentifier.Identifier;

                if (this.Dependencies.Any())
                {
                    foreach (IBulkCreationOperationContext dependency in this.Dependencies)
                    {
                        if (dependency.Response == null || dependency.Response.Location == null ||
                            !UniformResourceIdentifier.TryParse(dependency.Response.Location, this.BulkRequest.Extensions, out IUniformResourceIdentifier dependentResourceIdentifier) ||
                            dependentResourceIdentifier.Identifier == null || string.IsNullOrWhiteSpace(dependentResourceIdentifier.Identifier.Identifier))
                        {
                            this.Fault(HttpStatusCode.NotFound, ErrorType.noTarget);
                            result = false;
                            break;
                        }
                        else if (!patchRequest.TryFindReference(dependency.Operation.Identifier, out IReadOnlyCollection<OperationValue> references))
                        {
                            this.Fault(HttpStatusCode.InternalServerError);
                            result = false;
                            break;
                        }
                        else
                        {
                            foreach (OperationValue value in references)
                            {
                                value.Value = dependentResourceIdentifier.Identifier.Identifier;
                            }
                        }
                    }
                }

                request = requestBuffer;
            }

            return result;
        }
    }
}