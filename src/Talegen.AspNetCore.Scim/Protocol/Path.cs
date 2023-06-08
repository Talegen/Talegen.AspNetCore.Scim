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
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text.RegularExpressions;
    using Schema;

    /// <summary>
    /// This class implements a path.
    /// </summary>
    public class Path : IPath
    {
        /// <summary>
        /// Contains the path expression name.
        /// </summary>
        private const string ArgumentNamePathExpression = "pathExpression";

        /// <summary>
        /// Contains the sub attributes name.
        /// </summary>
        private const string ConstructNameSubAttributes = "subAttr";

        /// <summary>
        /// Contains the value path name.
        /// </summary>
        private const string ConstructNameValuePath = "valuePath";

        /// <summary>
        /// Contains the pattern template.
        /// </summary>
        private const string PatternTemplate = @"(?<{0}>.*)\[(?<{1}>.*)\]";

        /// <summary>
        /// Contains the sub-namespace identifier.
        /// </summary>
        private const string SchemaIdentifierSubnamespace = "urn:ietf:params:scim:schemas:";

        /// <summary>
        /// Contains the pattern.
        /// </summary>
        private static readonly string Pattern = string.Format(CultureInfo.InvariantCulture, Path.PatternTemplate, Path.ConstructNameValuePath, Path.ConstructNameSubAttributes);

        /// <summary>
        /// Contains obsolete schema prefix patterns.
        /// </summary>
        private static readonly Lazy<string[]> ObsoleteSchemaPrefixPatterns =
            new Lazy<string[]>(
                () =>
                    new string[]
                    {
                        "urn:scim:schemas:extension:enterprise:1.0.",
                        "urn:scim:schemas:extension:enterprise:2.0."
                    });

        /// <summary>
        /// Contains the regular expression object.
        /// </summary>
        private static readonly Lazy<Regex> RegularExpression =
            new Lazy<Regex>(
                () =>
                    new Regex(Path.Pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant));

        /// <summary>
        /// Contains the attributes separator.
        /// </summary>
        private const char SeparatorAttributes = '.';

        /// <summary>
        /// Initializes a new instance of the <see cref="Path" /> class.
        /// </summary>
        /// <param name="pathExpression">Contains the path expression.</param>
        /// <exception cref="ArgumentNullException">Exception is thrown if the path is not specified.</exception>
        private Path(string pathExpression)
        {
            if (string.IsNullOrWhiteSpace(pathExpression))
            {
                throw new ArgumentNullException(Path.ArgumentNamePathExpression);
            }

            this.Expression = pathExpression;
        }

        /// <inheritdoc />
        public string AttributePath { get; private set; }

        /// <summary>
        /// Gets or sets the expression.
        /// </summary>
        private string Expression { get; set; }

        /// <inheritdoc />
        public string SchemaIdentifier { get; private set; }

        /// <inheritdoc />
        public IReadOnlyCollection<IFilter> SubAttributes { get; private set; }

        /// <inheritdoc />
        public IPath ValuePath { get; private set; }

        /// <summary>
        /// This method is used to create a new path from an expression.
        /// </summary>
        /// <param name="pathExpression">Contains the path expression.</param>
        /// <returns>Returns a new <see cref="IPath" /> implementation.</returns>
        /// <exception cref="ArgumentNullException">Exception is thrown if the expression is not specified.</exception>
        /// <exception cref="ArgumentException">Exception is thrown if the expression is invalid.</exception>
        public static IPath Create(string pathExpression)
        {
            if (string.IsNullOrWhiteSpace(pathExpression))
            {
                throw new ArgumentNullException(Path.ArgumentNamePathExpression);
            }

            if (!Path.TryParse(pathExpression, out IPath result))
            {
                string exceptionMessage = string.Format(CultureInfo.InvariantCulture, Schema.Properties.Resources.ExceptionInvalidPathTemplate, pathExpression);
                throw new ArgumentException(exceptionMessage);
            }

            return result;
        }

        /// <summary>
        /// This method is used to extract a schema identifier from the path expression.
        /// </summary>
        /// <param name="pathExpression">Contains the path expression to search.</param>
        /// <param name="schemaIdentifier">Contains the extracted identifier if found.</param>
        /// <returns>Returns a value indicating whether the identifier was found.</returns>
        private static bool TryExtractSchemaIdentifier(string pathExpression, out string schemaIdentifier)
        {
            schemaIdentifier = null;
            bool result = false;

            if (!string.IsNullOrWhiteSpace(pathExpression) &&
                pathExpression.StartsWith(Path.SchemaIdentifierSubnamespace, StringComparison.OrdinalIgnoreCase) &&
                !Path.ObsoleteSchemaPrefixPatterns.Value.Any(item => pathExpression.StartsWith(item, StringComparison.OrdinalIgnoreCase)))
            {
                int separatorIndex = pathExpression.LastIndexOf(SchemaConstants.SeparatorSchemaIdentifierAttribute, StringComparison.OrdinalIgnoreCase);

                if (separatorIndex != -1)
                {
                    schemaIdentifier = pathExpression.Substring(0, separatorIndex);
                    result = true;
                }
            }

            return result;
        }

        /// <summary>
        /// This method is used to try and parse the path expression into a new <see cref="IPath" /> object.
        /// </summary>
        /// <param name="pathExpression">Contains the path expression to parse.</param>
        /// <param name="path">Contains new parsed <see cref="IPath" /> object if valid.</param>
        /// <returns>Returns a value indicating whether the path expression was parsed successfully.</returns>
        /// <exception cref="ArgumentNullException">Exception is thrown if the expression is not specified.</exception>
        public static bool TryParse(string pathExpression, out IPath path)
        {
            path = null;
            bool result = true;

            if (string.IsNullOrWhiteSpace(pathExpression))
            {
                throw new ArgumentNullException(Path.ArgumentNamePathExpression);
            }

            Path buffer = new Path(pathExpression);
            string expression = pathExpression;

            if (Path.TryExtractSchemaIdentifier(pathExpression, out string schemaIdentifier))
            {
                expression = expression.Substring(schemaIdentifier.Length + 1);
                buffer.SchemaIdentifier = schemaIdentifier;
            }

            int separatorIndex = expression.IndexOf(Path.SeparatorAttributes, StringComparison.InvariantCulture);

            if (separatorIndex >= 0)
            {
                string valuePathExpression = expression.Substring(separatorIndex + 1);
                expression = expression.Substring(0, separatorIndex);

                if (Path.TryParse(valuePathExpression, out IPath valuePath))
                {
                    result = true;
                    buffer.ValuePath = valuePath;
                    buffer.SubAttributes = Array.Empty<IFilter>();
                }
            }

            Match match = Path.RegularExpression.Value.Match(expression);

            if (!match.Success)
            {
                buffer.AttributePath = expression;
                buffer.SubAttributes = Array.Empty<IFilter>();
            }
            else
            {
                buffer.AttributePath = match.Groups[Path.ConstructNameValuePath].Value;
                string filterExpression = match.Groups[Path.ConstructNameSubAttributes].Value;

                result = Filter.TryParse(filterExpression, out IReadOnlyCollection<IFilter> filters);

                if (result)
                {
                    buffer.SubAttributes = filters;
                }
            }

            path = buffer;
            return result;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return this.Expression;
        }
    }
}