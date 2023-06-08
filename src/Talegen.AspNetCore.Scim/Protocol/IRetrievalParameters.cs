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

namespace Talegen.AspNetCore.Scim.Protocol
{
    using System.Collections.Generic;

    /// <summary>
    /// This interface defines the minimum implementation of a retrieval parameters object.
    /// </summary>
    public interface IRetrievalParameters
    {
        /// <summary>
        /// Gets the excluded attribute paths.
        /// </summary>
        IReadOnlyCollection<string> ExcludedAttributePaths { get; }

        /// <summary>
        /// Gets the path.
        /// </summary>
        string Path { get; }

        /// <summary>
        /// Gets a collection of requested attribute paths.
        /// </summary>
        IReadOnlyCollection<string> RequestedAttributePaths { get; }

        /// <summary>
        /// Gets the schema identifier.
        /// </summary>
        string SchemaIdentifier { get; }
    }
}