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
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;

    /// <summary>
    /// Class TypeScheme. This class cannot be inherited. Implements the <see cref="Talegen.AspNetCore.Scim.Schema.Resource" />
    /// </summary>
    /// <seealso cref="Talegen.AspNetCore.Scim.Schema.Resource" />
    [DataContract]
    public sealed class TypeScheme : Resource
    {
        /// <summary>
        /// The attributes
        /// </summary>
        private List<AttributeScheme> attributes;

        /// <summary>
        /// The attributes wrapper
        /// </summary>
        private IReadOnlyCollection<AttributeScheme> attributesWrapper;

        /// <summary>
        /// The this lock
        /// </summary>
        private object thisLock;

        /// <summary>
        /// Initializes a new instance of the <see cref="TypeScheme" /> class.
        /// </summary>
        public TypeScheme()
        {
            this.OnInitialization();
            this.OnInitialized();
            this.AddSchema(SchemaIdentifiers.Core2Schema);
            this.Metadata = new Core2Metadata
            {
                ResourceType = Types.Schema
            };
        }

        /// <summary>
        /// Gets the attributes.
        /// </summary>
        /// <value>The attributes.</value>
        [DataMember(Name = AttributeNames.Attributes, Order = 0)]
        public IReadOnlyCollection<AttributeScheme> Attributes => this.attributesWrapper;

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        [DataMember(Name = AttributeNames.Name)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        [DataMember(Name = AttributeNames.Description)]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the metadata.
        /// </summary>
        /// <value>The metadata.</value>
        [DataMember(Name = AttributeNames.Metadata)]
        public Core2Metadata Metadata { get; set; }

        /// <summary>
        /// Adds the attribute.
        /// </summary>
        /// <param name="attribute">The attribute.</param>
        /// <exception cref="System.ArgumentNullException">attribute</exception>
        public void AddAttribute(AttributeScheme attribute)
        {
            if (attribute == null)
            {
                throw new ArgumentNullException(nameof(attribute));
            }

            var containsFunction = new Func<bool>(() => this.attributes.Any((AttributeScheme item) => string.Equals(item.Name, attribute.Name, StringComparison.OrdinalIgnoreCase)));

            if (!containsFunction())
            {
                lock (this.thisLock)
                {
                    if (!containsFunction())
                    {
                        this.attributes.Add(attribute);
                    }
                }
            }
        }

        /// <summary>
        /// This event method is fired when deserialization.
        /// </summary>
        /// <param name="context">Contains the stream context.</param>
        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            this.OnInitialized();
        }

        /// <summary>
        /// This event method is fired on deserializing.
        /// </summary>
        /// <param name="context">Contains the stream context.</param>
        [OnDeserializing]
        private void OnDeserializing(StreamingContext context)
        {
            this.OnInitialization();
        }

        /// <summary>
        /// This event method is called on initialization.
        /// </summary>
        private void OnInitialization()
        {
            this.thisLock = new object();
            this.attributes = new List<AttributeScheme>();
        }

        /// <summary>
        /// This event method is called after initialization.
        /// </summary>
        private void OnInitialized()
        {
            this.attributesWrapper = this.attributes.AsReadOnly();
        }
    }
}