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
    using System.Linq;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// This class contains extension methods for generic objects.
    /// </summary>
    public static class ObjectExtensions
    {
        /// <summary>
        /// This extension method is used to determine if the object is a resource for the specified scheme.
        /// </summary>
        /// <param name="json">Contains the JSON object to evaluate.</param>
        /// <param name="scheme">Contains the scheme name of the resource type to validate.</param>
        /// <returns>Returns a value indicating whether the object is of the specified resource type.</returns>
        /// <exception cref="ArgumentNullException">Exception is thrown if the object or scheme are not specified.</exception>
        public static bool IsResourceType(this object json, string scheme)
        {
            if (json == null)
            {
                throw new ArgumentNullException(nameof(json));
            }

            if (string.IsNullOrWhiteSpace(scheme))
            {
                throw new ArgumentNullException(nameof(scheme));
            }

            dynamic operationDataJson = JsonConvert.DeserializeObject(json.ToString());
            bool result = false;

            switch (operationDataJson?.schemas)
            {
                case JArray schemas:
                    string[] schemasList = schemas.ToObject<string[]>();
                    result = schemasList.Any(item => string.Equals(item, scheme, StringComparison.OrdinalIgnoreCase));
                    break;

                default:
                    break;
            }

            return result;
        }
    }
}