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
    using System.Collections.Generic;

    /// <summary>
    /// This interface defines the minimum implementation of a bulk operation state.
    /// </summary>
    public interface IBulkUpdateOperationState : IBulkOperationState<IPatch>
    {
        /// <summary>
        /// Gets a collection of dependencies.
        /// </summary>
        IReadOnlyCollection<IBulkCreationOperationContext> Dependencies { get; }

        /// <summary>
        /// Gets the operation parent.
        /// </summary>
        IBulkCreationOperationContext Parent { get; }

        /// <summary>
        /// This method is used to add a new dependency to the collection.
        /// </summary>
        /// <param name="dependency">Contains the operation context to add.</param>
        void AddDependency(IBulkCreationOperationContext dependency);
    }
}