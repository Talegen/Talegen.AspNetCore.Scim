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

namespace Talegen.AspNetCore.Scim.Service
{
    using Schema;
    using Talegen.AspNetCore.Scim.Provider;

    /// <summary>
    /// This class implements the Core Enterprise User Provider Adapter
    /// </summary>
    internal class Core2EnterpriseUserProviderAdapter : ProviderAdapterTemplate<Core2EnterpriseUser>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Core2EnterpriseUserProviderAdapter" /> class.
        /// </summary>
        /// <param name="provider">Contains the provider.</param>
        public Core2EnterpriseUserProviderAdapter(IProvider provider)
            : base(provider)
        {
        }

        /// <summary>
        /// Gets the current schema identifier.
        /// </summary>
        public override string SchemaIdentifier => SchemaIdentifiers.Core2EnterpriseUser;
    }
}