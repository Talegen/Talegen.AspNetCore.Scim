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

namespace Talegen.AspNetCore.Scim.Provider
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Protocol;
    using Schema;
    using Service;

    /// <summary>
    /// This class implements a basic provider adapter template for a given object type.
    /// </summary>
    /// <typeparam name="T">Contains the object type.</typeparam>
    public abstract class ProviderAdapterTemplate<T> : IProviderAdapter<T> where T : Resource
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProviderAdapterTemplate{T}" /> class.
        /// </summary>
        /// <param name="provider">Contains the provider.</param>
        /// <exception cref="ArgumentNullException">Exception is thrown if a provider is not specified.</exception>
        protected ProviderAdapterTemplate(IProvider provider)
        {
            this.Provider = provider ?? throw new ArgumentNullException(nameof(provider));
        }

        /// <summary>
        /// Gets or sets the provider.
        /// </summary>
        public IProvider Provider { get; set; }

        /// <inheritdoc />
        public abstract string SchemaIdentifier { get; }

        /// <summary>
        /// This method is used to create a resource identifier.
        /// </summary>
        /// <param name="identifier">Contains the identifier.</param>
        /// <returns>Returns a new resource identifier.</returns>
        /// <exception cref="ArgumentNullException">Exception is thrown if an identifier is not specified.</exception>
        public virtual IResourceIdentifier CreateResourceIdentifier(string identifier)
        {
            if (string.IsNullOrWhiteSpace(identifier))
            {
                throw new ArgumentNullException(nameof(identifier));
            }

            IResourceIdentifier result = new ResourceIdentifier
            {
                Identifier = identifier,
                SchemaIdentifier = this.SchemaIdentifier
            };

            return result;
        }

        /// <summary>
        /// This method is used to get a schema identifier path from the specified request message.
        /// </summary>
        /// <param name="request">Contains the request message.</param>
        /// <returns>Returns the schema identifier path.</returns>
        public virtual string GetPath(HttpRequestMessage request)
        {
            IReadOnlyCollection<IExtension> extensions = this.ReadExtensions();

            if (extensions == null || !extensions.TryGetPath(this.SchemaIdentifier, out string result))
            {
                result = new SchemaIdentifier(this.SchemaIdentifier).FindPath();
            }

            return result;
        }

        /// <inheritdoc />
        public virtual async Task<Resource> Create(HttpRequestMessage request, Resource resource, string correlationIdentifier)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (resource == null)
            {
                throw new ArgumentNullException(nameof(resource));
            }

            if (string.IsNullOrWhiteSpace(correlationIdentifier))
            {
                throw new ArgumentNullException(nameof(correlationIdentifier));
            }

            IReadOnlyCollection<IExtension> extensions = this.ReadExtensions();
            IRequest<Resource> creationRequest = new CreationRequest(request, resource, correlationIdentifier, extensions);
            Resource result = await this.Provider.CreateAsync(creationRequest).ConfigureAwait(false);
            return result;
        }

        /// <inheritdoc />
        public virtual async Task Delete(HttpRequestMessage request, string identifier, string correlationIdentifier)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (string.IsNullOrWhiteSpace(identifier))
            {
                throw new ArgumentNullException(nameof(identifier));
            }

            if (string.IsNullOrWhiteSpace(correlationIdentifier))
            {
                throw new ArgumentNullException(nameof(correlationIdentifier));
            }

            IReadOnlyCollection<IExtension> extensions = this.ReadExtensions();
            IResourceIdentifier resourceIdentifier = this.CreateResourceIdentifier(identifier);
            IRequest<IResourceIdentifier> deletionRequest = new DeletionRequest(request, resourceIdentifier, correlationIdentifier, extensions);
            await this.Provider.DeleteAsync(deletionRequest).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public virtual async Task<QueryResponseBase> Query(
            HttpRequestMessage request,
            IReadOnlyCollection<IFilter> filters,
            IReadOnlyCollection<string> requestedAttributePaths,
            IReadOnlyCollection<string> excludedAttributePaths,
            IPaginationParameters paginationParameters,
            string correlationIdentifier)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (requestedAttributePaths == null)
            {
                throw new ArgumentNullException(nameof(requestedAttributePaths));
            }

            if (excludedAttributePaths == null)
            {
                throw new ArgumentNullException(nameof(excludedAttributePaths));
            }

            if (string.IsNullOrWhiteSpace(correlationIdentifier))
            {
                throw new ArgumentNullException(nameof(correlationIdentifier));
            }

            string path = this.GetPath(request);
            IQueryParameters queryParameters = new QueryParameters(this.SchemaIdentifier, path, filters, requestedAttributePaths, excludedAttributePaths);
            queryParameters.PaginationParameters = paginationParameters;
            IReadOnlyCollection<IExtension> extensions = this.ReadExtensions();
            IRequest<IQueryParameters> queryRequest = new QueryRequest(request, queryParameters, correlationIdentifier, extensions);
            QueryResponseBase result = await this.Provider.PaginateQueryAsync(queryRequest).ConfigureAwait(false);

            return result;
        }

        /// <inheritdoc />
        public virtual async Task<Resource> Replace(
            HttpRequestMessage request,
            Resource resource,
            string correlationIdentifier)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (resource == null)
            {
                throw new ArgumentNullException(nameof(resource));
            }

            if (string.IsNullOrWhiteSpace(correlationIdentifier))
            {
                throw new ArgumentNullException(nameof(correlationIdentifier));
            }

            IReadOnlyCollection<IExtension> extensions = this.ReadExtensions();
            IRequest<Resource> replaceRequest = new ReplaceRequest(request, resource, correlationIdentifier, extensions);
            Resource result = await this.Provider.ReplaceAsync(replaceRequest).ConfigureAwait(false);
            return result;
        }

        /// <inheritdoc />
        public virtual async Task<Resource> Retrieve(
            HttpRequestMessage request,
            string identifier,
            IReadOnlyCollection<string> requestedAttributePaths,
            IReadOnlyCollection<string> excludedAttributePaths,
            string correlationIdentifier)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (string.IsNullOrWhiteSpace(identifier))
            {
                throw new ArgumentNullException(nameof(identifier));
            }

            if (null == requestedAttributePaths)
            {
                throw new ArgumentNullException(nameof(requestedAttributePaths));
            }

            if (string.IsNullOrWhiteSpace(correlationIdentifier))
            {
                throw new ArgumentNullException(nameof(correlationIdentifier));
            }

            string path = this.GetPath(request);
            IResourceRetrievalParameters retrievalParameters = new ResourceRetrievalParameters(this.SchemaIdentifier, path, identifier, requestedAttributePaths, excludedAttributePaths);
            IReadOnlyCollection<IExtension> extensions = this.ReadExtensions();
            IRequest<IResourceRetrievalParameters> retrievalRequest = new RetrievalRequest(request, retrievalParameters, correlationIdentifier, extensions);
            Resource result = await this.Provider.RetrieveAsync(retrievalRequest).ConfigureAwait(false);
            return result;
        }

        /// <inheritdoc />
        public virtual async Task Update(
            HttpRequestMessage request,
            string identifier,
            PatchRequestBase patchRequest,
            string correlationIdentifier)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (string.IsNullOrWhiteSpace(correlationIdentifier))
            {
                throw new ArgumentNullException(nameof(correlationIdentifier));
            }

            IResourceIdentifier resourceIdentifier = this.CreateResourceIdentifier(identifier);
            IPatch patch = new Patch
            {
                ResourceIdentifier = resourceIdentifier,
                PatchRequest = patchRequest
            };

            IReadOnlyCollection<IExtension> extensions = this.ReadExtensions();
            IRequest<IPatch> updateRequest = new UpdateRequest(request, patch, correlationIdentifier, extensions);
            await this.Provider.UpdateAsync(updateRequest).ConfigureAwait(false);
        }

        /// <summary>
        /// This method is used to read extensions.
        /// </summary>
        /// <returns>Returns a collection of extensions read.</returns>
        private IReadOnlyCollection<IExtension> ReadExtensions()
        {
            IReadOnlyCollection<IExtension> result;

            try
            {
                result = this.Provider.Extensions;
            }
            catch (NotImplementedException)
            {
                result = null;
            }

            return result;
        }
    }
}