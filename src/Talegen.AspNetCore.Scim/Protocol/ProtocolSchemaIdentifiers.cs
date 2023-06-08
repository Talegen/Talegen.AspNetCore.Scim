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
    /// <summary>
    /// This class contains SCIM protocol schema identifiers.
    /// </summary>
    public static class ProtocolSchemaIdentifiers
    {
        /// <summary>
        /// Error.
        /// </summary>
        private const string Error = "Error";

        /// <summary>
        /// Patch Operation.
        /// </summary>
        private const string OperationPatch = "PatchOp";

        /// <summary>
        /// Version.
        /// </summary>
        private const string VersionMessages2 = "2.0:";

        /// <summary>
        /// Prefix messages.
        /// </summary>
        private const string PrefixMessages = "urn:ietf:params:scim:api:messages:";

        /// <summary>
        /// Prefix messages SCIM v2.
        /// </summary>
        public const string PrefixMessages2 = ProtocolSchemaIdentifiers.PrefixMessages + ProtocolSchemaIdentifiers.VersionMessages2;

        /// <summary>
        /// Response list.
        /// </summary>
        private const string ResponseList = "ListResponse";

        /// <summary>
        /// Bulk request.
        /// </summary>
        private const string RequestBulk = "BulkRequest";

        /// <summary>
        /// Bulk response.
        /// </summary>
        private const string ResponseBulk = "BulkResponse";

        /// <summary>
        /// SCIM v2 Error.
        /// </summary>
        public const string Version2Error = ProtocolSchemaIdentifiers.PrefixMessages2 + ProtocolSchemaIdentifiers.Error;

        /// <summary>
        /// SCIM v2 List Response.
        /// </summary>
        public const string Version2ListResponse = ProtocolSchemaIdentifiers.PrefixMessages2 + ProtocolSchemaIdentifiers.ResponseList;

        /// <summary>
        /// SCIM v2 Patch Operation.
        /// </summary>
        public const string Version2PatchOperation = ProtocolSchemaIdentifiers.PrefixMessages2 + ProtocolSchemaIdentifiers.OperationPatch;

        /// <summary>
        /// SCIM v2 Bulk Request.
        /// </summary>
        public const string Version2BulkRequest = ProtocolSchemaIdentifiers.PrefixMessages2 + ProtocolSchemaIdentifiers.RequestBulk;

        /// <summary>
        /// SCIM v2 Bulk response.
        /// </summary>
        public const string Version2BulkResponse = ProtocolSchemaIdentifiers.PrefixMessages2 + ProtocolSchemaIdentifiers.ResponseBulk;
    }
}