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
    /// This class implements an invalid bulk operation state.
    /// </summary>
    internal class InvalidBulkOperationState : IBulkOperationState
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidBulkOperationState" /> class.
        /// </summary>
        /// <param name="request">Contains the bulk request.</param>
        /// <param name="operation">Contains the bulk request operation.</param>
        /// <exception cref="ArgumentNullException">Exception is thrown if parameters are not specified.</exception>
        public InvalidBulkOperationState(IRequest<BulkRequest2> request, BulkRequestOperation operation)
        {
            this.BulkRequest = request ?? throw new ArgumentNullException(nameof(request));
            this.Operation = operation ?? throw new ArgumentNullException(nameof(operation));
        }

        /// <summary>
        /// Gets the bulk request.
        /// </summary>
        public IRequest<BulkRequest2> BulkRequest { get; }

        /// <summary>
        /// Gets the bulk request operation.
        /// </summary>
        public BulkRequestOperation Operation { get; }

        /// <summary>
        /// Gets the bulk response.
        /// </summary>
        public BulkResponseOperation Response { get; private set; }

        /// <summary>
        /// This method is used to complete the response.
        /// </summary>
        /// <param name="response">Contains the response.</param>
        /// <exception cref="ArgumentNullException">Exception is thrown if the response is not specified.</exception>
        /// <exception cref="ArgumentException">Exception is thrown if there is an invalid response.</exception>
        public void Complete(BulkResponseOperation response)
        {
            if (response == null)
            {
                throw new ArgumentNullException(nameof(response));
            }

            if (response.Response is ErrorResponse)
            {
                this.Response = response;
            }
            else
            {
                throw new ArgumentException(Schema.Properties.Resources.ExceptionInvalidResponse);
            }
        }

        /// <summary>
        /// This method is used to try and prepare the state.
        /// </summary>
        /// <returns>Returns a value indicating whether the state is prepared.</returns>
        public bool TryPrepare() => false;
    }
}