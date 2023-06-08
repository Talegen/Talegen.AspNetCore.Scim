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

namespace Talegen.AspNetCore.Scim.Service
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Protocol;
    using Schema;

    /// <summary>
    /// This class represents a URI for an SCIM request.
    /// </summary>
    internal class UniformResourceIdentifier : IUniformResourceIdentifier
    {
        /// <summary>
        /// Contains the alternate path template.
        /// </summary>
        private const string AlternatePathTemplate = UniformResourceIdentifier.RegularExpressionOperatorOr + "{0}";

        /// <summary>
        /// Contains identifier argument name.
        /// </summary>
        private const string ArgumentNameIdentifier = "identifier";

        /// <summary>
        /// Contains query argument name.
        /// </summary>
        private const string ArgumentNameQuery = "query";

        /// <summary>
        /// Contains identifier expression group name.
        /// </summary>
        private const string ExpressionGroupNameIdentifier = "identifier";

        /// <summary>
        /// Contains type expression group name.
        /// </summary>
        private const string ExpressionGroupNameType = "type";

        /// <summary>
        /// Contains expression Or operator.
        /// </summary>
        private const string RegularExpressionOperatorOr = "|";

        /// <summary> Contains expression retrieval pattern template. </summary> <remarks> (?<type>(Groups|Users{0}))/?(?<identifier>[^\?]*) wherein {0} will be
        /// replaced with, for example, |MyExtendedTypePath </remarks>
        private const string RetrievalPatternTemplate = @"(?<" +
                                                        ExpressionGroupNameType +
                                                        @">(" +
                                                        ProtocolConstants.PathGroups +
                                                        RegularExpressionOperatorOr +
                                                        ProtocolConstants.PathUsers +
                                                        @"{0}))/?(?<" +
                                                        ExpressionGroupNameIdentifier +
                                                        @">[^\?]*)";

        /// <summary>
        /// Initializes a new instance of the <see cref="UniformResourceIdentifier" /> class.
        /// </summary>
        /// <param name="identifier">Contains the resource identifier.</param>
        /// <param name="query">Contains the resource query.</param>
        /// <exception cref="ArgumentNullException">Exception is thrown if parameters are not specified.</exception>
        private UniformResourceIdentifier(IResourceIdentifier identifier, IResourceQuery query)
        {
            this.Identifier = identifier ?? throw new ArgumentNullException(UniformResourceIdentifier.ArgumentNameIdentifier);
            this.Query = query ?? throw new ArgumentNullException(UniformResourceIdentifier.ArgumentNameQuery);
            this.IsQuery = this.Identifier == null || string.IsNullOrWhiteSpace(this.Identifier.Identifier);
        }

        public IResourceIdentifier Identifier { get; }

        public bool IsQuery { get; }

        public IResourceQuery Query { get; }

        /// <summary>
        /// </summary>
        /// <param name="identifier"></param>
        /// <param name="extensions"></param>
        /// <param name="parsedIdentifier"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static bool TryParse(Uri identifier, IReadOnlyCollection<IExtension> extensions, out IUniformResourceIdentifier parsedIdentifier)
        {
            parsedIdentifier = null;
            bool result = true;

            if (identifier == null)
            {
                throw new ArgumentNullException(UniformResourceIdentifier.ArgumentNameIdentifier);
            }

            IReadOnlyCollection<IExtension> effectiveExtensions = extensions ?? Array.Empty<IExtension>();

            IResourceQuery query = new ResourceQuery(identifier);

            IReadOnlyCollection<string> alternatePathCollection =
                effectiveExtensions
                    .Select(item => string.Format(CultureInfo.InvariantCulture, UniformResourceIdentifier.AlternatePathTemplate, item.Path))
                    .ToArray();
            string alternatePaths = string.Join(string.Empty, alternatePathCollection);

            string retrievalPattern = string.Format(CultureInfo.InvariantCulture, UniformResourceIdentifier.RetrievalPatternTemplate, alternatePaths);
            Regex retrievalExpression = new Regex(retrievalPattern, RegexOptions.IgnoreCase);
            Match expressionMatch = retrievalExpression.Match(identifier.AbsoluteUri);

            if (expressionMatch.Success)
            {
                string type = expressionMatch.Groups[UniformResourceIdentifier.ExpressionGroupNameType].Value;

                if (!string.IsNullOrWhiteSpace(type))
                {
                    string schemaIdentifier = string.Empty;

                    switch (type)
                    {
                        case ProtocolConstants.PathGroups:
                            schemaIdentifier = SchemaIdentifiers.Core2Group;
                            break;

                        case ProtocolConstants.PathUsers:
                            schemaIdentifier = SchemaIdentifiers.Core2EnterpriseUser;
                            break;

                        default:
                            if (extensions != null)
                            {
                                schemaIdentifier = effectiveExtensions
                                        .Where(item => string.Equals(item.Path, type, StringComparison.OrdinalIgnoreCase))
                                        .Select(item => item.SchemaIdentifier)
                                        .SingleOrDefault();

                                if (string.IsNullOrWhiteSpace(schemaIdentifier))
                                {
                                    result = false;
                                }
                            }
                            else
                            {
                                result = false;
                            }

                            break;
                    }

                    IResourceIdentifier resourceIdentifier = new ResourceIdentifier();
                    resourceIdentifier.SchemaIdentifier = schemaIdentifier;

                    string resourceIdentifierValue = expressionMatch.Groups[UniformResourceIdentifier.ExpressionGroupNameIdentifier].Value;

                    if (!string.IsNullOrWhiteSpace(resourceIdentifierValue))
                    {
                        string unescapedIdentifier = Uri.UnescapeDataString(resourceIdentifierValue);
                        resourceIdentifier.Identifier = unescapedIdentifier;
                    }

                    parsedIdentifier = new UniformResourceIdentifier(resourceIdentifier, query);
                }
                else
                {
                    result = false;
                }
            }
            else
            {
                result = false;
            }

            return result;
        }
    }
}