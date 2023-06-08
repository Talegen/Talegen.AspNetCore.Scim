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
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// Class GroupBase. Implements the <see cref="Talegen.AspNetCore.Scim.Schema.Resource" />
    /// </summary>
    /// <seealso cref="Talegen.AspNetCore.Scim.Schema.Resource" />
    [DataContract]
    public abstract class GroupBase : Resource
    {
        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        /// <value>The display name.</value>
        [DataMember(Name = AttributeNames.DisplayName)]
        public virtual string DisplayName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the members.
        /// </summary>
        /// <value>The members.</value>
        [DataMember(Name = AttributeNames.Members, IsRequired = false, EmitDefaultValue = false)]
        public virtual IEnumerable<Member> Members
        {
            get;
            set;
        }
    }
}