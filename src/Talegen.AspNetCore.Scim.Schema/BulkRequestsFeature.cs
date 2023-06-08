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
    /// Class BulkRequestsFeature. This class cannot be inherited. Implements the <see cref="Talegen.AspNetCore.Scim.Schema.FeatureBase" />
    /// </summary>
    /// <seealso cref="Talegen.AspNetCore.Scim.Schema.FeatureBase" />
    [DataContract]
    public sealed class BulkRequestsFeature : FeatureBase
    {
        /// <summary>
        /// Prevents a default instance of the <see cref="BulkRequestsFeature" /> class from being created.
        /// </summary>
        private BulkRequestsFeature()
        {
        }

        /// <summary>
        /// Gets the concurrent operations.
        /// </summary>
        /// <value>The concurrent operations.</value>
        public int ConcurrentOperations { get; }

        /// <summary>
        /// Gets the maximum operations.
        /// </summary>
        /// <value>The maximum operations.</value>
        [DataMember(Name = AttributeNames.MaximumOperations)]
        public int MaximumOperations { get; }

        /// <summary>
        /// Gets the maximum size of the payload.
        /// </summary>
        /// <value>The maximum size of the payload.</value>
        [DataMember(Name = AttributeNames.MaximumPayloadSize)]
        public int MaximumPayloadSize { get; }

        /// <summary>
        /// Creates the unsupported feature.
        /// </summary>
        /// <returns>BulkRequestsFeature.</returns>
        public static BulkRequestsFeature CreateUnsupportedFeature()
        {
            return new BulkRequestsFeature
            {
                Supported = false
            };
        }
    }
}