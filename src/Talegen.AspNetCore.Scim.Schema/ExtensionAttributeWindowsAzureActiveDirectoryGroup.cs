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
    /// Class ExtensionAttributeWindowsAzureActiveDirectoryGroup. This class cannot be inherited.
    /// </summary>
    [DataContract]
    public sealed class ExtensionAttributeWindowsAzureActiveDirectoryGroup
    {
        /// <summary>
        /// Gets or sets the electronic mail addresses.
        /// </summary>
        /// <value>The electronic mail addresses.</value>
        [DataMember(Name = AttributeNames.ElectronicMailAddresses)]
        public IEnumerable<ElectronicMailAddress> ElectronicMailAddresses
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the external identifier.
        /// </summary>
        /// <value>The external identifier.</value>
        [DataMember(Name = AttributeNames.ExternalIdentifier)]
        public string ExternalIdentifier
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether [mail enabled].
        /// </summary>
        /// <value><c>true</c> if [mail enabled]; otherwise, <c>false</c>.</value>
        [DataMember(Name = AttributeNames.MailEnabled, IsRequired = false)]
        public bool MailEnabled
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether [security enabled].
        /// </summary>
        /// <value><c>true</c> if [security enabled]; otherwise, <c>false</c>.</value>
        [DataMember(Name = AttributeNames.SecurityEnabled, IsRequired = false)]
        public bool SecurityEnabled
        {
            get;
            set;
        }
    }
}