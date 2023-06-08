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
    /// Class Name. This class cannot be inherited.
    /// </summary>
    [DataContract]
    public sealed class Name
    {
        /// <summary>
        /// Gets or sets the formatted.
        /// </summary>
        /// <value>The formatted.</value>
        [DataMember(Name = AttributeNames.Formatted, Order = 0, IsRequired = false, EmitDefaultValue = false)]
        public string Formatted
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name of the family.
        /// </summary>
        /// <value>The name of the family.</value>
        [DataMember(Name = AttributeNames.FamilyName, Order = 1, IsRequired = false, EmitDefaultValue = false)]
        public string FamilyName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name of the given.
        /// </summary>
        /// <value>The name of the given.</value>
        [DataMember(Name = AttributeNames.GivenName, Order = 1, IsRequired = false, EmitDefaultValue = false)]
        public string GivenName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the honorific prefix.
        /// </summary>
        /// <value>The honorific prefix.</value>
        [DataMember(Name = AttributeNames.HonorificPrefix, Order = 1, IsRequired = false, EmitDefaultValue = false)]
        public string HonorificPrefix
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the honorific suffix.
        /// </summary>
        /// <value>The honorific suffix.</value>
        [DataMember(Name = AttributeNames.HonorificSuffix, Order = 1, IsRequired = false, EmitDefaultValue = false)]
        public string HonorificSuffix
        {
            get;
            set;
        }
    }
}