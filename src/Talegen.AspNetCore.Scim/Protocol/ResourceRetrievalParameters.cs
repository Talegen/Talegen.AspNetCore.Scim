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
    /// This method implements resource retrieval parameters.
    /// </summary>
    public sealed class ResourceRetrievalParameters : RetrievalParameters, IResourceRetrievalParameters
    {
        /// <inheritdoc />
        public ResourceRetrievalParameters(
            string schemaIdentifier,
            string path,
            string resourceIdentifier,
            IReadOnlyCollection<string> requestedAttributePaths,
            IReadOnlyCollection<string> excludedAttributePaths)
            : base(schemaIdentifier, path, requestedAttributePaths, excludedAttributePaths)
        {
            if (resourceIdentifier == null)
            {
                throw new ArgumentNullException(nameof(resourceIdentifier));
            }

            this.ResourceIdentifier = new ResourceIdentifier
            {
                Identifier = resourceIdentifier,
                SchemaIdentifier = this.SchemaIdentifier
            };
        }

        /// <inheritdoc />
        public ResourceRetrievalParameters(string schemaIdentifier, string path, string resourceIdentifier)
            : base(schemaIdentifier, path)
        {
            if (resourceIdentifier == null)
            {
                throw new ArgumentNullException(nameof(resourceIdentifier));
            }

            this.ResourceIdentifier = new ResourceIdentifier
            {
                Identifier = resourceIdentifier,
                SchemaIdentifier = this.SchemaIdentifier
            };
        }

        /// <inheritdoc />
        public IResourceIdentifier ResourceIdentifier { get; }
    }
}