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
    /// This interface defines the minimum implementation of a monitor.
    /// </summary>
    public interface IMonitor
    {
        /// <summary>
        /// This method is used to signal to a monitor of an informational notification.
        /// </summary>
        /// <param name="notification">Contains the informational notification.</param>
        void Inform(IInformationNotification notification);

        /// <summary>
        /// This method is used to signal to a monitor an exception has occurred.
        /// </summary>
        /// <param name="notification">Contains the exception notification.</param>
        void Report(IExceptionNotification notification);

        /// <summary>
        /// This method is used to signal a warning to a monitor an exception has occurred.
        /// </summary>
        /// <param name="notification">Contains the exception notification.</param>
        void Warn(Notification<Exception> notification);

        /// <summary>
        /// This method is used to signal a warning to a monitor with a message.
        /// </summary>
        /// <param name="notification">Contains the message notification.</param>
        void Warn(Notification<string> notification);
    }
}