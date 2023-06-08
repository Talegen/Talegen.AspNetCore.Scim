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
    using System.Reflection;
    using System.Threading;

    /// <summary>
    /// Class JsonNormalizer. This class cannot be inherited. Implements the <see cref="Talegen.AspNetCore.Scim.Schema.JsonNormalizerTemplate" />
    /// </summary>
    /// <seealso cref="Talegen.AspNetCore.Scim.Schema.JsonNormalizerTemplate" />
    public sealed class JsonNormalizer : JsonNormalizerTemplate
    {
        /// <summary>
        /// The attribute names
        /// </summary>
        private IReadOnlyCollection<string> attributeNames;

        /// <summary>
        /// Gets the attribute names.
        /// </summary>
        /// <value>The attribute names.</value>
        public override IReadOnlyCollection<string> AttributeNames
        {
            get
            {
                IReadOnlyCollection<string> result = LazyInitializer.EnsureInitialized<IReadOnlyCollection<string>>(ref this.attributeNames, CollectAttributeNames);
                return result;
            }
        }

        /// <summary>
        /// Collects the attribute names.
        /// </summary>
        /// <returns>IReadOnlyCollection&lt;System.String&gt;.</returns>
        private static IReadOnlyCollection<string> CollectAttributeNames()
        {
            Type attributeNamesType = typeof(AttributeNames);
            IReadOnlyCollection<FieldInfo> members = attributeNamesType.GetFields(BindingFlags.Public | BindingFlags.Static);
            IReadOnlyCollection<string> result = members.Select((FieldInfo item) => item.GetValue(null))
                .Cast<string>()
                .ToArray();
            return result;
        }
    }
}