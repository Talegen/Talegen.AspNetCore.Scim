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
    using System.Collections.ObjectModel;
    using System.Runtime.Serialization;

    /// <summary>
    /// Class Core2GroupBase. Implements the <see cref="Talegen.AspNetCore.Scim.Schema.GroupBase" />
    /// </summary>
    /// <seealso cref="Talegen.AspNetCore.Scim.Schema.GroupBase" />
    [DataContract]
    public abstract class Core2GroupBase : GroupBase
    {
        /// <summary>
        /// The custom extension
        /// </summary>
        private IDictionary<string, IDictionary<string, object>> customExtension;

        /// <summary>
        /// Initializes a new instance of the <see cref="Core2GroupBase" /> class.
        /// </summary>
        protected Core2GroupBase()
        {
            this.AddSchema(SchemaIdentifiers.Core2Group);
            this.Metadata = new Core2Metadata
            {
                ResourceType = Types.Group
            };
            this.OnInitialization();
        }

        /// <summary>
        /// Gets the custom extension.
        /// </summary>
        /// <value>The custom extension.</value>
        public virtual IReadOnlyDictionary<string, IDictionary<string, object>> CustomExtension => new ReadOnlyDictionary<string, IDictionary<string, object>>(this.customExtension);

        /// <summary>
        /// Gets or sets the metadata.
        /// </summary>
        /// <value>The metadata.</value>
        [DataMember(Name = AttributeNames.Metadata)]
        public Core2Metadata Metadata { get; set; }

        /// <summary>
        /// Adds the custom attribute.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public virtual void AddCustomAttribute(string key, object value)
        {
            if (key.StartsWith(SchemaIdentifiers.PrefixExtension, StringComparison.OrdinalIgnoreCase) && value is Dictionary<string, object> nestedObject)
            {
                this.customExtension.Add(key, nestedObject);
            }
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
            this.customExtension = new Dictionary<string, IDictionary<string, object>>();
        }

        /// <summary>
        /// Converts to json.
        /// </summary>
        /// <returns>Dictionary&lt;System.String, System.Object&gt;.</returns>
        public override Dictionary<string, object> ToJson()
        {
            Dictionary<string, object> result = base.ToJson();

            foreach (KeyValuePair<string, IDictionary<string, object>> entry in this.CustomExtension)
            {
                result.Add(entry.Key, entry.Value);
            }

            return result;
        }
    }
}