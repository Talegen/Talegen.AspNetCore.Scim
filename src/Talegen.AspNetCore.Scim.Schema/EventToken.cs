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
    using System.Globalization;
    using System.IdentityModel.Tokens.Jwt;
    using System.Linq;
    using Microsoft.IdentityModel.Tokens;

    // Implements https://tools.ietf.org/html/draft-ietf-secevent-token
    /// <summary>
    /// Class EventToken. Implements the <see cref="Talegen.AspNetCore.Scim.Schema.IEventToken" />
    /// </summary>
    /// <seealso cref="Talegen.AspNetCore.Scim.Schema.IEventToken" />
    public class EventToken : IEventToken
    {
        /// <summary>
        /// The header key algorithm
        /// </summary>
        public const string HeaderKeyAlgorithm = "alg";

        /// <summary>
        /// The JWT algorithm none
        /// </summary>
        public const string JwtAlgorithmNone = "none";

        /// <summary>
        /// The header default
        /// </summary>
        private static readonly Lazy<JwtHeader> HeaderDefault = new Lazy<JwtHeader>(() => ComposeDefaultHeader());

        /// <summary>
        /// The token serializer
        /// </summary>
        private static readonly Lazy<SecurityTokenHandler> TokenSerializer = new Lazy<SecurityTokenHandler>(() => new JwtSecurityTokenHandler());

        /// <summary>
        /// Initializes a new instance of the <see cref="EventToken" /> class.
        /// </summary>
        /// <param name="issuer">The issuer.</param>
        /// <param name="header">The header.</param>
        /// <exception cref="System.ArgumentNullException">issuer</exception>
        /// <exception cref="System.ArgumentNullException">header</exception>
        private EventToken(string issuer, JwtHeader header)
        {
            if (string.IsNullOrWhiteSpace(issuer))
            {
                throw new ArgumentNullException(nameof(issuer));
            }

            this.Issuer = issuer;
            this.Header = header ?? throw new ArgumentNullException(nameof(header));

            this.Identifier = Guid.NewGuid().ToString();
            this.IssuedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EventToken" /> class.
        /// </summary>
        /// <param name="issuer">The issuer.</param>
        /// <param name="header">The header.</param>
        /// <param name="events">The events.</param>
        /// <exception cref="System.ArgumentNullException">events</exception>
        public EventToken(string issuer, JwtHeader header, IDictionary<string, object> events)
            : this(issuer, header)
        {
            this.Events = events ?? throw new ArgumentNullException(nameof(events));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EventToken" /> class.
        /// </summary>
        /// <param name="issuer">The issuer.</param>
        /// <param name="events">The events.</param>
        public EventToken(string issuer, Dictionary<string, object> events)
            : this(issuer, HeaderDefault.Value, events)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EventToken" /> class.
        /// </summary>
        /// <param name="serialized">The serialized.</param>
        /// <exception cref="System.ArgumentNullException">serialized</exception>
        public EventToken(string serialized)
        {
            if (string.IsNullOrWhiteSpace(serialized))
            {
                throw new ArgumentNullException(nameof(serialized));
            }

            JwtSecurityToken token = new JwtSecurityToken(serialized);
            this.Header = token.Header;

            this.ParseIdentifier(token.Payload);
            this.ParseIssuer(token.Payload);
            this.ParseAudience(token.Payload);
            this.ParseIssuedAt(token.Payload);
            this.ParseNotBefore(token.Payload);
            this.ParseSubject(token.Payload);
            this.ParseExpiration(token.Payload);
            this.ParseEvents(token.Payload);
            this.ParseTransaction(token.Payload);
        }

        /// <summary>
        /// Gets or sets the audience.
        /// </summary>
        /// <value>The audience.</value>
        public IReadOnlyCollection<string> Audience
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the events.
        /// </summary>
        /// <value>The events.</value>
        public IDictionary<string, object> Events
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the expiration.
        /// </summary>
        /// <value>The expiration.</value>
        public DateTime? Expiration
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the header.
        /// </summary>
        /// <value>The header.</value>
        public JwtHeader Header
        {
            get;
        }

        /// <summary>
        /// Gets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        public string Identifier
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the issued at.
        /// </summary>
        /// <value>The issued at.</value>
        public DateTime IssuedAt
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
        /// Gets the not before.
        /// </summary>
        /// <value>The not before.</value>
        public DateTime? NotBefore
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the subject.
        /// </summary>
        /// <value>The subject.</value>
        public string Subject
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the transaction.
        /// </summary>
        /// <value>The transaction.</value>
        public string Transaction
        {
            get;
            set;
        }

        /// <summary>
        /// Composes the default header.
        /// </summary>
        /// <returns>JwtHeader.</returns>
        private static JwtHeader ComposeDefaultHeader()
        {
            JwtHeader result = new JwtHeader { { HeaderKeyAlgorithm, JwtAlgorithmNone } };
            return result;
        }

        /// <summary>
        /// Parses the audience.
        /// </summary>
        /// <param name="payload">The payload.</param>
        /// <exception cref="System.ArgumentNullException">payload</exception>
        /// <exception cref="System.ArgumentException"></exception>
        private void ParseAudience(JwtPayload payload)
        {
            if (null == payload)
            {
                throw new ArgumentNullException(nameof(payload));
            }

            if (!payload.TryGetValue(EventTokenClaimTypes.Audience, out object value) || null == value)
            {
                return;
            }

            object[] values = value as object[];
            if (null == values)
            {
                string exceptionMessage =
                string.Format(
                    CultureInfo.InvariantCulture,
                    Properties.Resources.ExceptionEventTokenInvalidClaimValueTemplate,
                    EventTokenClaimTypes.Audience,
                    value);
                throw new ArgumentException(exceptionMessage);
            }

            IReadOnlyCollection<string> audience =
                values
                .OfType<string>()
                .ToArray();
            if (audience.Count != values.Length)
            {
                string exceptionMessage =
                string.Format(
                    CultureInfo.InvariantCulture,
                    Properties.Resources.ExceptionEventTokenInvalidClaimValueTemplate,
                    EventTokenClaimTypes.Audience,
                    value);
                throw new ArgumentException(exceptionMessage);
            }

            this.Audience = audience;
        }

        /// <summary>
        /// Parses the events.
        /// </summary>
        /// <param name="payload">The payload.</param>
        /// <exception cref="System.ArgumentNullException">payload</exception>
        /// <exception cref="System.ArgumentException"></exception>
        private void ParseEvents(JwtPayload payload)
        {
            if (null == payload)
            {
                throw new ArgumentNullException(nameof(payload));
            }

            if (!payload.TryGetValue(EventTokenClaimTypes.Events, out object value) || null == value)
            {
                string exceptionMessage =
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Properties.Resources.ExceptionEventTokenMissingClaimTemplate,
                        EventTokenClaimTypes.Events);
                throw new ArgumentException(exceptionMessage);
            }

            IDictionary<string, object> events = value as Dictionary<string, object>;
            if (null == events)
            {
                string exceptionMessage =
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Properties.Resources.ExceptionEventTokenInvalidClaimValueTemplate,
                        EventTokenClaimTypes.Events,
                        value);
                throw new ArgumentException(exceptionMessage);
            }
            this.Events = events;
        }

        /// <summary>
        /// Parses the expiration.
        /// </summary>
        /// <param name="payload">The payload.</param>
        /// <exception cref="System.ArgumentNullException">payload</exception>
        /// <exception cref="System.ArgumentException"></exception>
        /// <exception cref="Microsoft.IdentityModel.Tokens.SecurityTokenExpiredException"></exception>
        private void ParseExpiration(JwtPayload payload)
        {
            if (null == payload)
            {
                throw new ArgumentNullException(nameof(payload));
            }

            if (!payload.TryGetValue(EventTokenClaimTypes.Expiration, out object value) || null == value)
            {
                return;
            }

            string serializedValue = value.ToString();
            if (!long.TryParse(serializedValue, out long expiration))
            {
                string exceptionMessage =
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Properties.Resources.ExceptionEventTokenInvalidClaimValueTemplate,
                        EventTokenClaimTypes.Expiration,
                        value);
                throw new ArgumentException(exceptionMessage);
            }

            this.Expiration = new UnixTime(expiration).ToUniversalTime();
            if (this.Expiration > DateTime.UtcNow)
            {
                throw new SecurityTokenExpiredException(Properties.Resources.ExceptionEventTokenExpired);
            }
        }

        /// <summary>
        /// Parses the identifier.
        /// </summary>
        /// <param name="payload">The payload.</param>
        /// <exception cref="System.ArgumentNullException">payload</exception>
        /// <exception cref="System.ArgumentException"></exception>
        private void ParseIdentifier(JwtPayload payload)
        {
            if (null == payload)
            {
                throw new ArgumentNullException(nameof(payload));
            }

            if (!payload.TryGetValue(EventTokenClaimTypes.Identifier, out object value) || null == value)
            {
                string exceptionMessage =
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Properties.Resources.ExceptionEventTokenMissingClaimTemplate,
                        EventTokenClaimTypes.Identifier);
                throw new ArgumentException(exceptionMessage);
            }

            string identifier = value as string;
            if (string.IsNullOrWhiteSpace(identifier))
            {
                string exceptionMessage =
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Properties.Resources.ExceptionEventTokenInvalidClaimValueTemplate,
                        EventTokenClaimTypes.Identifier,
                        value);
                throw new ArgumentException(exceptionMessage);
            }
            this.Identifier = identifier;
        }

        /// <summary>
        /// Parses the issued at.
        /// </summary>
        /// <param name="payload">The payload.</param>
        /// <exception cref="System.ArgumentNullException">payload</exception>
        /// <exception cref="System.ArgumentException"></exception>
        private void ParseIssuedAt(JwtPayload payload)
        {
            if (null == payload)
            {
                throw new ArgumentNullException(nameof(payload));
            }

            if (!payload.TryGetValue(EventTokenClaimTypes.IssuedAt, out object value) || null == value)
            {
                string exceptionMessage =
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Properties.Resources.ExceptionEventTokenMissingClaimTemplate,
                        EventTokenClaimTypes.IssuedAt);
                throw new ArgumentException(exceptionMessage);
            }

            string serializedValue = value.ToString();
            if (!long.TryParse(serializedValue, out long issuedAt))
            {
                string exceptionMessage =
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Properties.Resources.ExceptionEventTokenMissingClaimTemplate,
                        EventTokenClaimTypes.IssuedAt);
                throw new ArgumentException(exceptionMessage);
            }
            this.IssuedAt = new UnixTime(issuedAt).ToUniversalTime();
        }

        /// <summary>
        /// Parses the issuer.
        /// </summary>
        /// <param name="payload">The payload.</param>
        /// <exception cref="System.ArgumentNullException">payload</exception>
        /// <exception cref="System.ArgumentException"></exception>
        private void ParseIssuer(JwtPayload payload)
        {
            if (null == payload)
            {
                throw new ArgumentNullException(nameof(payload));
            }

            if (!payload.TryGetValue(EventTokenClaimTypes.Issuer, out object value) || null == value)
            {
                string exceptionMessage =
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Properties.Resources.ExceptionEventTokenMissingClaimTemplate,
                        EventTokenClaimTypes.Issuer);
                throw new ArgumentException(exceptionMessage);
            }

            string issuer = value as string;
            if (string.IsNullOrWhiteSpace(issuer))
            {
                string exceptionMessage =
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Properties.Resources.ExceptionEventTokenInvalidClaimValueTemplate,
                        EventTokenClaimTypes.Issuer,
                        value);
                throw new ArgumentException(exceptionMessage);
            }
            this.Issuer = issuer;
        }

        /// <summary>
        /// Parses the not before.
        /// </summary>
        /// <param name="payload">The payload.</param>
        /// <exception cref="System.ArgumentNullException">payload</exception>
        /// <exception cref="System.ArgumentException"></exception>
        private void ParseNotBefore(JwtPayload payload)
        {
            if (null == payload)
            {
                throw new ArgumentNullException(nameof(payload));
            }

            if (!payload.TryGetValue(EventTokenClaimTypes.NotBefore, out object value) || null == value)
            {
                return;
            }

            string serializedValue = value.ToString();
            if (!long.TryParse(serializedValue, out long notBefore))
            {
                string exceptionMessage =
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Properties.Resources.ExceptionEventTokenInvalidClaimValueTemplate,
                        EventTokenClaimTypes.NotBefore,
                        value);
                throw new ArgumentException(exceptionMessage);
            }

            this.NotBefore = new UnixTime(notBefore).ToUniversalTime();
        }

        /// <summary>
        /// Parses the subject.
        /// </summary>
        /// <param name="payload">The payload.</param>
        /// <exception cref="System.ArgumentNullException">payload</exception>
        /// <exception cref="System.ArgumentException"></exception>
        private void ParseSubject(JwtPayload payload)
        {
            if (null == payload)
            {
                throw new ArgumentNullException(nameof(payload));
            }

            if (!payload.TryGetValue(EventTokenClaimTypes.Subject, out object value) || null == value)
            {
                return;
            }

            string subject = value as string;
            if (null == subject)
            {
                string exceptionMessage =
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Properties.Resources.ExceptionEventTokenInvalidClaimValueTemplate,
                        EventTokenClaimTypes.Subject,
                        value);
                throw new ArgumentException(exceptionMessage);
            }

            this.Subject = subject;
        }

        /// <summary>
        /// Parses the transaction.
        /// </summary>
        /// <param name="payload">The payload.</param>
        /// <exception cref="System.ArgumentNullException">payload</exception>
        /// <exception cref="System.ArgumentException"></exception>
        private void ParseTransaction(JwtPayload payload)
        {
            if (null == payload)
            {
                throw new ArgumentNullException(nameof(payload));
            }

            if (!payload.TryGetValue(EventTokenClaimTypes.Transaction, out object value) || null == value)
            {
                return;
            }

            string transaction = value as string;
            if (null == transaction)
            {
                string exceptionMessage =
                string.Format(
                    CultureInfo.InvariantCulture,
                    Properties.Resources.ExceptionEventTokenInvalidClaimValueTemplate,
                    EventTokenClaimTypes.Transaction,
                    value);
                throw new ArgumentException(exceptionMessage);
            }

            this.Transaction = transaction;
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            JwtPayload payload = new JwtPayload
            {
                { EventTokenClaimTypes.Identifier, this.Identifier },
                { EventTokenClaimTypes.Issuer, this.Issuer }
            };

            if (this.Audience.Any())
            {
                string[] audience = this.Audience.ToArray();
                payload.Add(EventTokenClaimTypes.Audience, audience);
            }

            long issuedAt = new UnixTime(this.IssuedAt).EpochTimestamp;
            payload.Add(EventTokenClaimTypes.IssuedAt, issuedAt);

            if (this.NotBefore.HasValue)
            {
                long notBefore = new UnixTime(this.NotBefore.Value).EpochTimestamp;
                payload.Add(EventTokenClaimTypes.NotBefore, notBefore);
            }

            if (!string.IsNullOrWhiteSpace(this.Subject))
            {
                payload.Add(EventTokenClaimTypes.Subject, this.Subject);
            }

            if (this.Expiration.HasValue)
            {
                long expiration = new UnixTime(this.Expiration.Value).EpochTimestamp;
                payload.Add(EventTokenClaimTypes.Expiration, expiration);
            }

            payload.Add(EventTokenClaimTypes.Events, this.Events);

            if (!string.IsNullOrWhiteSpace(this.Transaction))
            {
                payload.Add(EventTokenClaimTypes.Transaction, this.Transaction);
            }

            SecurityToken token = new JwtSecurityToken(this.Header, payload);
            string result = TokenSerializer.Value.WriteToken(token);
            return result;
        }
    }
}