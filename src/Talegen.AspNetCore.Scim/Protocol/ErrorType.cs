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
    /// <summary>
    /// Contains an enumerated list of error types.
    /// </summary>
    public enum ErrorType
    {
        /// <summary>
        /// Invalid Filter.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "invalid", Justification = "The casing is stipulated in the specification of a protocol")]
        invalidFilter,

        /// <summary>
        /// Invalid path.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "invalid", Justification = "The casing is stipulated in the specification of a protocol")]
        invalidPath,

        /// <summary>
        /// Invalid syntax.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "invalid", Justification = "The casing is stipulated in the specification of a protocol")]
        invalidSyntax,

        /// <summary>
        /// Invalid value.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "invalid", Justification = "The casing is stipulated in the specification of a protocol")]
        invalidValue,

        /// <summary>
        /// Invalid version.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "invalid", Justification = "The casing is stipulated in the specification of a protocol")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Vers", Justification = "The name is stipulated in the specification of a protocol")]
        invalidVers,

        /// <summary>
        /// Mutability.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "mutability", Justification = "The casing is stipulated in the specification of a protocol")]
        mutability,

        /// <summary>
        /// No Target.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "no", Justification = "The casing is stipulated in the specification of a protocol")]
        noTarget,

        /// <summary>
        /// Sensitive.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "sensitive", Justification = "The casing is stipulated in the specification of a protocol")]
        sensitive,

        /// <summary>
        /// Too many.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "too", Justification = "The casing is stipulated in the specification of a protocol")]
        tooMany,

        /// <summary>
        /// Uniqueness.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "uniqueness", Justification = "The casing is stipulated in the specification of a protocol")]
        uniqueness
    }
}