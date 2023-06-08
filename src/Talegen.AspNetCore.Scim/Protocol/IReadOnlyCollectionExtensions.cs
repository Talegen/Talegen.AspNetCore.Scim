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

namespace Talegen.AspNetCore.Scim.Protocol
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web;

    /// <summary>
    /// This class contains extension methods for the <see cref="IReadOnlyCollection{T}" /> class.
    /// </summary>
    public static class IReadOnlyCollectionExtensions
    {
        /// <summary>
        /// This method is used to encode a collection of path strings into a URL encoded strings.
        /// </summary>
        /// <param name="collection">Contains the collection to encode.</param>
        /// <returns>Returns an encoded version of the passed collection.</returns>
        /// <exception cref="ArgumentNullException">Exception is thrown if the collection is not specified.</exception>
        public static IReadOnlyCollection<string> Encode(this IReadOnlyCollection<string> collection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }

            return collection.Select(HttpUtility.UrlEncode).ToArray()!;
        }

        /// <summary>
        /// This method is used to try and find a path within a specified collection of extensions by schema identifier.
        /// </summary>
        /// <param name="extensions">Contains the extensions collection to search.</param>
        /// <param name="schemaIdentifier">Contains the schema identifier to find.</param>
        /// <param name="path">Contains the path found.</param>
        /// <returns>Returns a value indicating whether the path was found.</returns>
        /// <exception cref="ArgumentNullException">Exception is thrown if the schema identifier was not specified.</exception>
        public static bool TryGetPath(this IReadOnlyCollection<IExtension> extensions, string schemaIdentifier, out string path)
        {
            bool result = false;

            if (string.IsNullOrWhiteSpace(schemaIdentifier))
            {
                throw new ArgumentNullException(nameof(schemaIdentifier));
            }

            path = null;

            IExtension extension = extensions.SingleOrDefault(item => string.Equals(schemaIdentifier, item.SchemaIdentifier, StringComparison.OrdinalIgnoreCase));

            if (extension != null)
            {
                path = extension.Path;
                result = true;
            }

            return result;
        }
    }
}