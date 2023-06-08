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
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

    /// <summary>
    /// Class SystemForCrossDomainIdentityManagementResourceIdentifier. This class cannot be inherited. Implements the <see cref="Talegen.AspNetCore.Scim.Schema.ISystemForCrossDomainIdentityManagementResourceIdentifier" />
    /// </summary>
    /// <seealso cref="Talegen.AspNetCore.Scim.Schema.ISystemForCrossDomainIdentityManagementResourceIdentifier" />
    public sealed class SystemForCrossDomainIdentityManagementResourceIdentifier :
        ISystemForCrossDomainIdentityManagementResourceIdentifier
    {
        /// <summary>
        /// The separator segments
        /// </summary>
        private const string SeparatorSegments = "/";

        /// <summary>
        /// The separators segments
        /// </summary>
        private static readonly Lazy<string[]> SeparatorsSegments = new(() => new string[] { SeparatorSegments });

        /// <summary>
        /// Initializes a new instance of the <see cref="SystemForCrossDomainIdentityManagementResourceIdentifier" /> class.
        /// </summary>
        /// <param name="identifier">The identifier.</param>
        /// <exception cref="System.ArgumentNullException">identifier</exception>
        /// <exception cref="System.ArgumentException"></exception>
        public SystemForCrossDomainIdentityManagementResourceIdentifier(Uri identifier)
        {
            if (identifier == null)
            {
                throw new ArgumentNullException(nameof(identifier));
            }

            string path = identifier.OriginalString;

            // System.Uri.Segments is not supported for relative identifiers.
            var segmentsIndexed =
                path.Split(SeparatorsSegments.Value, StringSplitOptions.None)
                .Select((item, index) => new { Segment = item, Index = index })
                .ToArray(); ;

            var segmentSystemForCrossDomainIdentityManagement =
                segmentsIndexed
                .LastOrDefault(item => item.Segment.Equals(SchemaConstants.PathInterface, StringComparison.OrdinalIgnoreCase));

            if (segmentSystemForCrossDomainIdentityManagement == null)
            {
                if (identifier.IsAbsoluteUri)
                {
                    string exceptionMessage = string.Format(CultureInfo.InvariantCulture, Properties.Resources.ExceptionInvalidIdentifierTemplate, path);
                    throw new ArgumentException(exceptionMessage);
                }
            }
            else
            {
                segmentsIndexed =
                    segmentsIndexed
                    .Where(item => item.Index > segmentSystemForCrossDomainIdentityManagement.Index)
                    .ToArray();
            }

            IReadOnlyCollection<string> segmentsRelative =
                segmentsIndexed
                .Select(item => item.Segment)
                .ToArray();

            string relativePath = string.Join(SeparatorSegments, segmentsRelative);

            if (!relativePath.StartsWith(SeparatorSegments, StringComparison.OrdinalIgnoreCase))
            {
                relativePath = string.Concat(SeparatorSegments, relativePath);
            }

            this.RelativePath = relativePath;
        }

        /// <summary>
        /// Gets the relative path.
        /// </summary>
        /// <value>The relative path.</value>
        public string RelativePath { get; }
    }
}