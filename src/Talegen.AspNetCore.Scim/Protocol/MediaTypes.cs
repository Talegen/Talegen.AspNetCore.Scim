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
    /// This class contains commonly defined media types.
    /// </summary>
    public static class MediaTypes
    {
        /// <summary>
        /// Contains the JWT media type.
        /// </summary>
        public const string JavaWebToken = "application/jwt";

        /// <summary>
        /// Contains the JSON media type.
        /// </summary>
        public const string Json = "application/json";

        /// <summary>
        /// Contains the SCIM Protocol media type.
        /// </summary>
        public const string Protocol = ProtocolConstants.ContentType;

        /// <summary>
        /// Contains a octet-stream media type.
        /// </summary>
        public const string Stream = "application/octet-stream";
    }
}