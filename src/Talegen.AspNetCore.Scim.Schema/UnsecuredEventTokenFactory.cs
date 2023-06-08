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
    using System.IdentityModel.Tokens.Jwt;

    /// <summary>
    /// Class UnsecuredEventTokenFactory. Implements the <see cref="Talegen.AspNetCore.Scim.Schema.EventTokenFactory" />
    /// </summary>
    /// <seealso cref="Talegen.AspNetCore.Scim.Schema.EventTokenFactory" />
    public abstract class UnsecuredEventTokenFactory : EventTokenFactory
    {
        /// <summary>
        /// The unsecured token header
        /// </summary>
        private static readonly Lazy<JwtHeader> UnsecuredTokenHeader = new(ComposeHeader);

        /// <summary>
        /// Initializes a new instance of the <see cref="UnsecuredEventTokenFactory" /> class.
        /// </summary>
        /// <param name="issuer">The issuer.</param>
        protected UnsecuredEventTokenFactory(string issuer)
            : base(issuer, UnsecuredTokenHeader.Value)
        {
        }

        /// <summary>
        /// Composes the header.
        /// </summary>
        /// <returns>JwtHeader.</returns>
        private static JwtHeader ComposeHeader()
        {
            JwtHeader result = new JwtHeader();
            result.Add(EventToken.HeaderKeyAlgorithm, EventToken.JwtAlgorithmNone);
            return result;
        }
    }
}