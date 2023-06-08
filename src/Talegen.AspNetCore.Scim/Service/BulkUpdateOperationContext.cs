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

    /// <summary>
    /// This class implements a bulk update operation context.
    /// </summary>
    internal sealed class BulkUpdateOperationContext : BulkOperationContextBase<IPatch>, IBulkUpdateOperationContext
    {
        /// <summary>
        /// Contains the received state.
        /// </summary>
        private readonly IBulkUpdateOperationState receivedState;

        /// <summary>
        /// Initializes a new instance of the <see cref="BulkUpdateOperationContext" /> class.
        /// </summary>
        /// <param name="request">Contains the request.</param>
        /// <param name="operation">Contains the bulk operation.</param>
        /// <exception cref="ArgumentNullException">Exception is thrown if any parameter is not specified.</exception>
        public BulkUpdateOperationContext(IRequest<BulkRequest2> request, BulkRequestOperation operation)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (operation == null)
            {
                throw new ArgumentNullException(nameof(operation));
            }

            this.receivedState = new BulkUpdateOperationState(request, operation, this);
            this.Initialize(this.receivedState);
        }

        public BulkUpdateOperationContext(IRequest<BulkRequest2> request, BulkRequestOperation operation, IBulkCreationOperationContext parent)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (operation == null)
            {
                throw new ArgumentNullException(nameof(operation));
            }

            this.receivedState = new BulkUpdateOperationState(request, operation, this, parent);
            this.Initialize(this.receivedState);
        }

        /// <summary>
        /// Gets a collection of operation context dependencies.
        /// </summary>
        public IReadOnlyCollection<IBulkCreationOperationContext> Dependencies => this.receivedState.Dependencies;

        /// <summary>
        /// Gets the state parent.
        /// </summary>
        public IBulkCreationOperationContext Parent => this.receivedState.Parent;

        /// <summary>
        /// This method is used to add a dependency to the dependencies collection.
        /// </summary>
        /// <param name="dependency">Contains the operation context dependency.</param>
        /// <exception cref="ArgumentNullException">Exception thrown if the dependency is not specified.</exception>
        public void AddDependency(IBulkCreationOperationContext dependency)
        {
            if (dependency == null)
            {
                throw new ArgumentNullException(nameof(dependency));
            }

            this.receivedState.AddDependency(dependency);
        }
    }
}