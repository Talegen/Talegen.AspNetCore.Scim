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
    /// <summary>
    /// Contains an enumerated list of attribute data types.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1720:Identifier contains type name", Justification = "Enum of type names will contain type names")]
    public enum AttributeDataType
    {
        /// <summary>
        /// The string
        /// </summary>
        @string,

        /// <summary>
        /// The boolean
        /// </summary>
        boolean,

        /// <summary>
        /// The decimal
        /// </summary>
        @decimal,

        /// <summary>
        /// The integer
        /// </summary>
        integer,

        /// <summary>
        /// The date time
        /// </summary>
        dateTime,

        /// <summary>
        /// The binary
        /// </summary>
        binary,

        /// <summary>
        /// The reference
        /// </summary>
        reference,

        /// <summary>
        /// The complex
        /// </summary>
        complex
    }
}