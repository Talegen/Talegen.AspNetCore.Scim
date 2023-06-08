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
    using System.Collections.Generic;
    using System.IdentityModel.Tokens.Jwt;

    /// <summary>
    /// Interface IEventToken
    /// </summary>
    public interface IEventToken
    {
        /// <summary>
        /// Gets or sets the audience.
        /// </summary>
        /// <value>The audience.</value>
        IReadOnlyCollection<string> Audience { get; set; }

        /// <summary>
        /// Gets the events.
        /// </summary>
        /// <value>The events.</value>
        IDictionary<string, object> Events { get; }

        /// <summary>
        /// Gets or sets the expiration.
        /// </summary>
        /// <value>The expiration.</value>
        DateTime? Expiration { get; set; }

        /// <summary>
        /// Gets the header.
        /// </summary>
        /// <value>The header.</value>
        JwtHeader Header { get; }

        /// <summary>
        /// Gets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        string Identifier { get; }

        /// <summary>
        /// Gets the issued at.
        /// </summary>
        /// <value>The issued at.</value>
        DateTime IssuedAt { get; }

        /// <summary>
        /// Gets the issuer.
        /// </summary>
        /// <value>The issuer.</value>
        string Issuer { get; }

        /// <summary>
        /// Gets the not before.
        /// </summary>
        /// <value>The not before.</value>
        DateTime? NotBefore { get; }

        /// <summary>
        /// Gets or sets the subject.
        /// </summary>
        /// <value>The subject.</value>
        string Subject { get; set; }

        /// <summary>
        /// Gets or sets the transaction.
        /// </summary>
        /// <value>The transaction.</value>
        string Transaction { get; set; }
    }
}