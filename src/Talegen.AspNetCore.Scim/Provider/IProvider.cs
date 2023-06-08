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
    using System.Threading.Tasks;
    using Protocol;
    using Schema;
    using Service;

    /// <summary>
    /// This interface defines the minimum implementation of a SCIM data provider.
    /// </summary>
    public interface IProvider
    {
        /// <summary>
        /// Contains a value indicating if the provider accepts large objects.
        /// </summary>
        bool AcceptLargeObjects { get; set; }

        /// <summary>
        /// Gets the service configuration.
        /// </summary>
        ServiceConfigurationBase Configuration { get; }

        ////IEventTokenHandler EventHandler { get; set; }

        /// <summary>
        /// Gets the extensions.
        /// </summary>
        IReadOnlyCollection<IExtension> Extensions { get; }

        /// <summary>
        /// Gets the group deserialization behavior.
        /// </summary>
        IResourceJsonDeserializingFactory<GroupBase> GroupDeserializationBehavior { get; }

        /// <summary>
        /// Gets the patch deserialization behavior.
        /// </summary>
        ISchematizedJsonDeserializingFactory<PatchRequest2> PatchRequestDeserializationBehavior { get; }

        /// <summary>
        /// Gets the User Deserialization behavior.
        /// </summary>
        IResourceJsonDeserializingFactory<Core2UserBase> UserDeserializationBehavior { get; }

        /// <summary>
        /// Gets supported resource types.
        /// </summary>
        IReadOnlyCollection<Core2ResourceType> ResourceTypes { get; }

        /// <summary>
        /// Gets the supported schema.
        /// </summary>
        IReadOnlyCollection<TypeScheme> Schema { get; }

        ////Action<IApplicationBuilder, HttpConfiguration> StartupBehavior { get; }

        /// <summary>
        /// This method is used to create a request asynchronously.
        /// </summary>
        /// <param name="request">Contains the request object.</param>
        /// <returns>Returns a new <see cref="Resource" /> object.</returns>
        Task<Resource> CreateAsync(IRequest<Resource> request);

        /// <summary>
        /// This method is used to delete a request asynchronously.
        /// </summary>
        /// <param name="request">Contains the request object.</param>
        /// <returns>Returns an async task object.</returns>
        Task DeleteAsync(IRequest<IResourceIdentifier> request);

        /// <summary>
        /// This method is used to execute a paginate query asynchronously.
        /// </summary>
        /// <param name="request">Contains the request object.</param>
        /// <returns>Returns a <see cref="QueryResponseBase" /> object.</returns>
        Task<QueryResponseBase> PaginateQueryAsync(IRequest<IQueryParameters> request);

        /// <summary>
        /// This method is used to execute a query asynchronously.
        /// </summary>
        /// <param name="request">Contains the request object.</param>
        /// <returns>Returns an array of <see cref="Resource" /> objects.</returns>
        Task<Resource[]> QueryAsync(IRequest<IQueryParameters> request);

        /// <summary>
        /// This method is used to replace a resource.
        /// </summary>
        /// <param name="request">Contains the request object.</param>
        /// <returns>Returns a <see cref="Resource" /> object.</returns>
        Task<Resource> ReplaceAsync(IRequest<Resource> request);

        /// <summary>
        /// This method is used to retrieve a resource.
        /// </summary>
        /// <param name="request">Contains the request object.</param>
        /// <returns>Returns a <see cref="Resource" /> object.</returns>
        Task<Resource> RetrieveAsync(IRequest<IResourceRetrievalParameters> request);

        /// <summary>
        /// This method is used to update a resource.
        /// </summary>
        /// <param name="request">Contains the request object.</param>
        /// <returns>Returns an async task object.</returns>
        Task UpdateAsync(IRequest<IPatch> request);

        /// <summary>
        /// This method is used to process a bulk request.
        /// </summary>
        /// <param name="request">Contains the request object.</param>
        /// <returns>Returns a <see cref="BulkResponse2" /> object.</returns>
        Task<BulkResponse2> ProcessAsync(IRequest<BulkRequest2> request);
    }
}