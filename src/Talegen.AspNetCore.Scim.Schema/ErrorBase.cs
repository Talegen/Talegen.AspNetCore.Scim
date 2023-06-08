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
    /// Class ErrorBase. Implements the <see cref="Talegen.AspNetCore.Scim.Schema.Schematized" />
    /// </summary>
    /// <seealso cref="Talegen.AspNetCore.Scim.Schema.Schematized" />
    [DataContract]
    public abstract class ErrorBase : Schematized
    {
        /// <summary>
        /// Gets or sets the type of the scim.
        /// </summary>
        /// <value>The type of the scim.</value>
        [DataMember(Name = "scimType", Order = 1)] //AttributeNames.ScimType
        public virtual string ScimType
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the detail.
        /// </summary>
        /// <value>The detail.</value>
        [DataMember(Name = "detail", Order = 2)] //AttributeNames.Detail
        public virtual string Detail
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>The status.</value>
        [DataMember(Name = "status", Order = 3)] //AttributeNames.Status
        public virtual int Status
        {
            get;
            set;
        }
    }
}