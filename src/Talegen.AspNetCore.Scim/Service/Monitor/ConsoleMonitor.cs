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
    /// This implements a basic Console rendering monitor for testing.
    /// </summary>
    public sealed class ConsoleMonitor : IMonitor
    {
        /// <summary>
        /// Contains the console serialization notification template.
        /// </summary>
        private const string PrefixTemplate = "{0}{1}<{2}>";

        /// <summary>
        /// Contains the correlation identifier default
        /// </summary>
        private static readonly Lazy<string> CorrelationIdentifierDefault = new Lazy<string>(() => Guid.Empty.ToString());

        /// <summary>
        /// This method is used to compose a prefix for the notification message.
        /// </summary>
        /// <typeparam name="TPayload">Contains the payload type.</typeparam>
        /// <param name="notification">Contains the payload.</param>
        /// <returns>Returns a prefix for the notification.</returns>
        /// <exception cref="ArgumentNullException">Exception is thrown if the notification payload is not specified.</exception>
        private static string ComposePrefix<TPayload>(INotification<TPayload> notification)
        {
            if (notification == null)
            {
                throw new ArgumentNullException(nameof(notification));
            }

            string effectiveCorrelationIdentifier = string.IsNullOrWhiteSpace(notification.CorrelationIdentifier) ?
                    CorrelationIdentifierDefault.Value :
                    notification.CorrelationIdentifier;
            string effectiveMessageIdentifier = notification.Identifier.HasValue ?
                    string.Empty :
                    string.Format(CultureInfo.InvariantCulture, Schema.Properties.Resources.MonitorCorrelationIdentifierPrefixTemplate, notification.Identifier);
            string result =
                string.Format(CultureInfo.InvariantCulture, PrefixTemplate,
                    effectiveMessageIdentifier, effectiveCorrelationIdentifier,
                    Environment.CurrentManagedThreadId);

            return result;
        }

        /// <inheritdoc />
        public void Inform(IInformationNotification notification)
        {
            if (notification == null)
            {
                throw new ArgumentNullException(nameof(notification));
            }

            string prefix = ComposePrefix(notification);
            Console.WriteLine(Schema.Properties.Resources.MonitorOutputInformationTemplate, prefix, notification.Message, notification.Verbose);
        }

        /// <inheritdoc />
        public void Report(IExceptionNotification notification)
        {
            if (notification == null)
            {
                throw new ArgumentNullException(nameof(notification));
            }

            string prefix = ComposePrefix(notification);
            Console.WriteLine(Schema.Properties.Resources.MonitorOutputExceptionTemplate, prefix, notification.Message, notification.Critical);
        }

        /// <inheritdoc />
        public void Warn(Notification<Exception> notification)
        {
            if (notification == null)
            {
                throw new ArgumentNullException(nameof(notification));
            }

            string prefix = ComposePrefix(notification);
            Console.WriteLine(Schema.Properties.Resources.MonitorOutputTemplate, prefix, notification.Message);
        }

        /// <inheritdoc />
        public void Warn(Notification<string> notification)
        {
            if (notification == null)
            {
                throw new ArgumentNullException(nameof(notification));
            }

            string prefix = ComposePrefix(notification);
            Console.WriteLine(Schema.Properties.Resources.MonitorOutputTemplate, prefix, notification.Message);
        }
    }
}