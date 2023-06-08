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
    using Protocol;

    /// <summary>
    /// This class implements a base bulk operation state for a given payload type.
    /// </summary>
    /// <typeparam name="TPayload">Contains the payload type.</typeparam>
    internal abstract class BulkOperationStateBase<TPayload> : IBulkOperationState<TPayload> where TPayload : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BulkOperationStateBase{TPayload}" /> class.
        /// </summary>
        /// <param name="request">Contains a bulk request object.</param>
        /// <param name="operation">Contains a bulk request operation.</param>
        /// <param name="context">Contains a bulk operation context of the given type.</param>
        /// <exception cref="ArgumentNullException">Exception is thrown if any parameter is not specified.</exception>
        protected BulkOperationStateBase(IRequest<BulkRequest2> request, BulkRequestOperation operation, IBulkOperationContext<TPayload> context)
        {
            this.BulkRequest = request ?? throw new ArgumentNullException(nameof(request));
            this.Operation = operation ?? throw new ArgumentNullException(nameof(operation));
            this.Context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <summary>
        /// Gets the bulk request.
        /// </summary>
        public IRequest<BulkRequest2> BulkRequest { get; }

        /// <summary>
        /// Gets the context.
        /// </summary>
        public IBulkOperationContext<TPayload> Context { get; }

        /// <summary>
        /// Gets or sets the operation.
        /// </summary>
        public BulkRequestOperation Operation { get; set; }

        /// <summary>
        /// Gets the request.
        /// </summary>
        public IRequest<TPayload> Request { get; private set; }

        /// <summary>
        /// Gets or sets the bulk response.
        /// </summary>
        public BulkResponseOperation Response { get; set; }

        /// <summary>
        /// This method is used to complete the bulk response operation.
        /// </summary>
        /// <param name="response">Contains the bulk response operation.</param>
        /// <exception cref="ArgumentNullException">Exception is thrown if the operation is not specified.</exception>
        /// <exception cref="InvalidOperationException">Exception is thrown if the operation state is not a prepared state.</exception>
        public virtual void Complete(BulkResponseOperation response)
        {
            if (response == null)
            {
                throw new ArgumentNullException(nameof(response));
            }

            ErrorResponse errorResponse = response.Response as ErrorResponse;

            if (this.Context.State != this.Context.PreparedState && errorResponse == null)
            {
                throw new InvalidOperationException(Schema.Properties.Resources.ExceptionInvalidStateTransition);
            }

            IBulkOperationState<TPayload> completionState;

            completionState = errorResponse != null ? this.Context.FaultedState : this.Context.ProcessedState;

            if (completionState == this)
            {
                this.Response = response;
                this.Context.State = this;
            }
            else
            {
                completionState.Complete(response);
            }
        }

        /// <summary>
        /// This method is used to prepare the request.
        /// </summary>
        /// <param name="request">Contains the request.</param>
        /// <exception cref="InvalidOperationException">Exception is thrown if the prepared state does not match/</exception>
        /// <exception cref="ArgumentNullException">Exception is thrown if the request is not specified.</exception>
        public virtual void Prepare(IRequest<TPayload> request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (this.Context.State != this.Context.ReceivedState)
            {
                throw new InvalidOperationException(Schema.Properties.Resources.ExceptionInvalidStateTransition);
            }

            if (this.Context.PreparedState != this)
            {
                throw new InvalidOperationException(Schema.Properties.Resources.ExceptionInvalidStateTransition);
            }

            this.Request = request ?? throw new ArgumentNullException(nameof(request));
            this.Context.State = this;
        }

        /// <summary>
        /// This method is used to try and prepare the base state.
        /// </summary>
        /// <returns>Returns a value indicating whether the operation state was prepared.</returns>
        /// <exception cref="InvalidOperationException"></exception>
        public virtual bool TryPrepare()
        {
            bool result = true;

            if (this.Context.State != this.Context.ReceivedState)
            {
                throw new InvalidOperationException(Schema.Properties.Resources.ExceptionInvalidStateTransition);
            }

            if (!this.TryPrepareRequest(out IRequest<TPayload> request))
            {
                if (this.Context.State != this.Context.FaultedState)
                {
                    this.Context.State = this.Context.FaultedState;
                }

                result = false;
            }

            this.Context.PreparedState.Prepare(request);

            return result;
        }

        /// <summary>
        /// This method is used to try and prepare the request.
        /// </summary>
        /// <param name="request">Contains the request.</param>
        /// <returns>Returns a value indicating whether the request was prepared.</returns>
        public abstract bool TryPrepareRequest(out IRequest<TPayload> request);
    }
}