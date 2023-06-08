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
    /// This class implements a verbose information notification factory.
    /// </summary>
    public sealed class VerboseInformationNotificationFactory : NotificationFactoryBase<IInformationNotification>
    {
        /// <summary>
        /// Contains the factory Singleton.
        /// </summary>
        private static readonly Lazy<NotificationFactoryBase<IInformationNotification>> Singleton = new(() => new VerboseInformationNotificationFactory());

        /// <summary>
        /// Initializes a new instance of the <see cref="VerboseInformationNotificationFactory" /> class.
        /// </summary>
        private VerboseInformationNotificationFactory()
        {
        }

        /// <summary>
        /// Gets the factory instance.
        /// </summary>
        public static NotificationFactoryBase<IInformationNotification> Instance => VerboseInformationNotificationFactory.Singleton.Value;

        /// <inheritdoc />
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
                    new InformationNotification(payload, true) :
                    new InformationNotification(payload, true, identifier.Value);
            }
            else
            {
                result = !identifier.HasValue ?
                    new InformationNotification(payload, true, correlationIdentifier) :
                    new InformationNotification(payload, true, correlationIdentifier, identifier.Value);
            }
            return result;
        }
    }
}