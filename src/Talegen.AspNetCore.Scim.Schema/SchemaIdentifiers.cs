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
    /// <summary>
    /// This class contains the constants of SCIM schema identifiers.
    /// </summary>
    public static class SchemaIdentifiers
    {
        /// <summary>
        /// The extension
        /// </summary>
        public const string Extension = "extension:";

        /// <summary>
        /// The extension enterprise2
        /// </summary>
        private const string ExtensionEnterprise2 = Extension + "enterprise:2.0:";

        /// <summary>
        /// The none
        /// </summary>
        public const string None = "/";

        /// <summary>
        /// The prefix types1
        /// </summary>
        public const string PrefixTypes1 = "urn:scim:schemas:";

        /// <summary>
        /// The prefix types2
        /// </summary>
        private const string PrefixTypes2 = "urn:ietf:params:scim:schemas:";

        /// <summary>
        /// The version schemas core2
        /// </summary>
        private const string VersionSchemasCore2 = "core:2.0:";

        /// <summary>
        /// The core2 enterprise user
        /// </summary>
        public const string Core2EnterpriseUser =
            PrefixTypes2 +
            ExtensionEnterprise2 +
            Types.User;

        /// <summary>
        /// The core2 group
        /// </summary>
        public const string Core2Group =
            PrefixTypes2 +
            VersionSchemasCore2 +
            Types.Group;

        /// <summary>
        /// The core2 resource type
        /// </summary>
        public const string Core2ResourceType =
            PrefixTypes2 +
            ExtensionEnterprise2 +
            Types.ResourceType;

        /// <summary>
        /// The core2 service configuration
        /// </summary>
        public const string Core2ServiceConfiguration =
            PrefixTypes2 +
            VersionSchemasCore2 +
            Types.ServiceProviderConfiguration;

        /// <summary>
        /// The core2 user
        /// </summary>
        public const string Core2User =
            PrefixTypes2 +
            VersionSchemasCore2 +
            Types.User;

        /// <summary>
        /// The core2 schema
        /// </summary>
        public const string Core2Schema =
            PrefixTypes2 +
            VersionSchemasCore2 +
            Types.Schema;

        /// <summary>
        /// The prefix extension
        /// </summary>
        public const string PrefixExtension =
            PrefixTypes2 +
            Extension;
    }
}