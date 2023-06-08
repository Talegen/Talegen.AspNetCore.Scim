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
    using System.Collections.Generic;
    using Protocol;
    using Schema;

    /// <summary>
    /// This class implements a bulk creation operation context.
    /// </summary>
    internal sealed class BulkCreationOperationContext : BulkOperationContextBase<Resource>, IBulkCreationOperationContext
    {
        /// <summary>
        /// Contains the operation state.
        /// </summary>
        private readonly IBulkCreationOperationState receivedState;

        /// <summary>
        /// Initializes a new instance of the <see cref="BulkCreationOperationContext" /> class.
        /// </summary>
        /// <param name="request">Contains the bulk operation request.</param>
        /// <param name="operation">Contains the operation.</param>
        /// <exception cref="ArgumentNullException">Exception is thrown if parameters are not specified.</exception>
        public BulkCreationOperationContext(IRequest<BulkRequest2> request, BulkRequestOperation operation)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (operation == null)
            {
                throw new ArgumentNullException(nameof(operation));
            }

            this.receivedState = new BulkCreationOperationState(request, operation, this);
            this.Initialize(this.receivedState);
            this.PendingState = new BulkOperationState<Resource>(request, operation, this);
        }

        /// <summary>
        /// Gets a collection of dependent contexts.
        /// </summary>
        public IReadOnlyCollection<IBulkUpdateOperationContext> Dependents => this.receivedState.Dependents;

        /// <summary>
        /// Gets the pending state.
        /// </summary>
        public IBulkOperationState<Resource> PendingState { get; }

        /// <summary>
        /// Gets the subordinates.
        /// </summary>
        public IReadOnlyCollection<IBulkUpdateOperationContext> Subordinates => this.receivedState.Subordinates;

        /// <summary>
        /// This method is used to add a dependent operation context.
        /// </summary>
        /// <param name="dependent">Contains the operation context to add.</param>
        /// <exception cref="ArgumentNullException">Exception is thrown when parameter is not specified.</exception>
        public void AddDependent(IBulkUpdateOperationContext dependent)
        {
            if (dependent == null)
            {
                throw new ArgumentNullException(nameof(dependent));
            }

            this.receivedState.AddDependent(dependent);
        }

        /// <summary>
        /// This method is used to add a subordinate operation context.
        /// </summary>
        /// <param name="subordinate">Contains the operation context to add.</param>
        /// <exception cref="ArgumentNullException">Exception is thrown when parameter is not specified.</exception>
        public void AddSubordinate(IBulkUpdateOperationContext subordinate)
        {
            if (subordinate == null)
            {
                throw new ArgumentNullException(nameof(subordinate));
            }

            this.receivedState.AddSubordinate(subordinate);
        }
    }
}