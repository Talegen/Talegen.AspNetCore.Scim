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
    /// This abstract class implements a minimal address type.
    /// </summary>
    [DataContract]
    public abstract class AddressBase : TypedItem
    {
        /// <summary>
        /// Contains the home address type.
        /// </summary>
        public const string Home = "home";

        /// <summary>
        /// Contains the other address type.
        /// </summary>
        public const string Other = "other";

        /// <summary>
        /// Contains the untyped address type.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Untyped", Justification = "False analysis")]
        public const string Untyped = "untyped";

        /// <summary>
        /// Contains the work address type.
        /// </summary>
        public const string Work = "work";

        /// <summary>
        /// Gets or sets the country.
        /// </summary>
        /// <value>The country.</value>
        [DataMember(Name = AttributeNames.Country, IsRequired = false, EmitDefaultValue = false)]
        public string Country { get; set; }

        /// <summary>
        /// Gets or sets the formatted address.
        /// </summary>
        /// <value>The formatted.</value>
        [DataMember(Name = AttributeNames.Formatted, IsRequired = false, EmitDefaultValue = false)]
        public string Formatted { get; set; }

        /// <summary>
        /// Gets or sets the locality.
        /// </summary>
        /// <value>The locality.</value>
        [DataMember(Name = AttributeNames.Locality, IsRequired = false, EmitDefaultValue = false)]
        public string Locality { get; set; }

        /// <summary>
        /// Gets or sets the postal code.
        /// </summary>
        /// <value>The postal code.</value>
        [DataMember(Name = AttributeNames.PostalCode, IsRequired = false, EmitDefaultValue = false)]
        public string PostalCode { get; set; }

        /// <summary>
        /// Gets or sets the region.
        /// </summary>
        /// <value>The region.</value>
        [DataMember(Name = AttributeNames.Region, IsRequired = false, EmitDefaultValue = false)]
        public string Region { get; set; }

        /// <summary>
        /// Gets or sets the street address.
        /// </summary>
        /// <value>The street address.</value>
        [DataMember(Name = AttributeNames.StreetAddress, IsRequired = false, EmitDefaultValue = false)]
        public string StreetAddress { get; set; }
    }
}