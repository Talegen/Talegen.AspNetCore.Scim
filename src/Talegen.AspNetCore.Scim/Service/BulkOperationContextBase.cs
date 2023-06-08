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
    using System.Net.Http;
    using Protocol;

    /// <summary>
    /// This class implements a base bulk operation context for a given payload type.
    /// </summary>
    /// <typeparam name="TPayload">Contains the payload type.</typeparam>
    internal abstract class BulkOperationContextBase<TPayload> : IBulkOperationContext<TPayload> where TPayload : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BulkOperationContextBase{TPayload}" /> class.
        /// </summary>
        protected BulkOperationContextBase()
        {
        }

        /// <summary>
        /// Gets the state bulk request.
        /// </summary>
        public IRequest<BulkRequest2> BulkRequest => this.State.BulkRequest;

        /// <summary>
        /// Gets a value indicating whether the operation is completed.
        /// </summary>
        public bool Completed => this.State == this.ProcessedState || this.State == this.FaultedState;

        /// <summary>
        /// Gets a value indicating whether the state is faulted.
        /// </summary>
        public bool Faulted => this.State == this.FaultedState;

        /// <summary>
        /// Gets the faulted operation state.
        /// </summary>
        public IBulkOperationState<TPayload> FaultedState { get; private set; }

        /// <summary>
        /// Gets the operation HTTP method.
        /// </summary>
        public HttpMethod Method => this.State.Operation.Method;

        /// <summary>
        /// Gets the state operation.
        /// </summary>
        public BulkRequestOperation Operation => this.State.Operation;

        /// <summary>
        /// Gets the bulk operation state.
        /// </summary>
        public IBulkOperationState<TPayload> PreparedState { get; private set; }

        /// <summary>
        /// Gets the bulk operation processed state.
        /// </summary>
        public IBulkOperationState<TPayload> ProcessedState { get; private set; }

        /// <summary>
        /// Gets or sets the bulk operation received state.
        /// </summary>
        public IBulkOperationState<TPayload> ReceivedState { get; set; }

        /// <summary>
        /// Gets the operation state request.
        /// </summary>
        public IRequest<TPayload> Request => this.State.Request;

        /// <summary>
        /// Gets the operation state response.
        /// </summary>
        public BulkResponseOperation Response => this.State.Response;

        /// <summary>
        /// Gets or sets the operation state.
        /// </summary>
        public IBulkOperationState<TPayload> State { get; set; }

        /// <summary>
        /// This method is used to complete the state.
        /// </summary>
        /// <param name="response">Contains the response.</param>
        /// <exception cref="ArgumentNullException">Exception thrown if the response is not specified.</exception>
        public void Complete(BulkResponseOperation response)
        {
            if (response == null)
            {
                throw new ArgumentNullException(nameof(response));
            }

            this.State.Complete(response);
        }

        /// <summary>
        /// This method is used to initialize the received state.
        /// </summary>
        /// <param name="receivedState">Contains the received state.</param>
        /// <exception cref="ArgumentNullException">Exception thrown if the received state is not specified.</exception>
        /// <exception cref="ArgumentException">Exception is thrown if the operation or bulk request are not specified in received state.</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "False analysis of the handling of receivedState.  It is assigned as the value of properties of the derived IBulkOperationState<TPayload> type, rather than of the base IBulkOperationState type.")]
        public void Initialize(IBulkOperationState<TPayload> receivedState)
        {
            if (receivedState == null)
            {
                throw new ArgumentNullException(nameof(receivedState));
            }

            if (receivedState.Operation == null)
            {
                throw new ArgumentException(Schema.Properties.Resources.ExceptionInvalidState);
            }

            if (receivedState.BulkRequest == null)
            {
                throw new ArgumentException(Schema.Properties.Resources.ExceptionInvalidState);
            }

            this.State = this.ReceivedState = receivedState;
            this.PreparedState = new BulkOperationState<TPayload>(receivedState.BulkRequest, receivedState.Operation, this);
            this.FaultedState = new BulkOperationState<TPayload>(receivedState.BulkRequest, receivedState.Operation, this);
            this.ProcessedState = new BulkOperationState<TPayload>(receivedState.BulkRequest, receivedState.Operation, this);
        }

        /// <summary>
        /// This method is used to prepare the state with request.
        /// </summary>
        /// <param name="request">Contains the request to prepare state.</param>
        /// <exception cref="ArgumentNullException">Exception thrown if the request is not specified.</exception>
        public void Prepare(IRequest<TPayload> request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            this.State.Prepare(request);
        }

        /// <summary>
        /// This method is used to try and prepare the state.
        /// </summary>
        /// <returns>Returns a value indicating success.</returns>
        public bool TryPrepare() => this.State.TryPrepare();
    }
}