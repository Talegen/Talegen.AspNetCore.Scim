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
    using System.Runtime.Serialization;

    /// <summary>
    /// This class represents a SCIM v2 Patch request.
    /// </summary>
    [DataContract]
    public sealed class PatchRequest2 : PatchRequest2Base<PatchOperation2Combined>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PatchRequest2" /> class.
        /// </summary>
        public PatchRequest2()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PatchRequest2" /> class.
        /// </summary>
        /// <param name="operations">Contains a collection of patch operations.</param>
        public PatchRequest2(IReadOnlyCollection<PatchOperation2Combined> operations)
            : base(operations)
        {
        }
    }
}