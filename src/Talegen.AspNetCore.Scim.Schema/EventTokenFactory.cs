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
    /// Class EventTokenFactory.
    /// </summary>
    public abstract class EventTokenFactory
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventTokenFactory" /> class.
        /// </summary>
        /// <param name="issuer">The issuer.</param>
        /// <param name="header">The header.</param>
        /// <exception cref="System.ArgumentNullException">issuer</exception>
        /// <exception cref="System.ArgumentNullException">header</exception>
        protected EventTokenFactory(string issuer, JwtHeader header)
        {
            if (string.IsNullOrWhiteSpace(issuer))
            {
                throw new ArgumentNullException(nameof(issuer));
            }

            this.Issuer = issuer;
            this.Header = header ?? throw new ArgumentNullException(nameof(header));
        }

        /// <summary>
        /// Gets the header.
        /// </summary>
        /// <value>The header.</value>
        public JwtHeader Header
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the issuer.
        /// </summary>
        /// <value>The issuer.</value>
        public string Issuer
        {
            get;
            private set;
        }

        /// <summary>
        /// Creates the specified events.
        /// </summary>
        /// <param name="events">The events.</param>
        /// <returns>IEventToken.</returns>
        public abstract IEventToken Create(IDictionary<string, object> events);
    }
}