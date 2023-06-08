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
    using System.Globalization;
    using System.Linq;
    using Protocol;
    using Schema;

    /// <summary>
    /// This class implements a bulk creation operation state.
    /// </summary>
    internal class BulkCreationOperationState : BulkOperationStateBase<Resource>, IBulkCreationOperationState
    {
        /// <summary>
        /// Contains the resource identifier template.
        /// </summary>
        private const string RelativeResourceIdentifierTemplate = "/{0}/{1}";

        /// <summary>
        /// Contains a list of dependent operation contexts.
        /// </summary>
        private readonly List<IBulkUpdateOperationContext> dependents;

        /// <summary>
        /// Contains a collection of dependent operation contexts.
        /// </summary>
        private readonly IReadOnlyCollection<IBulkUpdateOperationContext> dependentsWrapper;

        /// <summary>
        /// Contains a the creation request.
        /// </summary>
        private readonly IRequest<Resource> creationRequest;

        /// <summary>
        /// Contains a list of operation context subordinates.
        /// </summary>
        private readonly List<IBulkUpdateOperationContext> subordinates;

        /// <summary>
        /// Contains a collection of subordinates.
        /// </summary>
        private readonly IReadOnlyCollection<IBulkUpdateOperationContext> subordinatesWrapper;

        /// <summary>
        /// Contains the typed context.
        /// </summary>
        private readonly IBulkCreationOperationContext typedContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="BulkCreationOperationState" /> class.
        /// </summary>
        /// <param name="request">Contains the bulk request.</param>
        /// <param name="operation">Contains the request operation.</param>
        /// <param name="context">Contains the operation context.</param>
        /// <exception cref="ArgumentException">Exception is thrown if the base resource identifier is not specified in the request.</exception>
        public BulkCreationOperationState(IRequest<BulkRequest2> request, BulkRequestOperation operation, IBulkCreationOperationContext context)
            : base(request, operation, context)
        {
            this.typedContext = context;

            this.dependents = new List<IBulkUpdateOperationContext>();
            this.dependentsWrapper = this.dependents.AsReadOnly();

            this.subordinates = new List<IBulkUpdateOperationContext>();
            this.subordinatesWrapper = this.subordinates.AsReadOnly();

            if (this.BulkRequest.BaseResourceIdentifier == null)
            {
                throw new ArgumentException(Schema.Properties.Resources.ExceptionInvalidRequest);
            }

            if (this.Operation.Data == null)
            {
                string invalidOperationExceptionMessage =
                    string.Format(CultureInfo.InvariantCulture, Schema.Properties.Resources.ExceptionInvalidOperationTemplate, operation.Identifier);
                throw new ArgumentException(invalidOperationExceptionMessage);
            }

            dynamic operationDataJson = JsonConvert.DeserializeObject(this.Operation.Data.ToString());

            if (operationDataJson?.schemas != null)
            {
                object operationData = this.Operation.Data;
                Resource resource = null;

                if (operationData.IsResourceType(SchemaIdentifiers.Core2User))
                {
                    Core2EnterpriseUserBase user = operationDataJson.ToObject<Core2EnterpriseUser>();
                    resource = user;

                    if (user.EnterpriseExtension.Manager != null)
                    {
                        string resourceIdentifier =
                            string.Format(CultureInfo.InvariantCulture, BulkCreationOperationState.RelativeResourceIdentifierTemplate,
                                ProtocolConstants.PathUsers, this.Operation.Identifier);
                        Uri patchResourceIdentifier = new Uri(resourceIdentifier, UriKind.Relative);

                        PatchOperation2Combined patchOperation = PatchOperation2Combined.Create(OperationName.Add,
                            AttributeNames.Manager, user.EnterpriseExtension.Manager.Value);
                        PatchRequest2 patchRequest = new PatchRequest2();
                        patchRequest.AddOperation(patchOperation);

                        this.AddSubordinate(patchResourceIdentifier, patchRequest, context);

                        user.EnterpriseExtension.Manager = null;
                    }
                }

                if (operationData.IsResourceType(SchemaIdentifiers.Core2Group))
                {
                    GroupBase group = operationDataJson.ToObject<Core2Group>();
                    resource = group;

                    if (group.Members != null && group.Members.Any())
                    {
                        string resourceIdentifier =
                            string.Format(
                                CultureInfo.InvariantCulture,
                                BulkCreationOperationState.RelativeResourceIdentifierTemplate,
                                ProtocolConstants.PathGroups,
                                this.Operation.Identifier);
                        Uri patchResourceIdentifier =
                            new Uri(resourceIdentifier, UriKind.Relative);

                        PatchRequest2 patchRequest = new PatchRequest2();

                        foreach (Member member in group.Members)
                        {
                            if (member != null && !string.IsNullOrWhiteSpace(member.Value))
                            {
                                string memberValue = System.Text.Json.JsonSerializer.Serialize(member);

                                if (!string.IsNullOrWhiteSpace(memberValue))
                                {
                                    PatchOperation2Combined patchOperation = PatchOperation2Combined.Create(OperationName.Add, AttributeNames.Members, memberValue);
                                    patchRequest.AddOperation(patchOperation);
                                }
                            }
                        }

                        this.AddSubordinate(patchResourceIdentifier, patchRequest, context);

                        group.Members = null;
                    }
                }

                if (resource == null)
                {
                    string invalidOperationExceptionMessage =
                        string.Format(CultureInfo.InvariantCulture, Schema.Properties.Resources.ExceptionInvalidOperationTemplate, operation.Identifier);
                    throw new ArgumentException(invalidOperationExceptionMessage);
                }

                this.creationRequest = new CreationRequest(request.Request, resource, request.CorrelationIdentifier, request.Extensions);
            }
            else
            {
                string invalidOperationExceptionMessage =
                    string.Format(CultureInfo.InvariantCulture, Schema.Properties.Resources.ExceptionInvalidOperationTemplate, operation.Identifier);
                throw new ArgumentException(invalidOperationExceptionMessage);
            }
        }

        /// <summary>
        /// Gets a collection of dependent wrapper contexts.
        /// </summary>
        public IReadOnlyCollection<IBulkUpdateOperationContext> Dependents => this.dependentsWrapper;

        /// <summary>
        /// Gets a collection of subordinate wrapper contexts.
        /// </summary>
        public IReadOnlyCollection<IBulkUpdateOperationContext> Subordinates => this.subordinatesWrapper;

        /// <summary>
        /// This method is used to add a dependent context.
        /// </summary>
        /// <param name="dependent">Contains the context to add.</param>
        /// <exception cref="ArgumentNullException">Exception thrown if the context is not specified.</exception>
        /// <exception cref="InvalidOperationException">Exception thrown if the context state does not match the received state.</exception>
        public void AddDependent(IBulkUpdateOperationContext dependent)
        {
            if (dependent == null)
            {
                throw new ArgumentNullException(nameof(dependent));
            }

            if (this.Context.State != this.Context.ReceivedState)
            {
                throw new InvalidOperationException(Schema.Properties.Resources.ExceptionInvalidState);
            }

            this.dependents.Add(dependent);
        }

        /// <summary>
        /// This method is used to add a subordinate context.
        /// </summary>
        /// <param name="subordinate">Contains the context to add.</param>
        /// <exception cref="ArgumentNullException">Exception thrown if the context is not specified.</exception>
        /// <exception cref="InvalidOperationException">Exception thrown if the context state does not match the received state.</exception>
        public void AddSubordinate(IBulkUpdateOperationContext subordinate)
        {
            if (subordinate == null)
            {
                throw new ArgumentNullException(nameof(subordinate));
            }

            if (this.Context.State != this.Context.ReceivedState)
            {
                throw new InvalidOperationException(Schema.Properties.Resources.ExceptionInvalidState);
            }

            this.subordinates.Add(subordinate);
        }

        /// <summary>
        /// This method is used to add a subordinate context.
        /// </summary>
        /// <param name="resourceIdentifier">Contains a resource identifier.</param>
        /// <param name="patchRequest">Contains a patch request.</param>
        /// <param name="context">Contains an operation context.</param>
        /// <exception cref="ArgumentNullException">Exception thrown if parameters are not specified.</exception>
        private void AddSubordinate(Uri resourceIdentifier, PatchRequest2 patchRequest, IBulkCreationOperationContext context)
        {
            if (resourceIdentifier == null)
            {
                throw new ArgumentNullException(nameof(resourceIdentifier));
            }

            if (patchRequest == null)
            {
                throw new ArgumentNullException(nameof(patchRequest));
            }

            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            BulkRequestOperation bulkPatchOperation = BulkRequestOperation.CreatePatchOperation(resourceIdentifier, patchRequest);
            IBulkUpdateOperationContext patchOperationContext = new BulkUpdateOperationContext(this.BulkRequest, bulkPatchOperation, context);
            this.AddSubordinate(patchOperationContext);
        }

        /// <summary>
        /// This method is used to complete the bulk response operation.
        /// </summary>
        /// <param name="response">Contains the operation.</param>
        /// <exception cref="ArgumentNullException">Exception is thrown if the request is not specified.</exception>
        /// <exception cref="InvalidOperationException">Exception is thrown if the state does not match pending state.</exception>
        public override void Complete(BulkResponseOperation response)
        {
            if (response == null)
            {
                throw new ArgumentNullException(nameof(response));
            }

            if (!this.typedContext.Subordinates.Any())
            {
                base.Complete(response);
            }
            else
            {
                if (this.Context.State != this.Context.PreparedState && this.typedContext.State != this.typedContext.PendingState)
                {
                    throw new InvalidOperationException(Schema.Properties.Resources.ExceptionInvalidStateTransition);
                }

                IBulkOperationState<Resource> nextState;

                if (response.Response is ErrorResponse)
                {
                    nextState = this.Context.FaultedState;
                }
                else if (this.typedContext.Subordinates.Any(item => !item.Completed))
                {
                    nextState = this.typedContext.PendingState;
                }
                else
                {
                    nextState = this.Context.ProcessedState;
                }

                if (nextState == this)
                {
                    if (this != this.typedContext.PendingState || this.Response == null)
                    {
                        this.Response = response;
                    }

                    this.Context.State = this;
                }
                else
                {
                    nextState.Complete(response);
                }
            }
        }

        /// <summary>
        /// This method is used to try and prepare a request.
        /// </summary>
        /// <param name="request">Returns the prepared request.</param>
        /// <returns>Returns a value indicating whether the request was prepared.</returns>
        public override bool TryPrepareRequest(out IRequest<Resource> request)
        {
            request = this.creationRequest;
            return true;
        }
    }
}