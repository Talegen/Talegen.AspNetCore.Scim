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
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Protocol;
    using Schema;
    using Talegen.AspNetCore.Scim.Provider;

    /// <summary>
    /// This class implements a root provider adapter.
    /// </summary>
    internal class RootProviderAdapter : ProviderAdapterTemplate<Resource>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RootProviderAdapter" /> class.
        /// </summary>
        /// <param name="provider">Contains a provider.</param>
        public RootProviderAdapter(IProvider provider)
            : base(provider)
        {
        }

        /// <inheritdoc />
        public override string SchemaIdentifier => SchemaIdentifiers.None;

        /// <inheritdoc />
        public override Task<Resource> Create(
            HttpRequestMessage request,
            Resource resource,
            string correlationIdentifier)
        {
            throw new HttpResponseException(HttpStatusCode.NotImplemented);
        }

        /// <inheritdoc />
        public override IResourceIdentifier CreateResourceIdentifier(string identifier)
        {
            throw new HttpResponseException(HttpStatusCode.NotImplemented);
        }

        /// <inheritdoc />
        public override Task Delete(
            HttpRequestMessage request,
            string identifier,
            string correlationIdentifier)
        {
            throw new HttpResponseException(HttpStatusCode.NotImplemented);
        }

        /// <inheritdoc />
        public override Task<Resource> Replace(
            HttpRequestMessage request,
            Resource resource, string
            correlationIdentifier)
        {
            throw new HttpResponseException(HttpStatusCode.NotImplemented);
        }

        /// <inheritdoc />
        public override Task<Resource> Retrieve(
            HttpRequestMessage request,
            string identifier,
            IReadOnlyCollection<string> requestedAttributePaths,
            IReadOnlyCollection<string> excludedAttributePaths,
            string correlationIdentifier)
        {
            throw new HttpResponseException(HttpStatusCode.NotImplemented);
        }

        /// <inheritdoc />
        public override Task Update(
            HttpRequestMessage request,
            string identifier,
            PatchRequestBase patchRequest,
            string correlationIdentifier)
        {
            throw new HttpResponseException(HttpStatusCode.NotImplemented);
        }
    }
}