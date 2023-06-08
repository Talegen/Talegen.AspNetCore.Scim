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
    using Schema;

    /// <summary>
    /// This class contains the supported SCIM protocol attribute names.
    /// </summary>
    public static class ProtocolAttributeNames
    {
        /// <summary>
        /// Bulk operation identifier.
        /// </summary>
        public const string BulkOperationIdentifier = "bulkId";

        /// <summary>
        /// Count.
        /// </summary>
        public const string Count = "count";

        /// <summary>
        /// Data.
        /// </summary>
        public const string Data = "data";

        /// <summary>
        /// Detail.
        /// </summary>
        public const string Detail = "detail";

        /// <summary>
        /// SCIM error type.
        /// </summary>
        public const string ErrorType = "scimType";

        /// <summary>
        /// Excluded attributes.
        /// </summary>
        public const string ExcludedAttributes = "excludedAttributes";

        /// <summary>
        /// Fail on errors.
        /// </summary>
        public const string FailOnErrors = "failOnErrors";

        /// <summary>
        /// Items per page.
        /// </summary>
        public const string ItemsPerPage = "itemsPerPage";

        /// <summary>
        /// Location.
        /// </summary>
        public const string Location = "location";

        /// <summary>
        /// Method.
        /// </summary>
        public const string Method = "method";

        /// <summary>
        /// Operations.
        /// </summary>
        public const string Operations = "Operations";

        /// <summary>
        /// SCIM v1 Operation.
        /// </summary>
        public const string Patch1Operation = "operation";

        /// <summary>
        /// SCIM v2 Operation.
        /// </summary>
        public const string Patch2Operation = "op";

        /// <summary>
        /// Path.
        /// </summary>
        public const string Path = AttributeNames.Path;

        /// <summary>
        /// Reference.
        /// </summary>
        public const string Reference = "$ref";

        /// <summary>
        /// Resources.
        /// </summary>
        public const string Resources = "Resources";

        /// <summary>
        /// Response.
        /// </summary>
        public const string Response = "response";

        /// <summary>
        /// Schemas.
        /// </summary>
        public const string Schemas = "schemas";

        /// <summary>
        /// Sort by.
        /// </summary>
        public const string SortBy = "sortBy";

        /// <summary>
        /// Sort order.
        /// </summary>
        public const string SortOrder = "sortOrder";

        /// <summary>
        /// Start index.
        /// </summary>
        public const string StartIndex = "startIndex";

        /// <summary>
        /// Status.
        /// </summary>
        public const string Status = "status";

        /// <summary>
        /// Total results.
        /// </summary>
        public const string TotalResults = "totalResults";
    }
}