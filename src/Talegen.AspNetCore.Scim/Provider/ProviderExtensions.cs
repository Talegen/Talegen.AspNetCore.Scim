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
    using Protocol;

    /// <summary>
    /// This class contains provider extension methods.
    /// </summary>
    public static class ProviderExtension
    {
        /// <summary>
        /// This method is used to read extensions for the specified provider.
        /// </summary>
        /// <param name="provider">Contains the provider to read extensions.</param>
        /// <returns>Returns a collection of extensions from the provider.</returns>
        /// <exception cref="ArgumentNullException">Exception is thrown if a provider is not specified.</exception>
        public static IReadOnlyCollection<IExtension> ReadExtensions(this IProvider provider)
        {
            if (provider == null)
            {
                throw new ArgumentNullException(nameof(provider));
            }

            IReadOnlyCollection<IExtension> result;

            try
            {
                result = provider.Extensions;
            }
            catch (NotImplementedException)
            {
                result = null;
            }

            return result;
        }
    }
}