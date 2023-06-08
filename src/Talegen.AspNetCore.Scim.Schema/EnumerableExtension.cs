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

namespace Talegen.AspNetCore.Scim.Schema
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Class EnumerableExtension.
    /// </summary>
    internal static class EnumerableExtension
    {
        /// <summary>
        /// Checkeds the count.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumeration">The enumeration.</param>
        /// <returns>System.Int32.</returns>
        public static int CheckedCount<T>(this IEnumerable<T> enumeration)
        {
            long longCount = enumeration.LongCount();
            int result = Convert.ToInt32(longCount);
            return result;
        }
    }
}