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
    using Protocol;

    /// <summary>
    /// This class implements a patch request.
    /// </summary>
    public sealed class Patch : IPatch
    {
        /// <summary>
        /// Initializes a new instance of a <see cref="Patch" /> class.
        /// </summary>
        public Patch()
        {
        }

        /// <summary>
        /// Initializes a new instance of a <see cref="Patch" /> class.
        /// </summary>
        /// <param name="resourceIdentifier">Contains a resource identifier.</param>
        /// <param name="request">Contains a patch request.</param>
        /// <exception cref="ArgumentNullException">Exception is thrown if the parameters are not specified.</exception>
        public Patch(IResourceIdentifier resourceIdentifier, PatchRequestBase request)
        {
            this.ResourceIdentifier = resourceIdentifier ?? throw new ArgumentNullException(nameof(resourceIdentifier));
            this.PatchRequest = request ?? throw new ArgumentNullException(nameof(request));
        }

        /// <summary>
        /// Gets or sets the patch request.
        /// </summary>
        public PatchRequestBase PatchRequest { get; set; }

        /// <summary>
        /// Gets or sets the resource identifier.
        /// </summary>
        public IResourceIdentifier ResourceIdentifier { get; set; }
    }
}