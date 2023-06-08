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

namespace Talegen.AspNetCore.Scim.Service
{
    using System;
    using Monitor;

    /// <summary>
    /// This class implements a severe warning notification factory.
    /// </summary>
    public sealed class SevereWarningNotificationFactory : NotificationFactory<Exception, Notification<Exception>>
    {
        /// <summary>
        /// Contains the factory.
        /// </summary>
        private static readonly Lazy<SevereWarningNotificationFactory> Singleton = new(() => new SevereWarningNotificationFactory());

        /// <summary>
        /// Initializes a new instance of the <see cref="SevereWarningNotificationFactory" /> class.
        /// </summary>
        private SevereWarningNotificationFactory()
        {
        }

        /// <summary>
        /// Gets the factory instance.
        /// </summary>
        public static SevereWarningNotificationFactory Instance => Singleton.Value;

        /// <inheritdoc />
        public override Notification<Exception> CreateNotification(Exception payload, string correlationIdentifier, long? identifier)
        {
            if (payload == null)
            {
                throw new ArgumentNullException(nameof(payload));
            }

            Notification<Exception> result;

            if (string.IsNullOrWhiteSpace(correlationIdentifier))
            {
                result = !identifier.HasValue ? new ExceptionNotification(payload) : new ExceptionNotification(payload, identifier.Value);
            }
            else
            {
                result = !identifier.HasValue ? new ExceptionNotification(payload, correlationIdentifier) : new ExceptionNotification(payload, correlationIdentifier, identifier.Value);
            }

            return result;
        }
    }
}