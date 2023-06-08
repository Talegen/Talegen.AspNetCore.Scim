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
    /// Class RoleBase. Implements the <see cref="Talegen.AspNetCore.Scim.Schema.TypedItem" />
    /// </summary>
    /// <seealso cref="Talegen.AspNetCore.Scim.Schema.TypedItem" />
    [DataContract]
    public abstract class RoleBase : TypedItem
    {
        /// <summary>
        /// Gets or sets the display.
        /// </summary>
        /// <value>The display.</value>
        [DataMember(Name = AttributeNames.Display, IsRequired = false, EmitDefaultValue = false)]
        public string Display
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        [DataMember(Name = AttributeNames.Value, IsRequired = false, EmitDefaultValue = false)]
        public string Value
        {
            get;
            set;
        }
    }
}