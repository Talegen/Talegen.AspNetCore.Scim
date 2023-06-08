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
    /// Class MemberBase.
    /// </summary>
    [DataContract]
    public abstract class MemberBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MemberBase" /> class.
        /// </summary>
        internal MemberBase()
        {
        }

        /// <summary>
        /// Gets or sets the name of the type.
        /// </summary>
        /// <value>The name of the type.</value>
        [DataMember(Name = AttributeNames.Type, IsRequired = false, EmitDefaultValue = false)]
        public string TypeName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>The value.</value>
        [DataMember(Name = AttributeNames.Value)]
        public string Value
        {
            get;
            set;
        }
    }
}