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
    using System.Collections.Generic;
    using System.Net.Http;
    using Protocol;
    using Schema;

    /// <summary>
    /// This class implements a replace request.
    /// </summary>
    public sealed class ReplaceRequest : SystemForCrossDomainIdentityManagementRequest<Resource>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReplaceRequest" /> class.
        /// </summary>
        /// <param name="request">Contains the HTTP request message.</param>
        /// <param name="payload">Contains the payload.</param>
        /// <param name="correlationIdentifier">Contains a correlation identifier.</param>
        /// <param name="extensions">Contains a collection of extensions.</param>
        public ReplaceRequest(
            HttpRequestMessage request,
            Resource payload,
            string correlationIdentifier,
            IReadOnlyCollection<IExtension> extensions)
            : base(request, payload, correlationIdentifier, extensions)
        {
        }
    }
}