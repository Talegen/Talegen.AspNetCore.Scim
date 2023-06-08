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
    /// This class implements a retrieval parameters object.
    /// </summary>
    public abstract class RetrievalParameters : IRetrievalParameters
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RetrievalParameters" /> class.
        /// </summary>
        /// <param name="schemaIdentifier">Contains the schema identifier.</param>
        /// <param name="path">Contains the path.</param>
        /// <param name="requestedAttributePaths">Contains the requested attribute paths.</param>
        /// <param name="excludedAttributePaths">Contains the excluded attribute paths.</param>
        /// <exception cref="ArgumentNullException">Exception is thrown if parameters are not specified.</exception>
        protected RetrievalParameters(string schemaIdentifier, string path,
            IReadOnlyCollection<string> requestedAttributePaths, IReadOnlyCollection<string> excludedAttributePaths)
        {
            if (string.IsNullOrWhiteSpace(schemaIdentifier))
            {
                throw new ArgumentNullException(nameof(schemaIdentifier));
            }

            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentNullException(nameof(path));
            }

            this.SchemaIdentifier = schemaIdentifier;
            this.Path = path;
            this.RequestedAttributePaths = requestedAttributePaths ?? throw new ArgumentNullException(nameof(requestedAttributePaths));
            this.ExcludedAttributePaths = excludedAttributePaths ?? throw new ArgumentNullException(nameof(excludedAttributePaths));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RetrievalParameters" /> class.
        /// </summary>
        /// <param name="schemaIdentifier">Contains the schema identifier.</param>
        /// <param name="path">Contains the path.</param>
        /// <exception cref="ArgumentNullException">Exception is thrown if parameters are not specified.</exception>
        protected RetrievalParameters(string schemaIdentifier, string path)
        {
            if (string.IsNullOrWhiteSpace(schemaIdentifier))
            {
                throw new ArgumentNullException(nameof(schemaIdentifier));
            }

            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentNullException(nameof(path));
            }

            this.SchemaIdentifier = schemaIdentifier;
            this.Path = path;
            this.RequestedAttributePaths = Array.Empty<string>();
            this.ExcludedAttributePaths = Array.Empty<string>();
        }

        /// <inheritdoc />
        public IReadOnlyCollection<string> ExcludedAttributePaths { get; }

        /// <inheritdoc />
        public string Path { get; }

        /// <inheritdoc />
        public IReadOnlyCollection<string> RequestedAttributePaths { get; }

        /// <inheritdoc />
        public string SchemaIdentifier { get; }

        /// <inheritdoc />
        public override string ToString()
        {
            string result = new Query
            {
                RequestedAttributePaths = this.RequestedAttributePaths,
                ExcludedAttributePaths = this.ExcludedAttributePaths
            }.Compose();
            return result;
        }
    }
}