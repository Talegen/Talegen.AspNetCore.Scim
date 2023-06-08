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
    /// Class InstantMessagingBase. Implements the <see cref="Talegen.AspNetCore.Scim.Schema.TypedValue" />
    /// </summary>
    /// <seealso cref="Talegen.AspNetCore.Scim.Schema.TypedValue" />
    [DataContract]
    public abstract class InstantMessagingBase : TypedValue
    {
        /// <summary>
        /// The aim
        /// </summary>
        public const string Aim = "aim";

        /// <summary>
        /// The gtalk
        /// </summary>
        public const string Gtalk = "gtalk";

        /// <summary>
        /// The icq
        /// </summary>
        public const string Icq = "icq";

        /// <summary>
        /// The MSN
        /// </summary>
        public const string Msn = "msn";

        /// <summary>
        /// The qq
        /// </summary>
        public const string Qq = "qq";

        /// <summary>
        /// The skype
        /// </summary>
        public const string Skype = "skype";

        /// <summary>
        /// The XMPP
        /// </summary>
        public const string Xmpp = "xmpp";

        /// <summary>
        /// The yahoo
        /// </summary>
        public const string Yahoo = "yahoo";

        /// <summary>
        /// Initializes a new instance of the <see cref="InstantMessagingBase" /> class.
        /// </summary>
        internal InstantMessagingBase()
        {
        }
    }
}