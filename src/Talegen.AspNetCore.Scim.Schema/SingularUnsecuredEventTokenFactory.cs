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

    /// <summary>
    /// Class SingularUnsecuredEventTokenFactory. Implements the <see cref="Talegen.AspNetCore.Scim.Schema.UnsecuredEventTokenFactory" />
    /// </summary>
    /// <seealso cref="Talegen.AspNetCore.Scim.Schema.UnsecuredEventTokenFactory" />
    public class SingularUnsecuredEventTokenFactory : UnsecuredEventTokenFactory
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SingularUnsecuredEventTokenFactory" /> class.
        /// </summary>
        /// <param name="issuer">The issuer.</param>
        /// <param name="eventSchemaIdentifier">The event schema identifier.</param>
        /// <exception cref="System.ArgumentNullException">eventSchemaIdentifier</exception>
        public SingularUnsecuredEventTokenFactory(string issuer, string eventSchemaIdentifier)
            : base(issuer)
        {
            if (string.IsNullOrWhiteSpace(eventSchemaIdentifier))
            {
                throw new ArgumentNullException(nameof(eventSchemaIdentifier));
            }

            this.EventSchemaIdentifier = eventSchemaIdentifier;
        }

        /// <summary>
        /// Gets or sets the event schema identifier.
        /// </summary>
        /// <value>The event schema identifier.</value>
        private string EventSchemaIdentifier
        {
            get;
        }

        /// <summary>
        /// Creates the specified events.
        /// </summary>
        /// <param name="events">The events.</param>
        /// <returns>IEventToken.</returns>
        public override IEventToken Create(IDictionary<string, object> events)
        {
            IDictionary<string, object> tokenEvents = new Dictionary<string, object>(1);
            tokenEvents.Add(this.EventSchemaIdentifier, events);
            IEventToken result = new EventToken(this.Issuer, this.Header, tokenEvents);
            return result;
        }
    }
}