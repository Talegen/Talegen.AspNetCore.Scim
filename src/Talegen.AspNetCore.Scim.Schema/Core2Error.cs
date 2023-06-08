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
    /// Class Core2Error. This class cannot be inherited. Implements the <see cref="Talegen.AspNetCore.Scim.Schema.ErrorBase" />
    /// </summary>
    /// <seealso cref="Talegen.AspNetCore.Scim.Schema.ErrorBase" />
    [DataContract]
    public sealed class Core2Error : ErrorBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Core2Error" /> class.
        /// </summary>
        /// <param name="detail">The detail.</param>
        /// <param name="status">The status.</param>
        /// <param name="scimType">Type of the scim.</param>
        /// <remarks>See https://datatracker.ietf.org/doc/html/rfc7644#section-3.12</remarks>
        public Core2Error(string detail, int status, string scimType = null)
        {
            this.AddSchema(ProtocolSchemaIdentifiers.Version2Error);

            this.Detail = detail;
            this.Status = status;
            this.ScimType = scimType != null ? scimType : null;
        }
    }
}