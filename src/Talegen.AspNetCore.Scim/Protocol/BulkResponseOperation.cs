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
    /// This class represents a bulk response operation.
    /// </summary>
    [DataContract]
    [KnownType(typeof(ErrorResponse))]
    [KnownType(typeof(Core2EnterpriseUser))]
    [KnownType(typeof(QueryResponse<Core2EnterpriseUser>))]
    [KnownType(typeof(Core2User))]
    [KnownType(typeof(QueryResponse<Core2User>))]
    [KnownType(typeof(QueryResponse<Core2Group>))]
    [KnownType(typeof(Core2Group))]
    public sealed class BulkResponseOperation : BulkOperation, IResponse
    {
        /// <summary>
        /// Contains the response.
        /// </summary>
        private IResponse response;

        /// <summary>
        /// Initializes a new instance of the <see cref="BulkResponseOperation" /> class.
        /// </summary>
        public BulkResponseOperation()
            : base(null)
        {
            this.OnInitialization();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BulkResponseOperation" /> class.
        /// </summary>
        /// <param name="identifier">Contains the operation identifier</param>
        public BulkResponseOperation(string identifier)
            : base(identifier)
        {
            this.OnInitialization();
        }

        /// <summary>
        /// Gets or sets the Location.
        /// </summary>
        [DataMember(Name = ProtocolAttributeNames.Location)]
        public Uri Location { get; set; }

        /// <summary>
        /// Gets or sets the Response object.
        /// </summary>
        [DataMember(Name = ProtocolAttributeNames.Response)]
        public object Response { get; set; }

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        public HttpStatusCode Status
        {
            get => this.response.Status;
            set => this.response.Status = value;
        }

        /// <summary>
        /// Gets or sets the serialized status.
        /// </summary>
        [DataMember(Name = ProtocolAttributeNames.Status)]
        public string StatusCodeValue
        {
            get => this.response.StatusCodeValue;
            set => this.response.StatusCodeValue = value;
        }

        /// <summary>
        /// Gets a value indicating whether the response was an error.
        /// </summary>
        /// <returns></returns>
        public bool IsError() => this.response.IsError();

        /// <summary>
        /// This method is called when deserializing the response.
        /// </summary>
        /// <param name="_">Contains the streaming context.</param>
        [OnDeserializing]
        private void OnDeserializing(StreamingContext _) => this.OnInitialization();

        /// <summary>
        /// This method is called when response is deserialized.
        /// </summary>
        private void OnInitialization() => this.response = new Response();
    }
}