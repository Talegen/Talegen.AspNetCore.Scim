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

namespace Talegen.AspNetCore.Scim.Schema
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Runtime.Serialization;

    /// <summary>
    /// Class QualifiedResource. Implements the <see cref="Talegen.AspNetCore.Scim.Schema.Resource" />
    /// </summary>
    /// <seealso cref="Talegen.AspNetCore.Scim.Schema.Resource" />
    [DataContract]
    public abstract class QualifiedResource : Resource
    {
        /// <summary>
        /// The resource schema identifier template suffix
        /// </summary>
        private const string ResourceSchemaIdentifierTemplateSuffix = "{0}";

        /// <summary>
        /// The resource schema identifier template
        /// </summary>
        private string resourceSchemaIdentifierTemplate;

        /// <summary>
        /// Initializes a new instance of the <see cref="QualifiedResource" /> class.
        /// </summary>
        /// <param name="schemaIdentifier">The schema identifier.</param>
        /// <param name="resourceSchemaPrefix">The resource schema prefix.</param>
        protected QualifiedResource(string schemaIdentifier, string resourceSchemaPrefix)
        {
            this.OnInitialized(schemaIdentifier, resourceSchemaPrefix);
        }

        /// <summary>
        /// Gets or sets the resource schema prefix.
        /// </summary>
        /// <value>The resource schema prefix.</value>
        private string ResourceSchemaPrefix
        {
            get;
            set;
        }

        /// <summary>
        /// Adds the resource schema identifier.
        /// </summary>
        /// <param name="resourceTypeName">Name of the resource type.</param>
        /// <exception cref="System.InvalidOperationException"></exception>
        public virtual void AddResourceSchemaIdentifier(string resourceTypeName)
        {
            if (this.TryGetResourceTypeName(out string value))
            {
                string typeName = this.GetType().Name;
                string errorMessage =
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Properties.Resources.ExceptionMultipleQualifiedResourceTypeIdentifiersTemplate,
                        typeName);
                throw new InvalidOperationException(errorMessage);
            }
            string schemaIdentifier =
                string.Format(
                    CultureInfo.InvariantCulture,
                    this.resourceSchemaIdentifierTemplate,
                    resourceTypeName);
            this.AddSchema(schemaIdentifier);
        }

        /// <summary>
        /// Called when [deserialized].
        /// </summary>
        /// <param name="schemaIdentifier">The schema identifier.</param>
        /// <param name="resourceSchemaPrefix">The resource schema prefix.</param>
        /// <exception cref="System.InvalidOperationException"></exception>
        public void OnDeserialized(string schemaIdentifier, string resourceSchemaPrefix)
        {
            this.OnInitialized(schemaIdentifier, resourceSchemaPrefix);
            int countResourceSchemaIdentifiers =
                this
                .Schemas
                .Where(
                    (string item) =>
                        item.StartsWith(this.ResourceSchemaPrefix, StringComparison.Ordinal))
                .Count();
            if (countResourceSchemaIdentifiers > 1)
            {
                string typeName = this.GetType().Name;
                string errorMessage =
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Properties.Resources.ExceptionMultipleQualifiedResourceTypeIdentifiersTemplate,
                        typeName);
                throw new InvalidOperationException(errorMessage);
            }
        }

        /// <summary>
        /// Called when [initialized].
        /// </summary>
        /// <param name="schemaIdentifier">The schema identifier.</param>
        /// <param name="resourceSchemaPrefix">The resource schema prefix.</param>
        /// <exception cref="System.ArgumentNullException">schemaIdentifier</exception>
        /// <exception cref="System.ArgumentNullException">resourceSchemaPrefix</exception>
        private void OnInitialized(string schemaIdentifier, string resourceSchemaPrefix)
        {
            if (string.IsNullOrWhiteSpace(schemaIdentifier))
            {
                throw new ArgumentNullException(nameof(schemaIdentifier));
            }

            if (string.IsNullOrWhiteSpace(resourceSchemaPrefix))
            {
                throw new ArgumentNullException(nameof(resourceSchemaPrefix));
            }

            this.ResourceSchemaPrefix = resourceSchemaPrefix;
            this.resourceSchemaIdentifierTemplate =
                this.ResourceSchemaPrefix + ResourceSchemaIdentifierTemplateSuffix;
        }

        /// <summary>
        /// Tries the name of the get resource type.
        /// </summary>
        /// <param name="resourceTypeName">Name of the resource type.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public virtual bool TryGetResourceTypeName(out string resourceTypeName)
        {
            resourceTypeName = null;

            string resourceSchemaIdentifier =
                this
                .Schemas
                .SingleOrDefault(
                    (string item) =>
                        item.StartsWith(this.ResourceSchemaPrefix, StringComparison.Ordinal));
            if (string.IsNullOrWhiteSpace(resourceSchemaIdentifier))
            {
                return false;
            }
            string buffer = resourceSchemaIdentifier.Substring(this.ResourceSchemaPrefix.Length);
            if (buffer.Length <= 0)
            {
                return false;
            }
            resourceTypeName = buffer;
            return true;
        }
    }
}