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

namespace Talegen.AspNetCore.Scim.Protocol
{
    using System;
    using System.Net;
    using System.Runtime.Serialization;
    using Schema;

    /// <summary>
    /// This class represents an error response.
    /// </summary>
    [DataContract]
    public sealed class ErrorResponse : Schematized
    {
        /// <summary>
        /// Contains the error type.
        /// </summary>
        private ErrorType errorType;

        /// <summary>
        /// Contains the serialized error type.
        /// </summary>

        [DataMember(Name = ProtocolAttributeNames.ErrorType)]
        private string errorTypeValue;

        /// <summary>
        /// Contains the response.
        /// </summary>
        private Response response;

        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorResponse" /> class.
        /// </summary>
        public ErrorResponse()
        {
            this.Initialize();
            this.AddSchema(ProtocolSchemaIdentifiers.Version2Error);
        }

        /// <summary>
        /// Gets or sets the detail.
        /// </summary>
        [DataMember(Name = ProtocolAttributeNames.Detail)]
        public string Detail { get; set; }

        /// <summary>
        /// Gets or sets the error type.
        /// </summary>
        public ErrorType ErrorType
        {
            get => this.errorType;

            set
            {
                this.errorType = value;
                this.errorTypeValue = Enum.GetName(typeof(ErrorType), value) ?? string.Empty;
            }
        }

        /// <summary>
        /// Gets or sets the status code.
        /// </summary>
        public HttpStatusCode Status
        {
            get => this.response.Status;

            set => this.response.Status = value;
        }

        /// <summary>
        /// This method is used to initialize the object fields.
        /// </summary>
        private void Initialize()
        {
            this.response = new Response();
        }

        /// <summary>
        /// This method is called during deserialization.
        /// </summary>
        /// <param name="context">Contains the streaming context.</param>
        [OnDeserializing]
        private void OnDeserializing(StreamingContext context)
        {
            this.Initialize();
        }
    }
}