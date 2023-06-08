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
    /// This interface defines the minimum implementation of a notification factory.
    /// </summary>
    /// <typeparam name="TPayload">Contains the payload type.</typeparam>
    /// <typeparam name="TNotification">Contains the notification output type.</typeparam>
    public abstract class NotificationFactory<TPayload, TNotification>
    {
        /// <summary>
        /// This method is used to create a new notification from the factory.
        /// </summary>
        /// <param name="payload">Contains the payload.</param>
        /// <param name="correlationIdentifier">Contains a correlation identifier.</param>
        /// <param name="identifier">Contains an identifier.</param>
        /// <returns>Returns a new notification message.</returns>
        public abstract TNotification CreateNotification(TPayload payload, string correlationIdentifier, long? identifier);

        /// <summary>
        /// This method is used to create a new notification from the factory.
        /// </summary>
        /// <param name="payload">Contains the payload.</param>
        /// <param name="correlationIdentifier">Contains a correlation identifier.</param>
        /// <param name="identifier">Contains an identifier.</param>
        /// <returns>Returns a new notification message.</returns>
        public TNotification CreateNotification(TPayload payload, Guid correlationIdentifier, long? identifier)
        {
            string correlationIdentifierValue = correlationIdentifier.ToString();
            TNotification result = this.CreateNotification(payload, correlationIdentifierValue, identifier);
            return result;
        }
    }
}