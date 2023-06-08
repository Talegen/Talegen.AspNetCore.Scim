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
    /// This class implements a factory for information notifications.
    /// </summary>
    public sealed class InformationNotificationFactory : NotificationFactoryBase<IInformationNotification>
    {
        /// <summary>
        /// Contains a Singleton factory instance.
        /// </summary>
        private static readonly Lazy<NotificationFactoryBase<IInformationNotification>> Singleton = new(() => new InformationNotificationFactory());

        /// <summary>
        /// Initializes a new instance of the <see cref="InformationNotificationFactory" /> class.
        /// </summary>
        private InformationNotificationFactory()
        {
        }

        /// <summary>
        /// Gets a new instance of the informational notification factory.
        /// </summary>
        public static NotificationFactoryBase<IInformationNotification> Instance => InformationNotificationFactory.Singleton.Value;

        /// <summary>
        /// This method is used to create a new informational notification.
        /// </summary>
        /// <param name="payload">Contains the payload.</param>
        /// <param name="correlationIdentifier">Contains a correlation identifier.</param>
        /// <param name="identifier">Contains an identifier.</param>
        /// <returns>Returns a new information notification.</returns>
        /// <exception cref="ArgumentNullException">Exception is thrown if a payload is not specified.</exception>
        public override IInformationNotification CreateNotification(string payload, string correlationIdentifier, long? identifier)
        {
            if (string.IsNullOrWhiteSpace(payload))
            {
                throw new ArgumentNullException(nameof(payload));
            }

            IInformationNotification result;

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