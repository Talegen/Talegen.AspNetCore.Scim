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
    using System.Runtime.Serialization;

    /// <summary>
    /// Class Core2Metadata. This class cannot be inherited.
    /// </summary>
    [DataContract]
    public sealed class Core2Metadata
    {
        /// <summary>
        /// Gets or sets the type of the resource.
        /// </summary>
        /// <value>The type of the resource.</value>
        [DataMember(Name = AttributeNames.ResourceType, Order = 0)]
        public string ResourceType
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the created.
        /// </summary>
        /// <value>The created.</value>
        [DataMember(Name = AttributeNames.Created, Order = 1)]
        public DateTime Created
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the last modified.
        /// </summary>
        /// <value>The last modified.</value>
        [DataMember(Name = AttributeNames.LastModified, Order = 2)]
        public DateTime LastModified
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        /// <value>The version.</value>
        [DataMember(Name = AttributeNames.Version, Order = 3)]
        public string Version
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the location.
        /// </summary>
        /// <value>The location.</value>
        [DataMember(Name = AttributeNames.Location, Order = 4)]
        public string Location
        {
            get;
            set;
        }
    }
}