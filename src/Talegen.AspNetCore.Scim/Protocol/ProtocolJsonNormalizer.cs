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
    using System.Reflection;
    using System.Threading;
    using Schema;

    /// <summary>
    /// This class implements a JSON normalizer for SCIM protocol.
    /// </summary>
    public sealed class ProtocolJsonNormalizer : JsonNormalizerTemplate
    {
        /// <summary>
        /// Contains attribute names.
        /// </summary>
        private IReadOnlyCollection<string> attributeNames;

        /// <summary>
        /// Gets a collection of attribute names.
        /// </summary>
        public override IReadOnlyCollection<string> AttributeNames
        {
            get
            {
                IReadOnlyCollection<string> result =
                    LazyInitializer.EnsureInitialized<IReadOnlyCollection<string>>(ref this.attributeNames, ProtocolJsonNormalizer.CollectAttributeNames);
                return result;
            }
        }

        /// <summary>
        /// This method is used to collect attribute names.
        /// </summary>
        /// <returns>Returns a collection of attribute names.</returns>
        private static IReadOnlyCollection<string> CollectAttributeNames()
        {
            Type attributeNamesType = typeof(ProtocolAttributeNames);
            IReadOnlyCollection<FieldInfo> members = attributeNamesType.GetFields(BindingFlags.Public | BindingFlags.Static);
            IReadOnlyCollection<string> protocolAttributeNames =
                members.Select(item => item.GetValue(null))
                    .Cast<string>()
                    .ToArray();

            IReadOnlyCollection<string> result = new JsonNormalizer().AttributeNames.Union(protocolAttributeNames).ToArray();
            return result;
        }
    }
}