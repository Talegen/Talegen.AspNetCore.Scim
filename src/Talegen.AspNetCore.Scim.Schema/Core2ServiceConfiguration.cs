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
    using System.Runtime.Serialization;

    /// <summary>
    /// Class Core2ServiceConfiguration. This class cannot be inherited. Implements the <see cref="Talegen.AspNetCore.Scim.Schema.ServiceConfigurationBase" />
    /// </summary>
    /// <seealso cref="Talegen.AspNetCore.Scim.Schema.ServiceConfigurationBase" />
    [DataContract]
    public sealed class Core2ServiceConfiguration : ServiceConfigurationBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Core2ServiceConfiguration" /> class.
        /// </summary>
        /// <param name="bulkRequestsSupport">The bulk requests support.</param>
        /// <param name="supportsEntityTags">if set to <c>true</c> [supports entity tags].</param>
        /// <param name="supportsFiltering">if set to <c>true</c> [supports filtering].</param>
        /// <param name="supportsPasswordChange">if set to <c>true</c> [supports password change].</param>
        /// <param name="supportsPatching">if set to <c>true</c> [supports patching].</param>
        /// <param name="supportsSorting">if set to <c>true</c> [supports sorting].</param>
        public Core2ServiceConfiguration(
            BulkRequestsFeature bulkRequestsSupport,
            bool supportsEntityTags,
            bool supportsFiltering,
            bool supportsPasswordChange,
            bool supportsPatching,
            bool supportsSorting)
        {
            this.AddSchema(SchemaIdentifiers.Core2ServiceConfiguration);
            this.Metadata = new Core2Metadata
            {
                ResourceType = Types.ServiceProviderConfiguration
            };

            this.BulkRequests = bulkRequestsSupport;
            this.EntityTags = new Feature(supportsEntityTags);
            this.Filtering = new Feature(supportsFiltering);
            this.PasswordChange = new Feature(supportsPasswordChange);
            this.Patching = new Feature(supportsPatching);
            this.Sorting = new Feature(supportsSorting);
        }

        /// <summary>
        /// Gets or sets the metadata.
        /// </summary>
        /// <value>The metadata.</value>
        [DataMember(Name = AttributeNames.Metadata)]
        public Core2Metadata Metadata
        {
            get;
            set;
        }
    }
}