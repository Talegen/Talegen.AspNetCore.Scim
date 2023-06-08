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
    /// This class implements a bulk operation state for a given payload type.
    /// </summary>
    /// <typeparam name="TPayload">Contains the payload type.</typeparam>
    internal class BulkOperationState<TPayload> : BulkOperationStateBase<TPayload> where TPayload : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BulkOperationState{TPayload}" /> class.
        /// </summary>
        /// <param name="request">Contains the request.</param>
        /// <param name="operation">Contains the operation.</param>
        /// <param name="context">Contains the context.</param>
        public BulkOperationState(IRequest<BulkRequest2> request, BulkRequestOperation operation, IBulkOperationContext<TPayload> context)
            : base(request, operation, context)
        {
        }

        /// <summary>
        /// This method is called to try and prepare a request.
        /// </summary>
        /// <param name="request">Contains the request.</param>
        /// <returns>Returns a value indicating success.</returns>
        /// <exception cref="NotImplementedException">Exception is thrown as this feature has not been implemented.</exception>
        public override bool TryPrepareRequest(out IRequest<TPayload> request)
        {
            throw new NotImplementedException();
        }
    }
}