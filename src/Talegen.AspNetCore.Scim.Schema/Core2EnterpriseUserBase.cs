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
    /// Class Core2EnterpriseUserBase. Implements the <see cref="Talegen.AspNetCore.Scim.Schema.Core2UserBase" />
    /// </summary>
    /// <seealso cref="Talegen.AspNetCore.Scim.Schema.Core2UserBase" />
    [DataContract]
    public abstract class Core2EnterpriseUserBase : Core2UserBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Core2EnterpriseUserBase" /> class.
        /// </summary>
        protected Core2EnterpriseUserBase()
            : base()
        {
            this.AddSchema(SchemaIdentifiers.Core2EnterpriseUser);
            this.EnterpriseExtension = new ExtensionAttributeEnterpriseUser2();
        }

        /// <summary>
        /// Gets or sets the enterprise extension.
        /// </summary>
        /// <value>The enterprise extension.</value>
        [DataMember(Name = AttributeNames.ExtensionEnterpriseUser2)]
        public ExtensionAttributeEnterpriseUser2 EnterpriseExtension { get; set; }
    }
}