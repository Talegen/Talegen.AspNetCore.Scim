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
    /// This class implements the minimum implementation of a notification factory.
    /// </summary>
    /// <typeparam name="TNotification">Contains the notification message type.</typeparam>
    public abstract class NotificationFactoryBase<TNotification> : NotificationFactory<string, TNotification>
    {
        /// <inheritdoc />
        public abstract override TNotification CreateNotification(string payload, string correlationIdentifier, long? identifier);

        /// <summary>
        /// This method is used to format a notification message.
        /// </summary>
        /// <param name="template">Contains the template to use for formatting.</param>
        /// <param name="correlationIdentifier">Contains a correlation identifier.</param>
        /// <param name="identifier">Contains an identifier.</param>
        /// <param name="arguments">Contains arguments to add to the formatted message.</param>
        /// <returns>Returns a new notification with formatted message.</returns>
        /// <exception cref="ArgumentNullException">Exception is thrown if a template string is not specified.</exception>
        public TNotification FormatNotification(string template, string correlationIdentifier, long? identifier, params object[] arguments)
        {
            if (string.IsNullOrWhiteSpace(template))
            {
                throw new ArgumentNullException(nameof(template));
            }

            string payload = string.Format(CultureInfo.InvariantCulture, template, arguments);
            TNotification result = this.CreateNotification(payload, correlationIdentifier, identifier);

            return result;
        }

        /// <summary>
        /// This method is used to format a notification message.
        /// </summary>
        /// <param name="template">Contains the template to use for formatting.</param>
        /// <param name="correlationIdentifier">Contains a correlation identifier.</param>
        /// <param name="identifier">Contains an identifier.</param>
        /// <param name="arguments">Contains arguments to add to the formatted message.</param>
        /// <returns>Returns a new notification with formatted message.</returns>
        /// <exception cref="ArgumentNullException">Exception is thrown if a template string is not specified.</exception>
        public TNotification FormatNotification(string template, Guid correlationIdentifier, long? identifier, params object[] arguments)
        {
            if (string.IsNullOrWhiteSpace(template))
            {
                throw new ArgumentNullException(nameof(template));
            }

            string correlationIdentifierValue = correlationIdentifier.ToString();
            TNotification result = this.FormatNotification(template, correlationIdentifierValue, identifier, arguments);
            return result;
        }
    }
}