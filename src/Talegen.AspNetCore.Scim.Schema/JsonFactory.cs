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
    using Newtonsoft.Json;

    /// <summary>
    /// This abstract class contains the necessary
    /// </summary>
    public abstract class JsonFactory
    {
        /// <summary>
        /// The large object factory
        /// </summary>
        private static readonly Lazy<JsonFactory> LargeObjectFactory = new Lazy<JsonFactory>(() => new TrustedJsonFactory());

        /// <summary>
        /// The singleton
        /// </summary>
        private static readonly Lazy<JsonFactory> Singleton = new Lazy<JsonFactory>(() => InitializeFactory());

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>The instance.</value>
        public static JsonFactory Instance => Singleton.Value;

        /// <summary>
        /// Creates the specified json.
        /// </summary>
        /// <param name="json">The json.</param>
        /// <returns>Dictionary&lt;System.String, System.Object&gt;.</returns>
        public abstract Dictionary<string, object> Create(string json);

        /// <summary>
        /// Creates the specified json.
        /// </summary>
        /// <param name="json">The json.</param>
        /// <param name="acceptLargeObjects">if set to <c>true</c> [accept large objects].</param>
        /// <returns>Dictionary&lt;System.String, System.Object&gt;.</returns>
        public virtual Dictionary<string, object> Create(string json, bool acceptLargeObjects)
        {
            return acceptLargeObjects ? LargeObjectFactory.Value.Create(json) : this.Create(json);
        }

        /// <summary>
        /// Creates the specified json.
        /// </summary>
        /// <param name="json">The json.</param>
        /// <returns>System.String.</returns>
        public abstract string Create(string[] json);

        /// <summary>
        /// Creates the specified json.
        /// </summary>
        /// <param name="json">The json.</param>
        /// <param name="acceptLargeObjects">if set to <c>true</c> [accept large objects].</param>
        /// <returns>System.String.</returns>
        public virtual string Create(string[] json, bool acceptLargeObjects)
        {
            return acceptLargeObjects ? LargeObjectFactory.Value.Create(json) : this.Create(json);
        }

        /// <summary>
        /// Creates the specified json.
        /// </summary>
        /// <param name="json">The json.</param>
        /// <returns>System.String.</returns>
        public abstract string Create(Dictionary<string, object> json);

        /// <summary>
        /// Creates the specified json.
        /// </summary>
        /// <param name="json">The json.</param>
        /// <param name="acceptLargeObjects">if set to <c>true</c> [accept large objects].</param>
        /// <returns>System.String.</returns>
        public virtual string Create(Dictionary<string, object> json, bool acceptLargeObjects)
        {
            return acceptLargeObjects ? LargeObjectFactory.Value.Create(json) : this.Create(json);
        }

        /// <summary>
        /// Creates the specified json.
        /// </summary>
        /// <param name="json">The json.</param>
        /// <returns>System.String.</returns>
        public abstract string Create(IDictionary<string, object> json);

        /// <summary>
        /// Creates the specified json.
        /// </summary>
        /// <param name="json">The json.</param>
        /// <param name="acceptLargeObjects">if set to <c>true</c> [accept large objects].</param>
        /// <returns>System.String.</returns>
        public virtual string Create(IDictionary<string, object> json, bool acceptLargeObjects)
        {
            return acceptLargeObjects ? LargeObjectFactory.Value.Create(json) : this.Create(json);
        }

        /// <summary>
        /// Creates the specified json.
        /// </summary>
        /// <param name="json">The json.</param>
        /// <returns>System.String.</returns>
        public abstract string Create(IReadOnlyDictionary<string, object> json);

        /// <summary>
        /// Creates the specified json.
        /// </summary>
        /// <param name="json">The json.</param>
        /// <param name="acceptLargeObjects">if set to <c>true</c> [accept large objects].</param>
        /// <returns>System.String.</returns>
        public virtual string Create(IReadOnlyDictionary<string, object> json, bool acceptLargeObjects)
        {
            return acceptLargeObjects ? LargeObjectFactory.Value.Create(json) : this.Create(json);
        }

        /// <summary>
        /// Initializes the factory.
        /// </summary>
        /// <returns>JsonFactory.</returns>
        private static JsonFactory InitializeFactory()
        {
            return SystemForCrossDomainIdentityManagementConfigurationSection.Instance.AcceptLargeObjects ? new TrustedJsonFactory() : new Implementation();
        }

        /// <summary>
        /// This class implements the base <see cref="JsonFactory" /> class.
        /// </summary>
        private class Implementation : JsonFactory
        {
            /// <summary>
            /// Creates the specified json.
            /// </summary>
            /// <param name="json">The json.</param>
            /// <returns>Dictionary&lt;System.String, System.Object&gt;.</returns>
            public override Dictionary<string, object> Create(string json)
            {
                try
                {
                    return JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
                }
                finally
                {
                }
            }

            /// <summary>
            /// Creates the specified json.
            /// </summary>
            /// <param name="json">The json.</param>
            /// <returns>System.String.</returns>
            public override string Create(string[] json)
            {
                return JsonConvert.SerializeObject(json);
            }

            /// <summary>
            /// Creates the specified json.
            /// </summary>
            /// <param name="json">The json.</param>
            /// <returns>System.String.</returns>
            public override string Create(Dictionary<string, object> json)
            {
                return JsonConvert.SerializeObject(json);
            }

            /// <summary>
            /// Creates the specified json.
            /// </summary>
            /// <param name="json">The json.</param>
            /// <returns>System.String.</returns>
            public override string Create(IDictionary<string, object> json)
            {
                return JsonConvert.SerializeObject(json);
            }

            /// <summary>
            /// Creates the specified json.
            /// </summary>
            /// <param name="json">The json.</param>
            /// <returns>System.String.</returns>
            public override string Create(IReadOnlyDictionary<string, object> json)
            {
                return JsonConvert.SerializeObject(json);
            }
        }
    }
}