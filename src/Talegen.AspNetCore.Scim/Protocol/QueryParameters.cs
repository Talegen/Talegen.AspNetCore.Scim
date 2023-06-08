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
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// This class represents query parameters passed in an SCIM request.
    /// </summary>
    public sealed class QueryParameters : RetrievalParameters, IQueryParameters
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QueryParameters" /> class.
        /// </summary>
        /// <param name="schemaIdentifier">Contains a schema identifier.</param>
        /// <param name="path">Contains the path.</param>
        /// <param name="filter">Contains the filter for the query.</param>
        /// <param name="requestedAttributePaths">Contains requested attribute paths.</param>
        /// <param name="excludedAttributePaths">Contains excluded attribute paths.</param>
        /// <exception cref="ArgumentNullException">Exception is thrown if a filter is not specified.</exception>
        public QueryParameters(string schemaIdentifier, string path, IFilter filter,
            IReadOnlyCollection<string> requestedAttributePaths, IReadOnlyCollection<string> excludedAttributePaths)
            : base(schemaIdentifier, path, requestedAttributePaths, excludedAttributePaths)
        {
            if (filter == null)
            {
                throw new ArgumentNullException(nameof(filter));
            }

            this.AlternateFilters = filter.ToCollection();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryParameters" /> class.
        /// </summary>
        /// <param name="schemaIdentifier">Contains a schema identifier.</param>
        /// <param name="path">Contains the path.</param>
        /// <param name="alternateFilters">Contains the alternate filters for the query.</param>
        /// <param name="requestedAttributePaths">Contains requested attribute paths.</param>
        /// <param name="excludedAttributePaths">Contains excluded attribute paths.</param>
        /// <exception cref="ArgumentNullException">Exception is thrown if alternate filter is not specified.</exception>
        public QueryParameters(
            string schemaIdentifier,
            string path,
            IReadOnlyCollection<IFilter> alternateFilters,
            IReadOnlyCollection<string> requestedAttributePaths,
            IReadOnlyCollection<string> excludedAttributePaths)
            : base(schemaIdentifier, path, requestedAttributePaths, excludedAttributePaths)
        {
            this.AlternateFilters = alternateFilters ?? throw new ArgumentNullException(nameof(alternateFilters));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryParameters" /> class.
        /// </summary>
        /// <param name="schemaIdentifier">Contains a schema identifier.</param>
        /// <param name="path">Contains the path.</param>
        /// <param name="paginationParameters">Contains query pagination parameters.</param>
        /// <exception cref="ArgumentNullException">Exception is thrown if a pagination parameters are not specified.</exception>
        public QueryParameters(
            string schemaIdentifier,
            string path,
            IPaginationParameters paginationParameters)
            : this(schemaIdentifier, path, Array.Empty<IFilter>(), Array.Empty<string>(), Array.Empty<string>())
        {
            this.PaginationParameters = paginationParameters ?? throw new ArgumentNullException(nameof(paginationParameters));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryParameters" /> class.
        /// </summary>
        /// <param name="schemaIdentifier">Contains a schema identifier.</param>
        /// <param name="filter">Contains the filter for the query.</param>
        /// <param name="requestedAttributePaths">Contains requested attribute paths.</param>
        /// <param name="excludedAttributePaths">Contains excluded attribute paths.</param>
        [Obsolete("Use QueryParameters(string, string, IFilter, IReadOnlyCollection<string>, IReadOnlyCollection<string>) instead")]
        public QueryParameters(
            string schemaIdentifier,
            IFilter filter,
            IReadOnlyCollection<string> requestedAttributePaths,
            IReadOnlyCollection<string> excludedAttributePaths)
            : this(
                schemaIdentifier,
                new SchemaIdentifier(schemaIdentifier).FindPath(),
                filter,
                requestedAttributePaths,
                excludedAttributePaths)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QueryParameters" /> class.
        /// </summary>
        /// <param name="schemaIdentifier">Contains a schema identifier.</param>
        /// <param name="alternateFilters">Contains the alternate filter for the query.</param>
        /// <param name="requestedAttributePaths">Contains requested attribute paths.</param>
        /// <param name="excludedAttributePaths">Contains excluded attribute paths.</param>
        [Obsolete("Use QueryParameters(string, string, IReadOnlyCollection<IFilter>, IReadOnlyCollection<string>, IReadOnlyCollection<string>) instead")]
        public QueryParameters(
            string schemaIdentifier,
            IReadOnlyCollection<IFilter> alternateFilters,
            IReadOnlyCollection<string> requestedAttributePaths,
            IReadOnlyCollection<string> excludedAttributePaths)
            : this(
                schemaIdentifier,
                new SchemaIdentifier(schemaIdentifier).FindPath(),
                alternateFilters,
                requestedAttributePaths,
                excludedAttributePaths)
        {
        }

        /// <inheritdoc />
        public IReadOnlyCollection<IFilter> AlternateFilters { get; }

        /// <inheritdoc />
        public IPaginationParameters PaginationParameters { get; set; }

        /// <inheritdoc />
        public override string ToString()
        {
            string result = new Query
            {
                AlternateFilters = this.AlternateFilters,
                RequestedAttributePaths = this.RequestedAttributePaths,
                ExcludedAttributePaths = this.ExcludedAttributePaths,
                PaginationParameters = this.PaginationParameters
            }.Compose();

            return result;
        }
    }
}