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

    /// <summary>
    /// Class SpecificationVersion.
    /// </summary>
    public static class SpecificationVersion
    {
        /// <summary>
        /// The version one oh value
        /// </summary>
        private static readonly Lazy<Version> VersionOneOhValue =
            new Lazy<System.Version>(
                () =>
                    new Version(1, 0));

        /// <summary>
        /// The version one one value
        /// </summary>
        private static readonly Lazy<Version> VersionOneOneValue =
            new Lazy<Version>(
                () =>
                    new Version(1, 1));

        /// <summary>
        /// The version two oh value
        /// </summary>
        private static readonly Lazy<Version> VersionTwoOhValue =
            new Lazy<Version>(
                () =>
                    new Version(2, 0));

        // NOTE: This version is to be used for DCaaS only.
        /// <summary>
        /// The version two oh one value
        /// </summary>
        private static readonly Lazy<Version> VersionTwoOhOneValue =
            new Lazy<Version>(
                () =>
                    new Version(2, 0, 1));

        /// <summary>
        /// Gets the version one oh.
        /// </summary>
        /// <value>The version one oh.</value>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Oh", Justification = "Not an abbreviation")]
        public static Version VersionOneOh
        {
            get
            {
                return VersionOneOhValue.Value;
            }
        }

        /// <summary>
        /// Gets the version one one.
        /// </summary>
        /// <value>The version one one.</value>
        public static Version VersionOneOne
        {
            get
            {
                return VersionOneOneValue.Value;
            }
        }

        /// <summary>
        /// Gets the version two oh one.
        /// </summary>
        /// <value>The version two oh one.</value>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Oh", Justification = "Not an abbreviation")]
        public static Version VersionTwoOhOne
        {
            get
            {
                return VersionTwoOhOneValue.Value;
            }
        }

        /// <summary>
        /// Gets the version two oh.
        /// </summary>
        /// <value>The version two oh.</value>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Oh", Justification = "Not an abbreviation")]
        public static Version VersionTwoOh
        {
            get
            {
                return VersionTwoOhValue.Value;
            }
        }
    }
}