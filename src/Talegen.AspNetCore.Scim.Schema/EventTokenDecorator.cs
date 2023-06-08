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
    /// Class EventTokenDecorator. Implements the <see cref="Talegen.AspNetCore.Scim.Schema.IEventToken" />
    /// </summary>
    /// <seealso cref="Talegen.AspNetCore.Scim.Schema.IEventToken" />
    public abstract class EventTokenDecorator : IEventToken
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventTokenDecorator" /> class.
        /// </summary>
        /// <param name="innerToken">The inner token.</param>
        /// <exception cref="System.ArgumentNullException">innerToken</exception>
        protected EventTokenDecorator(IEventToken innerToken)
        {
            this.InnerToken = innerToken ?? throw new ArgumentNullException(nameof(innerToken));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EventTokenDecorator" /> class.
        /// </summary>
        /// <param name="serialized">The serialized.</param>
        protected EventTokenDecorator(string serialized)
            : this(new EventToken(serialized))
        {
        }

        /// <summary>
        /// Gets or sets the audience.
        /// </summary>
        /// <value>The audience.</value>
        public IReadOnlyCollection<string> Audience
        {
            get
            {
                IReadOnlyCollection<string> result = this.InnerToken.Audience;
                return result;
            }

            set => this.InnerToken.Audience = value;
        }

        /// <summary>
        /// Gets the events.
        /// </summary>
        /// <value>The events.</value>
        public IDictionary<string, object> Events
        {
            get
            {
                IDictionary<string, object> results = this.InnerToken.Events;
                return results;
            }
        }

        /// <summary>
        /// Gets or sets the expiration.
        /// </summary>
        /// <value>The expiration.</value>
        public DateTime? Expiration
        {
            get
            {
                DateTime? result = this.InnerToken.Expiration;
                return result;
            }

            set => this.InnerToken.Expiration = value;
        }

        /// <summary>
        /// Gets the header.
        /// </summary>
        /// <value>The header.</value>
        public JwtHeader Header
        {
            get
            {
                JwtHeader result = this.InnerToken.Header;
                return result;
            }
        }

        /// <summary>
        /// Gets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        public string Identifier
        {
            get
            {
                string result = this.InnerToken.Identifier;
                return result;
            }
        }

        /// <summary>
        /// Gets the inner token.
        /// </summary>
        /// <value>The inner token.</value>
        public IEventToken InnerToken
        {
            get;
        }

        /// <summary>
        /// Gets the issued at.
        /// </summary>
        /// <value>The issued at.</value>
        public DateTime IssuedAt
        {
            get
            {
                DateTime result = this.InnerToken.IssuedAt;
                return result;
            }
        }

        /// <summary>
        /// Gets the issuer.
        /// </summary>
        /// <value>The issuer.</value>
        public string Issuer
        {
            get
            {
                string result = this.InnerToken.Issuer;
                return result;
            }
        }

        /// <summary>
        /// Gets the not before.
        /// </summary>
        /// <value>The not before.</value>
        public DateTime? NotBefore
        {
            get
            {
                DateTime? result = this.InnerToken.NotBefore;
                return result;
            }
        }

        /// <summary>
        /// Gets or sets the subject.
        /// </summary>
        /// <value>The subject.</value>
        /// <exception cref="System.NotImplementedException"></exception>
        public string Subject
        {
            get
            {
                string result = this.InnerToken.Subject;
                return result;
            }

            set => throw new NotImplementedException();
        }

        /// <summary>
        /// Gets or sets the transaction.
        /// </summary>
        /// <value>The transaction.</value>
        public string Transaction
        {
            get
            {
                string result = this.InnerToken.Transaction;
                return result;
            }

            set => this.InnerToken.Transaction = value;
        }
    }
}