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

namespace Talegen.AspNetCore.Scim.Protocol
{
    using System.Globalization;
    using System.Runtime.Serialization;
    using Schema;

    /// <summary>
    /// This class defines an operation value data contract.
    /// </summary>
    [DataContract]
    public sealed class OperationValue
    {
        /// <summary>
        /// Contains the template for serialization.
        /// </summary>
        private const string Template = "{0} {1}";

        /// <summary>
        /// Gets or sets the reference.
        /// </summary>
        [DataMember(Name = ProtocolAttributeNames.Reference, Order = 0, IsRequired = false, EmitDefaultValue = false)]
        public string Reference { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        [DataMember(Name = AttributeNames.Value, Order = 1, IsRequired = false, EmitDefaultValue = false)]
        public string Value { get; set; }

        /// <summary>
        /// Serializes the operation value to a string.
        /// </summary>
        /// <returns>Returns the serialized value.</returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, OperationValue.Template, this.Value, this.Reference).Trim();
        }
    }
}