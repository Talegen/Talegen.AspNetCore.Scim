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
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Schema;

    /// <inheritdoc />
    public sealed class QueryResponseJsonDeserializingFactory<T> :
        ProtocolJsonDeserializingFactory<QueryResponse<T>>
        where T : Resource
    {
        /// <inheritdoc />
        public QueryResponseJsonDeserializingFactory(JsonDeserializingFactory<Schematized> jsonDeserializingFactory)
        {
            this.JsonDeserializingFactory = jsonDeserializingFactory ?? throw new ArgumentNullException(nameof(jsonDeserializingFactory));
        }

        /// <summary>
        /// Gets or sets the JSON deserialization factory.
        /// </summary>
        private JsonDeserializingFactory<Schematized> JsonDeserializingFactory { get; set; }

        /// <inheritdoc />
        public override QueryResponse<T> Create(IReadOnlyDictionary<string, object> json)
        {
            QueryResponse<T> result;

            if (json == null)
            {
                throw new ArgumentNullException(nameof(json));
            }

            if (!typeof(T).IsAbstract)
            {
                result = base.Create(json);
            }
            else
            {
                IReadOnlyDictionary<string, object> normalizedJson = this.Normalize(json);
                IReadOnlyDictionary<string, object> metadataJson =
                    normalizedJson
                    .Where(item => !string.Equals(ProtocolAttributeNames.Resources, item.Key, StringComparison.OrdinalIgnoreCase))
                    .ToDictionary(item => item.Key, item => item.Value);

                result = base.Create(metadataJson);

                IReadOnlyCollection<KeyValuePair<string, object>> resourcesJson =
                    normalizedJson.Where(item => string.Equals(ProtocolAttributeNames.Resources, item.Key, StringComparison.OrdinalIgnoreCase))
                    .ToArray();

                if (resourcesJson.Any())
                {
                    IEnumerable resourcesArray = (IEnumerable)resourcesJson.Single().Value;
                    List<T> resources = new List<T>(result.TotalResults);

                    foreach (object element in resourcesArray)
                    {
                        IReadOnlyDictionary<string, object> resourceJson = (IReadOnlyDictionary<string, object>)element;
                        T resource = (T)this.JsonDeserializingFactory.Create(resourceJson);
                        resources.Add(resource);
                    }

                    result.Resources = resources;
                }
            }

            return result;
        }
    }
}