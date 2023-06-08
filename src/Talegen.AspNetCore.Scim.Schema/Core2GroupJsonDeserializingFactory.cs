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

    /// <summary>
    /// Class Core2GroupJsonDeserializingFactory. This class cannot be inherited. Implements the <see cref="Talegen.AspNetCore.Scim.Schema.JsonDeserializingFactory{Talegen.AspNetCore.Scim.Schema.Core2Group}" />
    /// </summary>
    /// <seealso cref="Talegen.AspNetCore.Scim.Schema.JsonDeserializingFactory{Talegen.AspNetCore.Scim.Schema.Core2Group}" />
    public sealed class Core2GroupJsonDeserializingFactory : JsonDeserializingFactory<Core2Group>
    {
        /// <summary>
        /// Creates the specified json.
        /// </summary>
        /// <param name="json">The json.</param>
        /// <returns>Core2Group.</returns>
        /// <exception cref="System.ArgumentNullException">json</exception>
        public override Core2Group Create(IReadOnlyDictionary<string, object> json)
        {
            if (json == null)
            {
                throw new ArgumentNullException(nameof(json));
            }

            Core2Group result = base.Create(json);

            foreach (KeyValuePair<string, object> entry in json)
            {
                if (entry.Key.StartsWith(SchemaIdentifiers.PrefixExtension, StringComparison.OrdinalIgnoreCase) && entry.Value is Dictionary<string, object> nestedObject)
                {
                    result.AddCustomAttribute(entry.Key, nestedObject);
                }
            }

            return result;
        }
    }
}