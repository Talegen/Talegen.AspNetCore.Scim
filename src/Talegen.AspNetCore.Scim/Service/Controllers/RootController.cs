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

namespace Talegen.AspNetCore.Scim.Service.Controllers
{
    using System;
    using Monitor;
    using Schema;
    using Talegen.AspNetCore.Scim.Provider;

    /// <summary>
    /// This controller is used to handle SCIM root requests.
    /// </summary>
    public sealed class RootController : ControllerTemplate<Resource>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RootController" /> class.
        /// </summary>
        /// <param name="provider">Contains an instance of the SCIM provider repository.</param>
        /// <param name="monitor">Contains an instance of the SCIM monitor plugin.</param>
        public RootController(IProvider provider, IMonitor monitor)
            : base(provider, monitor)
        {
        }

        /// <inheritdoc />
        protected override IProviderAdapter<Resource> AdaptProvider(IProvider provider)
        {
            if (provider == null)
            {
                throw new ArgumentNullException(nameof(provider));
            }

            IProviderAdapter<Resource> result = new RootProviderAdapter(provider);
            return result;
        }
    }
}