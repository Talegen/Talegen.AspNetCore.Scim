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
    using Protocol;

    /// <summary>
    /// This interface defines the minimum implementation of a bulk operation state.
    /// </summary>
    public interface IBulkOperationState
    {
        /// <summary>
        /// Gets the bulk request.
        /// </summary>
        IRequest<BulkRequest2> BulkRequest { get; }

        /// <summary>
        /// Gets the bulk request operation.
        /// </summary>
        BulkRequestOperation Operation { get; }

        /// <summary>
        /// Gets the bulk request response.
        /// </summary>
        BulkResponseOperation Response { get; }

        /// <summary>
        /// This method is used to complete a bulk response operation.
        /// </summary>
        /// <param name="response"></param>
        void Complete(BulkResponseOperation response);

        /// <summary>
        /// This method is used to try and prepare.
        /// </summary>
        /// <returns>Returns a value indicating success.</returns>
        bool TryPrepare();
    }

    /// <summary>
    /// This interface defines the minimum implementation of a bulk operation state with payload.
    /// </summary>
    public interface IBulkOperationState<TPayload> : IBulkOperationState where TPayload : class
    {
        /// <summary>
        /// This method is used to prepare the request.
        /// </summary>
        /// <param name="request"></param>
        void Prepare(IRequest<TPayload> request);

        /// <summary>
        /// Gets the request with payload.
        /// </summary>
        IRequest<TPayload> Request { get; }
    }
}