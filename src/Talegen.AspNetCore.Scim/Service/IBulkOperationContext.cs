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
    using System.Net.Http;

    /// <summary>
    /// This interface defines the minimum implementation of a bulk operation context.
    /// </summary>
    public interface IBulkOperationContext : IBulkOperationState
    {
        /// <summary>
        /// Gets a value indicating whether the context was completed.
        /// </summary>
        bool Completed { get; }

        /// <summary>
        /// Gets a value indicating whether the context faulted.
        /// </summary>
        bool Faulted { get; }

        /// <summary>
        /// Gets the HTTP method.
        /// </summary>
        HttpMethod Method { get; }
    }

    /// <summary>
    /// This interface defines the minimum implementation of a bulk creation operation context with payload.
    /// </summary>
    public interface IBulkOperationContext<TPayload> : IBulkOperationContext, IBulkOperationState<TPayload> where TPayload : class
    {
        /// <summary>
        /// Gets the operation fault state.
        /// </summary>
        IBulkOperationState<TPayload> FaultedState { get; }

        /// <summary>
        /// Gets the prepared state.
        /// </summary>
        IBulkOperationState<TPayload> PreparedState { get; }

        /// <summary>
        /// Gets the processed state.
        /// </summary>
        IBulkOperationState<TPayload> ProcessedState { get; }

        /// <summary>
        /// Gets the received state.
        /// </summary>
        IBulkOperationState<TPayload> ReceivedState { get; }

        /// <summary>
        /// Gets the state.
        /// </summary>
        IBulkOperationState<TPayload> State { get; set; }
    }
}