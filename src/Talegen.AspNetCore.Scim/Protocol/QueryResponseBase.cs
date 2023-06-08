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
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using Schema;

    /// <inheritdoc />
    [DataContract]
    public abstract class QueryResponseBase : Schematized
    {
        /// <summary>
        /// Contains the resources of the response.
        /// </summary>
        [DataMember(Name = ProtocolAttributeNames.Resources, Order = 3)]
        private Resource[] resources = null;

        /// <inheritdoc />
        protected QueryResponseBase()
        {
            this.AddSchema(ProtocolSchemaIdentifiers.Version2ListResponse);
        }

        /// <inheritdoc />
        protected QueryResponseBase(IReadOnlyCollection<Resource> resources)
            : this()
        {
            if (null == resources)
            {
                throw new ArgumentNullException(nameof(resources));
            }

            this.resources = resources.ToArray();
        }

        /// <inheritdoc />
        protected QueryResponseBase(IList<Resource> resources)
            : this()
        {
            if (null == resources)
            {
                throw new ArgumentNullException(nameof(resources));
            }

            this.resources = resources.ToArray();
        }

        /// <summary>
        /// Gets or sets the items per page value.
        /// </summary>
        [DataMember(Name = ProtocolAttributeNames.ItemsPerPage, Order = 1)]
        public int ItemsPerPage { get; set; }

        /// <summary>
        /// Gets or sets the resources.
        /// </summary>
        /// <exception cref="InvalidOperationException">Exception is thrown if the value is not specified.</exception>
        public IEnumerable<Resource> Resources
        {
            get => this.resources;

            set
            {
                if (value == null)
                {
                    throw new InvalidOperationException(Schema.Properties.Resources.ExceptionInvalidValue);
                }

                this.resources = value.ToArray();
            }
        }

        /// <summary>
        /// Gets or sets the start index.
        /// </summary>
        [DataMember(Name = ProtocolAttributeNames.StartIndex, Order = 2)]
        public int? StartIndex { get; set; }

        /// <summary>
        /// Gets or sets the total results.
        /// </summary>
        [DataMember(Name = ProtocolAttributeNames.TotalResults, Order = 0)]
        public int TotalResults { get; set; }
    }

    /// <inheritdoc />
    [DataContract]
    public abstract class QueryResponseBase<TResource> : Schematized where TResource : Resource
    {
        /// <summary>
        /// Contains the response resources.
        /// </summary>
        [DataMember(Name = ProtocolAttributeNames.Resources, Order = 3)]
        private TResource[] resources;

        /// <inheritdoc />
        protected QueryResponseBase(string schemaIdentifier)
        {
            if (string.IsNullOrWhiteSpace(schemaIdentifier))
            {
                throw new ArgumentNullException(nameof(schemaIdentifier));
            }

            this.AddSchema(schemaIdentifier);
            this.OnInitialization();
        }

        /// <inheritdoc />
        protected QueryResponseBase(string schemaIdentifier, IReadOnlyCollection<TResource> resources)
            : this(schemaIdentifier)
        {
            if (null == resources)
            {
                throw new ArgumentNullException(nameof(resources));
            }

            this.resources = resources.ToArray();
        }

        /// <inheritdoc />
        protected QueryResponseBase(string schemaIdentifier, IList<TResource> resources)
            : this(schemaIdentifier)
        {
            if (null == resources)
            {
                throw new ArgumentNullException(nameof(resources));
            }

            this.resources = resources.ToArray();
        }

        /// <summary>
        /// Gets or sets the items per page.
        /// </summary>
        [DataMember(Name = ProtocolAttributeNames.ItemsPerPage, Order = 1)]
        public int ItemsPerPage { get; set; }

        /// <summary>
        /// Gets or sets the resources.
        /// </summary>
        public IEnumerable<TResource> Resources
        {
            get => this.resources;

            set
            {
                if (value == null)
                {
                    throw new InvalidOperationException(Schema.Properties.Resources.ExceptionInvalidValue);
                }

                this.resources = value.ToArray();
            }
        }

        /// <summary>
        /// Gets or sets the start index.
        /// </summary>
        [DataMember(Name = ProtocolAttributeNames.StartIndex, Order = 2)]
        public int? StartIndex { get; set; }

        /// <summary>
        /// Gets or sets the total results.
        /// </summary>
        [DataMember(Name = ProtocolAttributeNames.TotalResults, Order = 0)]
        public int TotalResults { get; set; }

        /// <summary>
        /// This method is called during deserialization.
        /// </summary>
        /// <param name="context">Contains the stream context.</param>
        [OnDeserializing]
        private void OnDeserializing(StreamingContext context)
        {
            this.OnInitialization();
        }

        /// <summary>
        /// This method is called to initialize the resources.
        /// </summary>
        private void OnInitialization()
        {
            this.resources = Array.Empty<TResource>();
        }
    }
}