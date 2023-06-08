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
    using System.Runtime.Serialization;

    /// <summary>
    /// Class Feature. This class cannot be inherited. Implements the <see cref="Talegen.AspNetCore.Scim.Schema.FeatureBase" />
    /// </summary>
    /// <seealso cref="Talegen.AspNetCore.Scim.Schema.FeatureBase" />
    [DataContract]
    public sealed class Feature : FeatureBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Feature" /> class.
        /// </summary>
        /// <param name="supported">if set to <c>true</c> [supported].</param>
        public Feature(bool supported)
        {
            this.Supported = supported;
        }
    }
}