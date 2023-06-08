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
    /// This class implements a factory for exception notifications.
    /// </summary>
    public sealed class ExceptionNotificationFactory : NotificationFactory<Exception, IExceptionNotification>
    {
        /// <summary>
        /// Contains the factory Singleton
        /// </summary>
        private static readonly Lazy<NotificationFactory<Exception, IExceptionNotification>> Singleton = new(() => new ExceptionNotificationFactory());

        /// <summary>
        /// Initializes a new instance of the <see cref="ExceptionNotificationFactory" /> class.
        /// </summary>
        private ExceptionNotificationFactory()
        {
        }

        /// <summary>
        /// Gets the factory instance.
        /// </summary>
        public static NotificationFactory<Exception, IExceptionNotification> Instance => Singleton.Value;

        /// <inheritdoc />
        public override IExceptionNotification CreateNotification(Exception payload, string correlationIdentifier, long? identifier)
        {
            if (payload == null)
            {
                throw new ArgumentNullException(nameof(payload));
            }

            IExceptionNotification result;

            if (string.IsNullOrWhiteSpace(correlationIdentifier))
            {
                result = !identifier.HasValue ?
                    new ExceptionNotification(payload) :
                    new ExceptionNotification(payload, identifier.Value);
            }
            else
            {
                result = !identifier.HasValue ?
                    new ExceptionNotification(payload, correlationIdentifier) :
                    new ExceptionNotification(payload, correlationIdentifier, identifier.Value);
            }
            return result;
        }
    }
}