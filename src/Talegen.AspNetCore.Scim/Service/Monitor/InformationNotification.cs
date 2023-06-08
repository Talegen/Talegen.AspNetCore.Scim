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
    /// <summary>
    /// This class represents an informational notification to be sent to a monitor.
    /// </summary>
    public sealed class InformationNotification : Notification<string>, IInformationNotification
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InformationNotification" /> class.
        /// </summary>
        /// <param name="payload">Contains the payload.</param>
        public InformationNotification(string payload)
            : this(payload, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InformationNotification" /> class.
        /// </summary>
        /// <param name="payload">Contains the payload.</param>
        /// <param name="verbose">Contains a value indicating whether the message reporting is verbose.</param>
        public InformationNotification(string payload, bool verbose)
            : base(payload)
        {
            this.Verbose = verbose;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InformationNotification" /> class.
        /// </summary>
        /// <param name="payload">Contains the payload.</param>
        /// <param name="identifier">Contains an identifier.</param>
        public InformationNotification(string payload, long identifier)
            : base(payload, identifier)
        {
            this.Verbose = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InformationNotification" /> class.
        /// </summary>
        /// <param name="payload">Contains the payload.</param>
        /// <param name="verbose">Contains a value indicating whether the message reporting is verbose.</param>
        /// <param name="identifier">Contains an identifier.</param>
        public InformationNotification(string payload, bool verbose, long identifier)
            : base(payload, identifier)
        {
            this.Verbose = verbose;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InformationNotification" /> class.
        /// </summary>
        /// <param name="payload">Contains the payload.</param>
        /// <param name="verbose">Contains a value indicating whether the message reporting is verbose.</param>
        /// <param name="correlationIdentifier">Contains a correlation identifier.</param>
        public InformationNotification(string payload, bool verbose, string correlationIdentifier)
            : base(payload, correlationIdentifier)
        {
            this.Verbose = verbose;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InformationNotification" /> class.
        /// </summary>
        /// <param name="payload">Contains the payload.</param>
        /// <param name="correlationIdentifier">Contains a correlation identifier.</param>
        public InformationNotification(string payload, string correlationIdentifier)
            : this(payload, false, correlationIdentifier)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InformationNotification" /> class.
        /// </summary>
        /// <param name="payload">Contains the payload.</param>
        /// <param name="verbose">Contains a value indicating whether the message reporting is verbose.</param>
        /// <param name="correlationIdentifier">Contains a correlation identifier.</param>
        /// <param name="identifier">Contains an identifier.</param>
        public InformationNotification(string payload, bool verbose, string correlationIdentifier, long identifier)
            : base(payload, correlationIdentifier, identifier)
        {
            this.Verbose = verbose;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InformationNotification" /> class.
        /// </summary>
        /// <param name="payload">Contains the payload.</param>
        /// <param name="correlationIdentifier">Contains a correlation identifier.</param>
        /// <param name="identifier">Contains an identifier.</param>
        public InformationNotification(string payload, string correlationIdentifier, long identifier)
            : this(payload, false, correlationIdentifier, identifier)
        {
        }

        /// <inheritdoc />
        public bool Verbose { get; set; }
    }
}