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
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// This class contains dictionary extension helper methods.
    /// </summary>
    internal static class DictionaryExtension
    {
        /// <summary>
        /// This method is used to trim empty entries from the specified dictionary.
        /// </summary>
        /// <param name="dictionary">Contains the dictionary to trim.</param>
        public static void Trim(this IDictionary<string, object> dictionary)
        {
            IReadOnlyCollection<string> keys = dictionary.Keys.ToArray();

            foreach (string key in keys)
            {
                object value = dictionary[key];

                if (value == null)
                {
                    dictionary.Remove(key);
                }

                IDictionary<string, object> dictionaryValue = value as IDictionary<string, object>;

                if (dictionaryValue != null)
                {
                    dictionaryValue.Trim();

                    if (dictionaryValue.Count <= 0)
                    {
                        dictionary.Remove(key);
                    }
                }
            }
        }
    }
}