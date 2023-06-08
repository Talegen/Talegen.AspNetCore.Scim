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
    /// Class EventTokenClaimTypes.
    /// </summary>
    public static class EventTokenClaimTypes
    {
        /// <summary>
        /// The audience
        /// </summary>
        public const string Audience = "aud";

        /// <summary>
        /// The expiration
        /// </summary>
        public const string Expiration = "exp";

        /// <summary>
        /// The events
        /// </summary>
        public const string Events = "events";

        /// <summary>
        /// The identifier
        /// </summary>
        public const string Identifier = "jti";

        /// <summary>
        /// The issued at
        /// </summary>
        public const string IssuedAt = "iat";

        /// <summary>
        /// The issuer
        /// </summary>
        public const string Issuer = "iss";

        /// <summary>
        /// The not before
        /// </summary>
        public const string NotBefore = "nbf";

        /// <summary>
        /// The subject
        /// </summary>
        public const string Subject = "sub";

        /// <summary>
        /// The transaction
        /// </summary>
        public const string Transaction = "txn";
    }
}