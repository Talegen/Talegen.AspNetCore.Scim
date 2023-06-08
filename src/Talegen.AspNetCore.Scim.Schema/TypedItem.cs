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
    /// This abstract class contains the base for all schema type model classes.
    /// </summary>
    [DataContract]
    public abstract class TypedItem
    {
        /// <summary>
        /// Gets or sets the item type.
        /// </summary>
        /// <value>The type of the item.</value>
        [DataMember(Name = AttributeNames.Type)]
        public string ItemType { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the type is primary.
        /// </summary>
        /// <value><c>true</c> if primary; otherwise, <c>false</c>.</value>
        [DataMember(Name = AttributeNames.Primary, IsRequired = false)]
        public bool Primary { get; set; }
    }
}