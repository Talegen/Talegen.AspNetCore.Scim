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

    /// <summary>
    /// This class implements a warning notification message factory.
    /// </summary>
    public sealed class WarningNotificationFactory : NotificationFactoryBase<Notification<string>>
    {
        /// <summary>
        /// Contains the factory Singleton
        /// </summary>
        private static readonly Lazy<WarningNotificationFactory> Singleton = new(() => new WarningNotificationFactory());

        /// <summary>
        /// Initializes a new instance of the <see cref="WarningNotificationFactory" /> class.
        /// </summary>
        private WarningNotificationFactory()
        {
        }

        /// <summary>
        /// Gets the factory instance.
        /// </summary>
        public static WarningNotificationFactory Instance => WarningNotificationFactory.Singleton.Value;

        /// <inheritdoc />
        public override Notification<string> CreateNotification(string payload, string correlationIdentifier, long? identifier)
        {
            if (string.IsNullOrWhiteSpace(payload))
            {
                throw new ArgumentNullException(nameof(payload));
            }

            Notification<string> result;

            if (string.IsNullOrWhiteSpace(correlationIdentifier))
            {
                result = !identifier.HasValue ?
                    new InformationNotification(payload) :
                    new InformationNotification(payload, identifier.Value);
            }
            else
            {
                result = !identifier.HasValue ?
                    new InformationNotification(payload, correlationIdentifier) :
                    new InformationNotification(payload, correlationIdentifier, identifier.Value);
            }
            return result;
        }
    }
}