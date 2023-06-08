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
    using Schema;

    /// <summary>
    /// This interface defines the minimum implementation of a bulk creation operation state.
    /// </summary>
    public interface IBulkCreationOperationState : IBulkOperationState<Resource>
    {
        /// <summary>
        /// Gets a collection of dependents.
        /// </summary>
        IReadOnlyCollection<IBulkUpdateOperationContext> Dependents { get; }

        /// <summary>
        /// Gets a collection of subordinates.
        /// </summary>
        IReadOnlyCollection<IBulkUpdateOperationContext> Subordinates { get; }

        /// <summary>
        /// This method is used to add a dependent operation context.
        /// </summary>
        /// <param name="dependent">Contains the update operation context.</param>
        void AddDependent(IBulkUpdateOperationContext dependent);

        /// <summary>
        /// This method is used to add a subordinate operation context.
        /// </summary>
        /// <param name="subordinate">Contains the update operation context.</param>
        void AddSubordinate(IBulkUpdateOperationContext subordinate);
    }
}