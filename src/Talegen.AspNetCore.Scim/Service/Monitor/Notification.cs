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

namespace Talegen.AspNetCore.Scim.Service.Monitor
{
    using System;
    using System.Globalization;

    /// <summary>
    /// This class implements the minimum implementation of a notification message of a specified payload type.
    /// </summary>
    /// <typeparam name="TPayload">Contains the payload message type.</typeparam>
    public abstract class Notification<TPayload> : INotification<TPayload> where TPayload : class
    {
        /// <summary>
        /// Contains the serialization template for the message.
        /// </summary>
        private const string Template = @"{0:O} {1} {2} {3}";

        /// <summary>
        /// Contains a Thread-Unique static field for the correlation identifier default.
        /// </summary>
        [ThreadStatic]
        private static string correlationIdentifierDefault;

        /// <summary>
        /// Contains the correlation identifier value.
        /// </summary>
        private readonly string correlationIdentifierValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="Notification{TPayload}" /> class.
        /// </summary>
        /// <param name="payload">Contains the payload</param>
        /// <exception cref="ArgumentNullException">Exception is thrown if the payload is not specified.</exception>
        protected Notification(TPayload payload)
        {
            this.Message = payload ?? throw new ArgumentNullException(nameof(payload));
        }

        /// <inheritdoc />
        protected Notification(TPayload payload, long identifier)
            : this(payload)
        {
            if (payload == null)
            {
                throw new ArgumentNullException(nameof(payload));
            }

            this.Identifier = identifier;
        }

        /// <inheritdoc />
        protected Notification(TPayload payload, string correlationIdentifier)
            : this(payload)
        {
            if (payload == null)
            {
                throw new ArgumentNullException(nameof(payload));
            }

            if (string.IsNullOrWhiteSpace(correlationIdentifier))
            {
                throw new ArgumentNullException(nameof(correlationIdentifier));
            }

            this.CorrelationIdentifier = correlationIdentifier;
        }

        /// <inheritdoc />
        protected Notification(TPayload payload, string correlationIdentifier, long identifier)
            : this(payload, correlationIdentifier)
        {
            if (payload == null)
            {
                throw new ArgumentNullException(nameof(payload));
            }

            if (string.IsNullOrWhiteSpace(correlationIdentifier))
            {
                throw new ArgumentNullException(nameof(correlationIdentifier));
            }

            this.Identifier = identifier;
        }

        /// <inheritdoc />
        public string CorrelationIdentifier
        {
            get
            {
                string result =
                    string.IsNullOrWhiteSpace(this.correlationIdentifierValue) ?
                        correlationIdentifierDefault : this.correlationIdentifierValue;
                return result;
            }

            private init => correlationIdentifierDefault = this.correlationIdentifierValue = value;
        }

        /// <inheritdoc />
        public long? Identifier { get; }

        /// <inheritdoc />
        public TPayload Message { get; }

        /// <inheritdoc />
        public override string ToString()
        {
            string result =
                string.Format(
                    CultureInfo.InvariantCulture,
                    Notification<TPayload>.Template,
                    DateTime.UtcNow,
                    this.CorrelationIdentifier,
                    this.Identifier,
                    this.Message);
            return result;
        }
    }
}