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
    /// This class represents a schematized object.
    /// </summary>
    [DataContract]
    public abstract class Schematized : IJsonSerializable
    {
        /// <summary>
        /// Contains a list of schemas.
        /// </summary>
        [DataMember(Name = AttributeNames.Schemas, Order = 0)]
        private List<string> schemas;

        /// <summary>
        /// Contains a read-only collection of schemas.
        /// </summary>
        private IReadOnlyCollection<string> schemasWrapper;

        /// <summary>
        /// Contains the lock.
        /// </summary>
        private object thisLock;

        /// <summary>
        /// Contains an instance of the JSON serializer.
        /// </summary>
        private IJsonSerializable serializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="Schematized" /> class.
        /// </summary>
        protected Schematized()
        {
            this.OnInitialization();
            this.OnInitialized();
        }

        /// <summary>
        /// Gets the schema collection.
        /// </summary>
        /// <value>The schemas.</value>
        public virtual IReadOnlyCollection<string> Schemas => this.schemasWrapper;

        /// <summary>
        /// Adds the schema.
        /// </summary>
        /// <param name="schemaIdentifier">The schema identifier.</param>
        /// <exception cref="System.ArgumentNullException">schemaIdentifier</exception>
        public void AddSchema(string schemaIdentifier)
        {
            if (string.IsNullOrWhiteSpace(schemaIdentifier))
            {
                throw new ArgumentNullException(nameof(schemaIdentifier));
            }

            // setup a contains search function for schema
            Func<bool> containsFunction = new Func<bool>(() => this.schemas.Any((string item) => string.Equals(item, schemaIdentifier, StringComparison.OrdinalIgnoreCase)));

            // execute search
            if (!containsFunction())
            {
                lock (this.thisLock)
                {
                    if (!containsFunction())
                    {
                        // add the schema identifier
                        this.schemas.Add(schemaIdentifier);
                    }
                }
            }
        }

        /// <summary>
        /// Determines if the scheme is already in schemas
        /// </summary>
        /// <param name="scheme">Contains the schema to find.</param>
        /// <returns>Returns a value indicating whether the scheme is found.</returns>
        /// <exception cref="System.ArgumentNullException">scheme</exception>
        public bool Is(string scheme)
        {
            if (string.IsNullOrWhiteSpace(scheme))
            {
                throw new ArgumentNullException(nameof(scheme));
            }

            return this.schemas.Any((string item) => string.Equals(item, scheme, StringComparison.OrdinalIgnoreCase));
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
            this.serializer = new JsonSerializer(this);
            this.schemas = new List<string>();
        }

        /// <summary>
        /// This event method is called after initialization.
        /// </summary>
        private void OnInitialized()
        {
            this.schemasWrapper = this.schemas.AsReadOnly();
        }

        /// <summary>
        /// This method is used to convert the object to JSON.
        /// </summary>
        /// <returns>Returns a dictionary of JSON</returns>
        public virtual Dictionary<string, object> ToJson()
        {
            return this.serializer.ToJson();
        }

        /// <summary>
        /// This method is called to serialize the object.
        /// </summary>
        /// <returns>Returns the JSON object.</returns>
        public virtual string Serialize()
        {
            IDictionary<string, object> json = this.ToJson();
            return JsonFactory.Instance.Create(json, true);
        }

        /// <summary>
        /// This method is called to serialize the object.
        /// </summary>
        /// <returns>Returns the JSON object.</returns>
        public override string ToString()
        {
            return this.Serialize();
        }

        /// <summary>
        /// Tries the get path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public virtual bool TryGetPath(out string path)
        {
            path = null;
            return false;
        }

        /// <summary>
        /// Tries the get schema identifier.
        /// </summary>
        /// <param name="schemaIdentifier">The schema identifier.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public virtual bool TryGetSchemaIdentifier(out string schemaIdentifier)
        {
            schemaIdentifier = null;
            return false;
        }
    }
}