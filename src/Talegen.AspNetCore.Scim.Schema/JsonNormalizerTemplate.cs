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
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    /// <summary>
    /// Class JsonNormalizerTemplate. Implements the <see cref="Talegen.AspNetCore.Scim.Schema.IJsonNormalizationBehavior" />
    /// </summary>
    /// <seealso cref="Talegen.AspNetCore.Scim.Schema.IJsonNormalizationBehavior" />
    public abstract class JsonNormalizerTemplate : IJsonNormalizationBehavior
    {
        /// <summary>
        /// Gets the attribute names.
        /// </summary>
        /// <value>The attribute names.</value>
        public abstract IReadOnlyCollection<string> AttributeNames
        {
            get;
        }

        /// <summary>
        /// Normalizes the specified json.
        /// </summary>
        /// <param name="json">The json.</param>
        /// <returns>IEnumerable&lt;KeyValuePair&lt;System.String, System.Object&gt;&gt;.</returns>
        /// <exception cref="System.ArgumentNullException">json</exception>
        private IEnumerable<KeyValuePair<string, object>> Normalize(IReadOnlyCollection<KeyValuePair<string, object>> json)
        {
            if (null == json)
            {
                throw new ArgumentNullException(nameof(json));
            }

            int countElements = json.CheckedCount();
            IDictionary<string, object> result = new Dictionary<string, object>(countElements);
            foreach (KeyValuePair<string, object> element in json)
            {
                string key;
                key = element.Key;
                object value = element.Value;
                string attributeName = this.AttributeNames.SingleOrDefault((string item) => string.Equals(item, key, StringComparison.OrdinalIgnoreCase));

                if (attributeName != null)
                {
                    if (!string.Equals(key, attributeName, StringComparison.Ordinal))
                    {
                        key = attributeName;
                    }

                    switch (value)
                    {
                        case IEnumerable<KeyValuePair<string, object>> jsonValue:
                            value = this.Normalize(jsonValue);
                            break;

                        case ArrayList jsonCollectionValue:
                            ArrayList jsonCollectionNormalized = new ArrayList();
                            foreach (object innerValue in jsonCollectionValue)
                            {
                                IEnumerable<KeyValuePair<string, object>> innerObject = innerValue as IEnumerable<KeyValuePair<string, object>>;

                                if (innerObject != null)
                                {
                                    IEnumerable<KeyValuePair<string, object>> normalizedInnerObject = this.Normalize(innerObject);
                                    jsonCollectionNormalized.Add(normalizedInnerObject);
                                }
                                else
                                {
                                    jsonCollectionNormalized.Add(innerValue);
                                }
                            }

                            value = jsonCollectionNormalized;
                            break;

                        default:
                            break;
                    }
                }

                result.Add(key, value);
            }

            return result;
        }

        /// <summary>
        /// Normalizes the specified json.
        /// </summary>
        /// <param name="json">The json.</param>
        /// <returns>IEnumerable&lt;KeyValuePair&lt;System.String, System.Object&gt;&gt;.</returns>
        /// <exception cref="System.ArgumentNullException">json</exception>
        private IEnumerable<KeyValuePair<string, object>> Normalize(IEnumerable<KeyValuePair<string, object>> json)
        {
            if (null == json)
            {
                throw new ArgumentNullException(nameof(json));
            }

            IReadOnlyCollection<KeyValuePair<string, object>> materializedJson = json.ToArray();
            IEnumerable<KeyValuePair<string, object>> result = this.Normalize(materializedJson);
            return result;
        }

        /// <summary>
        /// Normalizes the specified json.
        /// </summary>
        /// <param name="json">The json.</param>
        /// <returns>IReadOnlyDictionary&lt;System.String, System.Object&gt;.</returns>
        /// <exception cref="System.ArgumentNullException">json</exception>
        public IReadOnlyDictionary<string, object> Normalize(IReadOnlyDictionary<string, object> json)
        {
            if (null == json)
            {
                throw new ArgumentNullException(nameof(json));
            }

            IReadOnlyCollection<KeyValuePair<string, object>> keyedPairs = (IReadOnlyCollection<KeyValuePair<string, object>>)json;

            Dictionary<string, object> normalizedJson = this.Normalize(keyedPairs)
                .ToDictionary((KeyValuePair<string, object> item) => item.Key, (KeyValuePair<string, object> item) => item.Value);
            IReadOnlyDictionary<string, object> result = new ReadOnlyDictionary<string, object>(normalizedJson);
            return result;
        }
    }
}