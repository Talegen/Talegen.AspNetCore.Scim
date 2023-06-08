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
    using System.Text;
    using System.Text.RegularExpressions;

    /// <summary>
    /// This class contains internal string extension methods.
    /// </summary>
    internal static class StringExtension
    {
        /// <summary>
        /// Contains a regex pattern for double quotes.
        /// </summary>
        private const string PatternEscapedDoubleQuote = @"\\*" + StringExtension.QuoteDouble;

        /// <summary>
        /// Contains a regex pattern for single quotes.
        /// </summary>
        private const string PatternEscapedSingleQuote = @"\\*" + StringExtension.QuoteSingle;

        /// <summary>
        /// Contains a double quote.
        /// </summary>
        private const string QuoteDouble = "\"";

        /// <summary>
        /// Contains a single quote.
        /// </summary>
        private const string QuoteSingle = "'";

        /// <summary>
        /// Contains a RegEx expression for double quotes.
        /// </summary>
        private static readonly Lazy<Regex> ExpressionEscapedDoubleQuote = new Lazy<Regex>(() => new Regex(StringExtension.PatternEscapedDoubleQuote, RegexOptions.Compiled | RegexOptions.CultureInvariant));

        /// <summary>
        /// Contains a RegEx expression for a single quote.
        /// </summary>
        private static readonly Lazy<Regex> ExpressionEscapedSingleQuote = new Lazy<Regex>(() => new Regex(StringExtension.PatternEscapedSingleQuote, RegexOptions.Compiled | RegexOptions.CultureInvariant));

        /// <summary>
        /// This method is used to remove quotes from an input string.
        /// </summary>
        /// <param name="input">Contains the input string to parse.</param>
        /// <returns>Returns the filtered input string.</returns>
        public static string Unquote(this string input)
        {
            string result = input;

            if (!string.IsNullOrWhiteSpace(input))
            {
                int indexQuoteDouble = input.Trim().IndexOf(StringExtension.QuoteDouble, 0, StringComparison.OrdinalIgnoreCase);
                int indexQuoteSingle = input.Trim().IndexOf(StringExtension.QuoteSingle, 0, StringComparison.OrdinalIgnoreCase);
                Regex expression = null;

                if (indexQuoteDouble == 0)
                {
                    expression = ExpressionEscapedDoubleQuote.Value;
                }
                else if (indexQuoteSingle == 0)
                {
                    expression = ExpressionEscapedSingleQuote.Value;
                }

                if (expression != null)
                {
                    MatchCollection matches = expression.Matches(input);

                    if (matches.Count > 0)
                    {
                        StringBuilder buffer = new StringBuilder(input);

                        for (int matchIndex = matches.Count - 1; matchIndex >= 0; matchIndex--)
                        {
                            Match match = matches[matchIndex];
                            int index = match.Index;
                            buffer.Remove(index, 1);
                        }

                        result = buffer.ToString();
                    }
                }
            }

            return result;
        }
    }
}