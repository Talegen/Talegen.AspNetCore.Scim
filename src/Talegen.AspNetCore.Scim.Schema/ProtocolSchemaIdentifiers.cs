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
    /// Class ProtocolSchemaIdentifiers.
    /// </summary>
    public static class ProtocolSchemaIdentifiers
    {
        /// <summary>
        /// The error
        /// </summary>
        private const string Error = "Error";

        /// <summary>
        /// The operation patch
        /// </summary>
        private const string OperationPatch = "PatchOp";

        /// <summary>
        /// The version messages2
        /// </summary>
        private const string VersionMessages2 = "2.0:";

        /// <summary>
        /// The prefix messages
        /// </summary>
        private const string PrefixMessages = "urn:ietf:params:scim:api:messages:";

        /// <summary>
        /// The prefix messages2
        /// </summary>
        public const string PrefixMessages2 = PrefixMessages + VersionMessages2;

        /// <summary>
        /// The response list
        /// </summary>
        private const string ResponseList = "ListResponse";

        /// <summary>
        /// The request bulk
        /// </summary>
        private const string RequestBulk = "BulkRequest";

        /// <summary>
        /// The response bulk
        /// </summary>
        private const string ResponseBulk = "BulkResponse";

        /// <summary>
        /// The version2 error
        /// </summary>
        public const string Version2Error =
            PrefixMessages2 + Error;

        /// <summary>
        /// The version2 list response
        /// </summary>
        public const string Version2ListResponse =
            PrefixMessages2 + ResponseList;

        /// <summary>
        /// The version2 patch operation
        /// </summary>
        public const string Version2PatchOperation =
            PrefixMessages2 + OperationPatch;

        /// <summary>
        /// The version2 bulk request
        /// </summary>
        public const string Version2BulkRequest =
             PrefixMessages2 + RequestBulk;

        /// <summary>
        /// The version2 bulk response
        /// </summary>
        public const string Version2BulkResponse =
            PrefixMessages2 + ResponseBulk;
    }
}