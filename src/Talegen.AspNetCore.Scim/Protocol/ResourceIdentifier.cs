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
    using System.Globalization;

    /// <summary>
    /// This class implements a resource identifier.
    /// </summary>
    public sealed class ResourceIdentifier : IResourceIdentifier
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceIdentifier" /> class.
        /// </summary>
        public ResourceIdentifier()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceIdentifier" /> class.
        /// </summary>
        /// <param name="schemaIdentifier">Contains the schema identifier.</param>
        /// <param name="resourceIdentifier">Contains the resource identifier.</param>
        /// <exception cref="ArgumentNullException">Exception is thrown if the schema or resource identifiers are not specified.</exception>
        public ResourceIdentifier(string schemaIdentifier, string resourceIdentifier)
        {
            if (string.IsNullOrWhiteSpace(schemaIdentifier))
            {
                throw new ArgumentNullException(nameof(schemaIdentifier));
            }

            if (string.IsNullOrWhiteSpace(resourceIdentifier))
            {
                throw new ArgumentNullException(nameof(resourceIdentifier));
            }

            this.SchemaIdentifier = schemaIdentifier;
            this.Identifier = resourceIdentifier;
        }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        public string Identifier { get; set; }

        /// <summary>
        /// Gets or sets the schema identifier.
        /// </summary>
        public string SchemaIdentifier { get; set; }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            bool result = false;

            if (obj != null)
            {
                IResourceIdentifier otherIdentifier = obj as IResourceIdentifier;

                if (otherIdentifier != null &&
                    (string.Equals(this.SchemaIdentifier, otherIdentifier.SchemaIdentifier, StringComparison.OrdinalIgnoreCase) ||
                     string.Equals(this.Identifier, otherIdentifier.Identifier, StringComparison.OrdinalIgnoreCase)))
                {
                    result = true;
                }
            }

            return result;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            int identifierCode = string.IsNullOrWhiteSpace(this.Identifier) ? 0 : this.Identifier.GetHashCode(StringComparison.InvariantCulture);
            int schemaIdentifierCode = string.IsNullOrWhiteSpace(this.SchemaIdentifier) ? 0 : this.SchemaIdentifier.GetHashCode(StringComparison.InvariantCulture);
            int result = identifierCode ^ schemaIdentifierCode;
            return result;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, Schema.Properties.Resources.ResourceIdentifierTemplate, this.SchemaIdentifier, this.Identifier);
        }
    }
}