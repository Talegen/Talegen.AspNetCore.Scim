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
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Protocol;
    using Schema;

    /// <summary>
    /// This interface defines the minimum implementation of a provider adapter class.
    /// </summary>
    /// <typeparam name="T">Contains the object type for the adapter.</typeparam>
    public interface IProviderAdapter<T> where T : Resource
    {
        /// <summary>
        /// Gets the schema identifier.
        /// </summary>
        string SchemaIdentifier { get; }

        /// <summary>
        /// This method is used to create a resource asynchronously.
        /// </summary>
        /// <param name="request">Contains the request message.</param>
        /// <param name="resource">Contains the replacement resource.</param>
        /// <param name="correlationIdentifier">Contains a correlation identifier.</param>
        /// <returns>Returns a <see cref="Resource" /> object.</returns>
        Task<Resource> Create(HttpRequestMessage request, Resource resource, string correlationIdentifier);

        /// <summary>
        /// This method is used to delete a resource asynchronously.
        /// </summary>
        /// <param name="request">Contains the request message.</param>
        /// <param name="identifier">Contains the identifier.</param>
        /// <param name="correlationIdentifier">Contains a correlation identifier.</param>
        /// <returns>Returns an async task object.</returns>
        Task Delete(HttpRequestMessage request, string identifier, string correlationIdentifier);

        /// <summary>
        /// This method is used query resources asynchronously.
        /// </summary>
        /// <param name="request">Contains the request message.</param>
        /// <param name="filters">Contains a collection of filters.</param>
        /// <param name="requestedAttributePaths">Contains requested attribute paths.</param>
        /// <param name="excludedAttributePaths">Contains excluded attribute paths.</param>
        /// <param name="paginationParameters">Contains pagination parameters.</param>
        /// <param name="correlationIdentifier">Contains a correlation identifier.</param>
        /// <returns>Returns a <see cref="QueryResponseBase" /> object.</returns>
        Task<QueryResponseBase> Query(
            HttpRequestMessage request,
            IReadOnlyCollection<IFilter> filters,
            IReadOnlyCollection<string> requestedAttributePaths,
            IReadOnlyCollection<string> excludedAttributePaths,
            IPaginationParameters paginationParameters,
            string correlationIdentifier);

        /// <summary>
        /// This method is used to replace a resource asynchronously.
        /// </summary>
        /// <param name="request">Contains the request message.</param>
        /// <param name="resource">Contains the replacement resource.</param>
        /// <param name="correlationIdentifier">Contains a correlation identifier.</param>
        /// <returns>Returns a <see cref="Resource" /> object.</returns>
        Task<Resource> Replace(HttpRequestMessage request, Resource resource, string correlationIdentifier);

        /// <summary>
        /// This method is used to retrieve a resource asynchronously.
        /// </summary>
        /// <param name="request">Contains the request message.</param>
        /// <param name="identifier">Contains the identifier.</param>
        /// <param name="requestedAttributePaths">Contains requested attribute paths.</param>
        /// <param name="excludedAttributePaths">Contains excluded attribute paths.</param>
        /// <param name="correlationIdentifier">Contains a correlation identifier.</param>
        /// <returns>Returns a <see cref="Resource" /> object.</returns>
        Task<Resource> Retrieve(
            HttpRequestMessage request,
            string identifier,
            IReadOnlyCollection<string> requestedAttributePaths,
            IReadOnlyCollection<string> excludedAttributePaths,
            string correlationIdentifier);

        /// <summary>
        /// This method is used to update a resource asynchronously.
        /// </summary>
        /// <param name="request">Contains the request message.</param>
        /// <param name="identifier">Contains the identifier.</param>
        /// <param name="patchRequest">Contains the patch request.</param>
        /// <param name="correlationIdentifier">Contains a correlation identifier.</param>
        /// <returns>Returns an async task object.</returns>
        Task Update(
            HttpRequestMessage request,
            string identifier,
            PatchRequestBase patchRequest,
            string correlationIdentifier);
    }
}