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
    /// This class implements a bulk deletion operation context.
    /// </summary>
    internal sealed class BulkDeletionOperationContext : BulkOperationContextBase<IResourceIdentifier>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BulkDeletionOperationContext" /> class.
        /// </summary>
        /// <param name="request">Contains a bulk request.</param>
        /// <param name="operation">Contains the bulk request operation.</param>
        /// <exception cref="ArgumentNullException">Exception is thrown if parameters are not specified.</exception>
        public BulkDeletionOperationContext(IRequest<BulkRequest2> request, BulkRequestOperation operation)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (operation == null)
            {
                throw new ArgumentNullException(nameof(operation));
            }

            IBulkOperationState<IResourceIdentifier> receivedState = new BulkDeletionOperationState(request, operation, this);
            this.Initialize(receivedState);
        }
    }
}