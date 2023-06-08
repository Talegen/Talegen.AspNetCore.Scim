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
    /// This interface defines the minimum implementation of a query request.
    /// </summary>
    public interface IQuery
    {
        /// <summary>
        /// Gets or sets alternate filters collection.
        /// </summary>
        IReadOnlyCollection<IFilter> AlternateFilters { get; set; }

        /// <summary>
        /// Gets or sets excluded attribute paths.
        /// </summary>
        IReadOnlyCollection<string> ExcludedAttributePaths { get; set; }

        /// <summary>
        /// Gets or sets pagination parameters.
        /// </summary>
        IPaginationParameters PaginationParameters { get; set; }

        /// <summary>
        /// Gets or sets the path.
        /// </summary>
        string Path { get; set; }

        /// <summary>
        /// Gets or sets the requested attribute paths.
        /// </summary>
        IReadOnlyCollection<string> RequestedAttributePaths { get; set; }

        /// <summary>
        /// This method is used to serialize and compose the query to a string.
        /// </summary>
        /// <returns>Returns the query as a string.</returns>
        string Compose();
    }
}