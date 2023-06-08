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
    /// Class ExtensionAttributeEnterpriseUserBase.
    /// </summary>
    [DataContract]
    public abstract class ExtensionAttributeEnterpriseUserBase
    {
        /// <summary>
        /// Gets or sets the cost center.
        /// </summary>
        /// <value>The cost center.</value>
        [DataMember(Name = AttributeNames.CostCenter, IsRequired = false, EmitDefaultValue = false)]
        public string CostCenter
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the department.
        /// </summary>
        /// <value>The department.</value>
        [DataMember(Name = AttributeNames.Department, IsRequired = false, EmitDefaultValue = false)]
        public string Department
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the division.
        /// </summary>
        /// <value>The division.</value>
        [DataMember(Name = AttributeNames.Division, IsRequired = false, EmitDefaultValue = false)]
        public string Division
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the employee number.
        /// </summary>
        /// <value>The employee number.</value>
        [DataMember(Name = AttributeNames.EmployeeNumber, IsRequired = false, EmitDefaultValue = false)]
        public string EmployeeNumber
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the organization.
        /// </summary>
        /// <value>The organization.</value>
        [DataMember(Name = AttributeNames.Organization, IsRequired = false, EmitDefaultValue = false)]
        public string Organization
        {
            get;
            set;
        }
    }
}