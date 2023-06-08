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
    using System.Runtime.Serialization;

    /// <summary>
    /// This class represents a bulk request of operations
    /// </summary>
    [DataContract]
    public sealed class BulkRequest2 : BulkOperations<BulkRequestOperation>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BulkRequest2" /> class.
        /// </summary>
        public BulkRequest2()
            : base(ProtocolSchemaIdentifiers.Version2BulkRequest)
        {
        }

        /// <summary>
        /// Gets or sets the fail on errors value.
        /// </summary>
        [DataMember(Name = ProtocolAttributeNames.FailOnErrors, Order = 1)]
        public int? FailOnErrors { get; set; }
    }
}