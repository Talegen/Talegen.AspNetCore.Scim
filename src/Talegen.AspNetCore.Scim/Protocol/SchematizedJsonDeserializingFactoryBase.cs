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
    using Schema;

    /// <summary>
    /// This abstract class implements a schematized JSON deserialization factory.
    /// </summary>
    public abstract class SchematizedJsonDeserializingFactoryBase : ProtocolJsonDeserializingFactory, ISchematizedJsonDeserializingFactory
    {
        /// <summary>
        /// Gets or sets a collection of extensions.
        /// </summary>
        public abstract IReadOnlyCollection<IExtension> Extensions { get; set; }

        /// <summary>
        /// Gets or sets the Group deserialization behavior.
        /// </summary>
        public abstract IResourceJsonDeserializingFactory<GroupBase> GroupDeserializationBehavior { get; set; }

        /// <summary>
        /// Gets or sets the Patch request deserialization behavior.
        /// </summary>
        public abstract ISchematizedJsonDeserializingFactory<PatchRequest2> PatchRequest2DeserializationBehavior { get; set; }

        /// <summary>
        /// Gets or sets the User deserialization behavior.
        /// </summary>
        public abstract IResourceJsonDeserializingFactory<Core2UserBase> UserDeserializationBehavior { get; set; }
    }
}