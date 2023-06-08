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
    /// Class Core2EnterpriseUser. This class cannot be inherited. Implements the <see cref="Talegen.AspNetCore.Scim.Schema.Core2EnterpriseUserBase" /> class.
    /// </summary>
    /// <seealso cref="Talegen.AspNetCore.Scim.Schema.Core2EnterpriseUserBase" />
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1501:AvoidExcessiveInheritance", Justification = "The long inheritence hieararchy reflects the the System for Cross-Domain Identity Management inheritence mechanism.")]
    [DataContract(Name = DataContractName)]
    public sealed class Core2EnterpriseUser : Core2EnterpriseUserBase
    {
        /// <summary>
        /// The data contract name
        /// </summary>
        private const string DataContractName = "Core2EnterpriseUser";

        /// <summary>
        /// Initializes a new instance of the <see cref="Core2EnterpriseUser" /> class.
        /// </summary>
        public Core2EnterpriseUser()
            : base()
        {
        }
    }
}