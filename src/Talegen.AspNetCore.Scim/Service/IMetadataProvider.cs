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
    using Schema;

    /// <summary>
    /// This interface defines the minimum implementation of a metadata provider
    /// </summary>
    public interface IMetadataProvider
    {
        /// <summary>
        /// Gets the configuration.
        /// </summary>
        Core2ServiceConfiguration Configuration { get; }

        /// <summary>
        /// Gets a collection of resource types.
        /// </summary>
        IReadOnlyCollection<Core2ResourceType> ResourceTypes { get; }

        /// <summary>
        /// Gets a collection of schema.
        /// </summary>
        IReadOnlyCollection<TypeScheme> Schema { get; }
    }
}