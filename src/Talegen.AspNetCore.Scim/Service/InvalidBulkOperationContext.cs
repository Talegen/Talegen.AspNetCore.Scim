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
    /// This class implements an invalid bulk operation context.
    /// </summary>
    internal class InvalidBulkOperationContext : IBulkOperationContext
    {
        /// <summary>
        /// Contains the bulk operation state.
        /// </summary>
        private readonly IBulkOperationState state;

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidBulkOperationContext" /> class.
        /// </summary>
        /// <param name="request">Contains the bulk request.</param>
        /// <param name="operation">Contains the bulk request operation.</param>
        /// <exception cref="ArgumentNullException">Exception is thrown if parameters are not specified.</exception>
        public InvalidBulkOperationContext(IRequest<BulkRequest2> request, BulkRequestOperation operation)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (operation == null)
            {
                throw new ArgumentNullException(nameof(operation));
            }

            this.state = new InvalidBulkOperationState(request, operation);
        }

        /// <summary>
        /// Gets a value indicating whether operation is completed.
        /// </summary>
        public bool Completed => true;

        /// <summary>
        /// Gets a value indicating whether operation is faulted.
        /// </summary>
        public bool Faulted => true;

        /// <summary>
        /// Gets the bulk request.
        /// </summary>
        public IRequest<BulkRequest2> BulkRequest => this.state.BulkRequest;

        /// <summary>
        /// Gets the operation method.
        /// </summary>
        public HttpMethod Method => this.state.Operation.Method;

        /// <summary>
        /// Gets the state operation.
        /// </summary>
        public BulkRequestOperation Operation => this.state.Operation;

        /// <summary>
        /// Gets the response.
        /// </summary>
        public BulkResponseOperation Response => this.state.Response;

        /// <summary>
        /// This method is used to complete the response.
        /// </summary>
        /// <param name="response">Contains the response.</param>
        public void Complete(BulkResponseOperation response) => this.state.Complete(response);

        /// <summary>
        /// This method is used to try and prepare the state.
        /// </summary>
        /// <returns>Returns a value indicating whether the state is prepared.</returns>
        public bool TryPrepare() => this.state.TryPrepare();
    }
}