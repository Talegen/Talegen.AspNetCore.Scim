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
    /// This class implements a exception notification message.
    /// </summary>
    public sealed class ExceptionNotification : Notification<Exception>, IExceptionNotification
    {
        /// <inheritdoc />
        public ExceptionNotification(Exception payload, bool critical)
            : base(payload)
        {
            this.Critical = critical;
        }

        /// <inheritdoc />
        public ExceptionNotification(Exception payload)
            : this(payload, false)
        {
        }

        /// <inheritdoc />
        public ExceptionNotification(Exception payload, long identifier)
            : base(payload, identifier)
        {
            this.Critical = false;
        }

        /// <inheritdoc />
        public ExceptionNotification(Exception payload, bool critical, long identifier)
            : base(payload, identifier)
        {
            this.Critical = critical;
        }

        /// <inheritdoc />
        public ExceptionNotification(Exception payload, bool critical, string correlationIdentifier)
            : base(payload, correlationIdentifier)
        {
            this.Critical = critical;
        }

        /// <inheritdoc />
        public ExceptionNotification(Exception payload, string correlationIdentifier)
            : this(payload, false, correlationIdentifier)
        {
        }

        /// <inheritdoc />
        public ExceptionNotification(Exception payload, bool critical, string correlationIdentifier, long identifier)
            : base(payload, correlationIdentifier, identifier)
        {
            this.Critical = critical;
        }

        /// <inheritdoc />
        public ExceptionNotification(Exception payload, string correlationIdentifier, long identifier)
            : this(payload, false, correlationIdentifier, identifier)
        {
        }

        /// <inheritdoc />
        public bool Critical { get; set; }
    }
}