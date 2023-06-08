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

    /// <summary>
    /// Class Core2EnterpriseUserJsonDeserializingFactory. This class cannot be inherited. Implements the <see cref="Talegen.AspNetCore.Scim.Schema.JsonDeserializingFactory{Talegen.AspNetCore.Scim.Schema.Core2EnterpriseUser}" />
    /// </summary>
    /// <seealso cref="Talegen.AspNetCore.Scim.Schema.JsonDeserializingFactory{Talegen.AspNetCore.Scim.Schema.Core2EnterpriseUser}" />
    public sealed class Core2EnterpriseUserJsonDeserializingFactory : JsonDeserializingFactory<Core2EnterpriseUser>
    {
        /// <summary>
        /// The manager factory
        /// </summary>
        private static readonly Lazy<JsonDeserializingFactory<Manager>> ManagerFactory = new Lazy<JsonDeserializingFactory<Manager>>(() => new ManagerDeserializingFactory());

        /// <summary>
        /// Creates the specified json.
        /// </summary>
        /// <param name="json">The json.</param>
        /// <returns>Core2EnterpriseUser.</returns>
        /// <exception cref="System.ArgumentNullException">json</exception>
        /// <exception cref="System.NotSupportedException"></exception>
        public override Core2EnterpriseUser Create(IReadOnlyDictionary<string, object> json)
        {
            if (json == null)
            {
                throw new ArgumentNullException(nameof(json));
            }

            IReadOnlyDictionary<string, object> normalizedJson = this.Normalize(json);
            IReadOnlyDictionary<string, object> safeJson;

            if (normalizedJson.TryGetValue(AttributeNames.Manager, out object managerData))
            {
                safeJson = normalizedJson
                    .Where(item => !string.Equals(AttributeNames.Manager, item.Key, StringComparison.OrdinalIgnoreCase))
                    .ToDictionary(item => item.Key, item => item.Value);

                switch (managerData)
                {
                    case string value:
                        break;

                    case Dictionary<string, object> managerJson:
                        ManagerFactory.Value.Create(managerJson);
                        break;

                    default:
                        throw new NotSupportedException(managerData.GetType().FullName);
                }
            }
            else
            {
                safeJson = normalizedJson.ToDictionary(item => item.Key, item => item.Value);
            }

            Core2EnterpriseUser result = base.Create(safeJson);

            foreach (KeyValuePair<string, object> entry in json)
            {
                if (entry.Key.StartsWith(SchemaIdentifiers.PrefixExtension, StringComparison.OrdinalIgnoreCase) &&
                    !entry.Key.StartsWith(SchemaIdentifiers.Core2EnterpriseUser, StringComparison.OrdinalIgnoreCase) &&
                    entry.Value is Dictionary<string, object> nestedObject)
                {
                    result.AddCustomAttribute(entry.Key, nestedObject);
                }
            }

            return result;
        }

        /// <inheritdoc />
        private class ManagerDeserializingFactory : JsonDeserializingFactory<Manager>
        {
        }
    }
}