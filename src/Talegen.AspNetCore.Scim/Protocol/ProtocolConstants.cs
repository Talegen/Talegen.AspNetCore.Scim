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
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using Schema;

    /// <summary>
    /// This class contains SCIM protocol values.
    /// </summary>
    public static class ProtocolConstants
    {
        /// <summary>
        /// SCIM Application content type.
        /// </summary>
        public const string ContentType = "application/scim+json";

        /// <summary>
        /// Path Groups.
        /// </summary>
        public const string PathGroups = "Groups";

        /// <summary>
        /// Path Users.
        /// </summary>
        public const string PathUsers = "Users";

        /// <summary>
        /// Path Bulk operations.
        /// </summary>
        public const string PathBulk = "Bulk";

        /// <summary>
        /// Path with Batch interface.
        /// </summary>
        public const string PathWebBatchInterface = SchemaConstants.PathInterface + "/batch";

        /// <summary>
        /// JSON serialization settings
        /// </summary>
        public static readonly Lazy<JsonSerializerSettings> JsonSettings = new Lazy<JsonSerializerSettings>(() => ProtocolConstants.InitializeSettings());

        /// <summary>
        /// This method is used to initialize JSON serialization settings.
        /// </summary>
        /// <returns></returns>
        private static JsonSerializerSettings InitializeSettings()
        {
            JsonSerializerSettings result = new JsonSerializerSettings
            {
                Error = delegate (object sender, ErrorEventArgs args) { args.ErrorContext.Handled = true; }
            };

            return result;
        }
    }
}