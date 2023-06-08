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

namespace Talegen.AspNetCore.Scim.Schema
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Class Core2ResourceType. This class cannot be inherited. Implements the <see cref="Talegen.AspNetCore.Scim.Schema.Resource" />
    /// </summary>
    /// <seealso cref="Talegen.AspNetCore.Scim.Schema.Resource" />
    [DataContract]
    public sealed class Core2ResourceType : Resource
    {
        /// <summary>
        /// The endpoint
        /// </summary>
        private Uri endpoint;

        /// <summary>
        /// The endpoint value
        /// </summary>
        [DataMember(Name = AttributeNames.Endpoint)]
        private string endpointValue;

        /// <summary>
        /// The name
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Justification = "Serialized")]
        [DataMember(Name = AttributeNames.Name)]
        private string name;

        /// <summary>
        /// Initializes a new instance of the <see cref="Core2ResourceType" /> class.
        /// </summary>
        public Core2ResourceType()
        {
            this.AddSchema(SchemaIdentifiers.Core2ResourceType);
            this.Metadata = new Core2Metadata
            {
                ResourceType = Types.ResourceType
            };
        }

        /// <summary>
        /// Gets or sets the endpoint.
        /// </summary>
        /// <value>The endpoint.</value>
        public Uri Endpoint
        {
            get => this.endpoint;

            set
            {
                this.endpoint = value;
                this.endpointValue = new SystemForCrossDomainIdentityManagementResourceIdentifier(value).RelativePath;
            }
        }

        /// <summary>
        /// Gets or sets the metadata.
        /// </summary>
        /// <value>The metadata.</value>
        [DataMember(Name = AttributeNames.Metadata)]
        public Core2Metadata Metadata
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the schema.
        /// </summary>
        /// <value>The schema.</value>
        [DataMember(Name = AttributeNames.Schema)]
        public string Schema
        {
            get;
            set;
        }

        /// <summary>
        /// Initializes the endpoint.
        /// </summary>
        /// <param name="value">The value.</param>
        private void InitializeEndpoint(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                this.endpoint = null;
                return;
            }

            this.endpoint = new Uri(value, UriKind.Relative);
        }

        /// <summary>
        /// Initializes the endpoint.
        /// </summary>
        private void InitializeEndpoint()
        {
            this.InitializeEndpoint(this.endpointValue);
        }

        /// <summary>
        /// This event method is fired when deserialization.
        /// </summary>
        /// <param name="context">Contains the stream context.</param>
        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            this.InitializeEndpoint();
        }

        /// <summary>
        /// Called when [serializing].
        /// </summary>
        /// <param name="context">The context.</param>
        [OnSerializing]
        private void OnSerializing(StreamingContext context)
        {
            this.name = this.Identifier;
        }
    }
}