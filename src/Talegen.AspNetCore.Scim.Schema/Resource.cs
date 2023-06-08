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
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// This class represents a schematized resource.
    /// </summary>
    [DataContract]
    public abstract class Resource : Schematized
    {
        /// <summary>
        /// Gets or sets the external identifier
        /// </summary>
        /// <value>The external identifier.</value>
        [DataMember(Name = AttributeNames.ExternalIdentifier, IsRequired = false, EmitDefaultValue = false)]
        public string ExternalIdentifier { get; set; }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        [DataMember(Name = AttributeNames.Identifier)]
        public string Identifier { get; set; }

        /// <summary>
        /// Tries the get identifier.
        /// </summary>
        /// <param name="baseIdentifier">The base identifier.</param>
        /// <param name="identifier">The identifier.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public virtual bool TryGetIdentifier(Uri baseIdentifier, out Uri identifier)
        {
            identifier = null;
            return false;
        }

        /// <summary>
        /// Tries the get path identifier.
        /// </summary>
        /// <param name="pathIdentifier">The path identifier.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public virtual bool TryGetPathIdentifier(out Uri pathIdentifier)
        {
            pathIdentifier = null;
            return false;
        }
    }
}