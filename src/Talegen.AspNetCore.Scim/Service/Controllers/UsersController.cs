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
    using System;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Monitor;
    using Schema;
    using Talegen.AspNetCore.Scim.Provider;

    /// <summary>
    /// This controller is used to handle SCIM User requests.
    /// </summary>
    [Route(ServiceConstants.RouteUsers)]
    [Authorize]
    [ApiController]
    public sealed class UsersController : ControllerTemplate<Core2EnterpriseUser>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UsersController" /> class.
        /// </summary>
        /// <param name="provider">Contains an instance of the SCIM provider repository.</param>
        /// <param name="monitor">Contains an instance of the SCIM monitor plugin.</param>
        public UsersController(IProvider provider, IMonitor monitor)
            : base(provider, monitor)
        {
        }

        /// <inheritdoc />
        protected override IProviderAdapter<Core2EnterpriseUser> AdaptProvider(IProvider provider)
        {
            if (provider == null)
            {
                throw new ArgumentNullException(nameof(provider));
            }

            IProviderAdapter<Core2EnterpriseUser> result = new Core2EnterpriseUserProviderAdapter(provider);
            return result;
        }
    }
}