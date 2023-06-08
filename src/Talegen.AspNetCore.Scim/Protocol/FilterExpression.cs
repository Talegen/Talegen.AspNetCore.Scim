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
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using Schema;

    /// <summary>
    /// This class implements a minimum filter expression.
    /// </summary>
    /// <remarks>
    /// Parses filter expressions into a doubly-linked list. A collection of IFilter objects can be obtained from the fully-parsed expression.
    ///
    /// Brackets, that is, '(' and '),' characters demarcate groups. So, each expression has a group identifier. Group identifiers are integers, but the group
    /// identifier may be consisted a "nominal variable," in the terminology of applied statistics: https://en.wikipedia.org/wiki/Level_of_measurement.
    /// Specifically, it does not matter that group 4 is followed by group 6, but merely that the expressions of group six are not in group 4.
    ///
    /// Brackets also demarcate levels. So, each expression has a zero-based level number, zero being the top level. Thus, in the filter expression, a eq 1 and
    /// (b eq 2 or c eq 3) and (d eq 4 or e eq 5), the clause, a eq 1, has the level number 0, while the bracketed clauses have the level number 1. The clause,
    /// a eq 1 is one group, the first pair of bracketed clauses are in a second group, and the second pair of bracketed clauses are in a third group.
    /// </remarks>
    internal sealed class FilterExpression : IFilterExpression
    {
        /// <summary>
        /// Contains the closed bracket.
        /// </summary>
        private const char BracketClose = ')';

        /// <summary>
        /// Contains the escape character.
        /// </summary>
        private const char Escape = '\\';

        /// <summary>
        /// Contains the quote character.
        /// </summary>
        private const char Quote = '"';

        /// <summary>
        /// Contains the left pattern group.
        /// </summary>
        private const string PatternGroupLeft = "left";

        /// <summary>
        /// Contains the level up pattern.
        /// </summary>
        private const string PatternGroupLevelUp = "levelUp";

        /// <summary>
        /// Contains the group operator.
        /// </summary>
        private const string PatternGroupOperator = "operator";

        /// <summary>
        /// Contains the right operator.
        /// </summary>
        private const string PatternGroupRight = "right";

        // (?<levelUp>\(*)?(?<left>(\S)*)(\s*(?<operator>bitAnd|eq|ne|co|sw|ew|ge|gt|isMemberOf|lt|matchesExpression|le|notBitAnd|notMatchesExpression)\s*(?<right>(.)*))?
        private const string PatternTemplate =
            @"(?<" +
            FilterExpression.PatternGroupLevelUp +
            @">\(*)?(?<" +
            FilterExpression.PatternGroupLeft +
            @">(\S)*)(\s*(?<" +
            FilterExpression.PatternGroupOperator +
            @">{0})\s*(?<" +
            FilterExpression.PatternGroupRight +
            @">(.)*))?";

        /// <summary>
        /// Contains the OR operator.
        /// </summary>
        private const string RegularExpressionOperatorOr = "|";

        /// <summary>
        /// Contains a space character.
        /// </summary>
        private const char Space = ' ';

        /// <summary>
        /// Contains the filter template.
        /// </summary>
        private const string Template = "{0} {1} {2}";

        /// <summary>
        /// Contains trailing characters.
        /// </summary>
        private static readonly Lazy<char[]> TrailingCharacters =
            new Lazy<char[]>(() => new char[] { FilterExpression.Quote, FilterExpression.Space, FilterExpression.BracketClose });

        /// <summary>
        /// Contains the comparison operators.
        /// </summary>
        private static readonly Lazy<string> ComparisonOperators = new Lazy<string>(() => FilterExpression.Initialize<ComparisonOperatorValue>());

        /// <summary>
        /// Contains the filter pattern.
        /// </summary>
        private static readonly Lazy<string> FilterPattern = new Lazy<string>(() => FilterExpression.InitializeFilterPattern());

        /// <summary>
        /// Contains the expression
        /// </summary>
        private static readonly Lazy<Regex> Expression = new Lazy<Regex>(() => new Regex(FilterExpression.FilterPattern.Value, RegexOptions.CultureInvariant | RegexOptions.Compiled));

        /// <summary>
        /// Contains the logical AND operator.
        /// </summary>
        private static readonly Lazy<string> LogicalOperatorAnd = new Lazy<string>(() => Enum.GetName(typeof(LogicalOperatorValue), LogicalOperatorValue.and));

        /// <summary>
        /// Contains the logical OR operator.
        /// </summary>
        private static readonly Lazy<string> LogicalOperatorOr = new Lazy<string>(() => Enum.GetName(typeof(LogicalOperatorValue), LogicalOperatorValue.or));

        /// <summary>
        /// Contains the attribute path.
        /// </summary>
        private string attributePath;

        /// <summary>
        /// Contains the comparison operator.
        /// </summary>
        private ComparisonOperatorValue comparisonOperator;

        /// <summary>
        /// Contains the filter operator.
        /// </summary>
        private ComparisonOperator filterOperator;

        /// <summary>
        /// Contains a group value.
        /// </summary>
        private int groupValue;

        /// <summary>
        /// Contains a level value.
        /// </summary>
        private int levelValue;

        /// <summary>
        /// Contains a logical operator value.
        /// </summary>
        private LogicalOperatorValue logicalOperator;

        /// <summary>
        /// Contains the next filter expression.
        /// </summary>
        private FilterExpression next;

        /// <summary>
        /// Contains the comparison value.
        /// </summary>
        private ComparisonValue value;

        /// <summary>
        /// Initializes a new instance of the <see cref="FilterExpression" /> class.
        /// </summary>
        /// <param name="other">Contains another filter expression to process.</param>
        /// <exception cref="ArgumentNullException">Exception is thrown if the expression is not specified.</exception>
        private FilterExpression(FilterExpression other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            this.Text = other.Text;
            this.attributePath = other.attributePath;
            this.comparisonOperator = other.comparisonOperator;
            this.filterOperator = other.filterOperator;
            this.Group = other.Group;
            this.Level = other.Level;
            this.logicalOperator = other.logicalOperator;
            this.value = other.value;

            if (other.next != null)
            {
                this.next = new FilterExpression(other.next);
                this.next.Previous = this;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FilterExpression" /> class.
        /// </summary>
        /// <param name="text">Contains the text to filter.</param>
        /// <param name="group">Contains the group value.</param>
        /// <param name="level">Contains the level.</param>
        /// <exception cref="ArgumentNullException">Exception is thrown if text is not specified.</exception>
        /// <exception cref="ArgumentException">Exception is thrown if the value is invalid.</exception>
        private FilterExpression(string text, int group, int level)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                throw new ArgumentNullException(nameof(text));
            }

            this.Text = text.Trim();

            this.Level = level;
            this.Group = group;

            MatchCollection matches = FilterExpression.Expression.Value.Matches(this.Text);

            foreach (Match match in matches)
            {
                Group levelUpGroup = match.Groups[FilterExpression.PatternGroupLevelUp];

                if (levelUpGroup.Success && levelUpGroup.Value.Any())
                {
                    this.Level += levelUpGroup.Value.Length;
                    this.Group += 1;
                }

                Group operatorGroup = match.Groups[FilterExpression.PatternGroupOperator];

                if (operatorGroup.Success)
                {
                    Group leftGroup = match.Groups[FilterExpression.PatternGroupLeft];
                    Group rightGroup = match.Groups[FilterExpression.PatternGroupRight];
                    this.Initialize(leftGroup, operatorGroup, rightGroup);
                }
                else
                {
                    string remainder = match.Value.Trim();

                    if (!string.IsNullOrWhiteSpace(remainder) && (remainder.Length != 1 || FilterExpression.BracketClose != remainder[0]))
                    {
                        throw new ArgumentException(remainder, nameof(text));
                    }
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FilterExpression" /> class.
        /// </summary>
        /// <param name="text">Contains the text value.</param>
        public FilterExpression(string text)
            : this(text: text, group: 0, level: 0)
        {
        }

        /// <summary>
        /// Contains a private minimal implementation interface for comparing values.
        /// </summary>
        private interface IComparisonValue
        {
            /// <summary>
            /// Gets the data type.
            /// </summary>
            AttributeDataType DataType { get; }

            /// <summary>
            /// Gets a value indicating whether the value is in quotes.
            /// </summary>
            bool Quoted { get; }

            /// <summary>
            /// Gets the value.
            /// </summary>
            string Value { get; }
        }

        /// <summary>
        /// Gets or sets the group value.
        /// </summary>
        private int Group
        {
            get
            {
                return this.groupValue;
            }

            set
            {
                if (value < 0)
                {
                    string message = string.Format(CultureInfo.InvariantCulture, Schema.Properties.Resources.ExceptionInvalidFilterTemplate, this.Text);
#pragma warning disable CA1303 // Do not pass literals as localized parameters
                    throw new ArgumentOutOfRangeException(message, nameof(this.Group));
#pragma warning restore CA1303 // Do not pass literals as localized parameters
                }

                this.groupValue = value;
            }
        }

        /// <summary>
        /// Gets or sets the level.
        /// </summary>
        private int Level
        {
            get
            {
                return this.levelValue;
            }

            set
            {
                if (value < 0)
                {
                    string message = string.Format(CultureInfo.InvariantCulture, Schema.Properties.Resources.ExceptionInvalidFilterTemplate, this.Text);
#pragma warning disable CA1303 // Do not pass literals as localized parameters
                    throw new ArgumentOutOfRangeException(message, nameof(this.Level));
#pragma warning restore CA1303 // Do not pass literals as localized parameters
                }

                this.levelValue = value;
            }
        }

        /// <summary>
        /// Gets or sets the operator.
        /// </summary>
        private ComparisonOperatorValue Operator
        {
            get => this.comparisonOperator;

            set
            {
                switch (value)
                {
                    case ComparisonOperatorValue.bitAnd:
                        this.filterOperator = ComparisonOperator.BitAnd;
                        break;

                    case ComparisonOperatorValue.ew:
                        this.filterOperator = ComparisonOperator.EndsWith;
                        break;

                    case ComparisonOperatorValue.eq:
                        this.filterOperator = ComparisonOperator.Equals;
                        break;

                    case ComparisonOperatorValue.ge:
                        this.filterOperator = ComparisonOperator.EqualOrGreaterThan;
                        break;

                    case ComparisonOperatorValue.gt:
                        this.filterOperator = ComparisonOperator.GreaterThan;
                        break;

                    case ComparisonOperatorValue.le:
                        this.filterOperator = ComparisonOperator.EqualOrLessThan;
                        break;

                    case ComparisonOperatorValue.lt:
                        this.filterOperator = ComparisonOperator.LessThan;
                        break;

                    case ComparisonOperatorValue.includes:
                        this.filterOperator = ComparisonOperator.Includes;
                        break;

                    case ComparisonOperatorValue.isMemberOf:
                        this.filterOperator = ComparisonOperator.IsMemberOf;
                        break;

                    case ComparisonOperatorValue.matchesExpression:
                        this.filterOperator = ComparisonOperator.MatchesExpression;
                        break;

                    case ComparisonOperatorValue.notBitAnd:
                        this.filterOperator = ComparisonOperator.NotBitAnd;
                        break;

                    case ComparisonOperatorValue.ne:
                        this.filterOperator = ComparisonOperator.NotEquals;
                        break;

                    case ComparisonOperatorValue.notMatchesExpression:
                        this.filterOperator = ComparisonOperator.NotMatchesExpression;
                        break;

                    default:
                        string notSupported = Enum.GetName(typeof(ComparisonOperatorValue), this.Operator);
                        throw new NotSupportedException(notSupported);
                }

                this.comparisonOperator = value;
            }
        }

        /// <summary>
        /// Gets or sets the previous filter expression.
        /// </summary>
        private FilterExpression Previous { get; set; }

        /// <summary>
        /// Gets or sets the Text.
        /// </summary>
        private string Text { get; set; }

        /// <summary>
        /// This method is used to operate and AND operator on the filter.
        /// </summary>
        /// <param name="left">The prior filter.</param>
        /// <param name="right">The next filter.</param>
        /// <exception cref="ArgumentNullException">Exception is thrown if a filter is not specified.</exception>
        private static void And(IFilter left, IFilter right)
        {
            if (left == null)
            {
                throw new ArgumentNullException(nameof(left));
            }

            if (right == null)
            {
                throw new ArgumentNullException(nameof(right));
            }

            FilterExpression.And(left.AdditionalFilter, right);
        }

        /// <summary>
        /// This method is used to operate and AND operator on the filter.
        /// </summary>
        /// <param name="left">The prior filter.</param>
        /// <param name="right">The next filter collection.</param>
        /// <returns>Returns a new filter collection.</returns>
        private static IReadOnlyCollection<IFilter> And(IFilter left, IReadOnlyCollection<IFilter> right)
        {
            List<IFilter> result = new List<IFilter>();
            IFilter template = new Filter(left);

            for (int index = 0; index < right.Count; index++)
            {
                IFilter rightFilter = right.ElementAt(index);
                IFilter leftFilter;

                if (index == 0)
                {
                    leftFilter = left;
                }
                else
                {
                    leftFilter = new Filter(template);
                    result.Add(leftFilter);
                }

                FilterExpression.And(leftFilter, rightFilter);
            }

            return result;
        }

        /// <summary>
        /// This method is used to operate and AND operator on the filter.
        /// </summary>
        /// <param name="left">The prior filter collection.</param>
        /// <param name="right">The next filter.</param>
        /// <returns>Returns a new filter collection.</returns>
        /// <exception cref="ArgumentNullException">Exception thrown if a filter is not specified.</exception>
        private static IReadOnlyCollection<IFilter> And(IReadOnlyCollection<IFilter> left, IFilter right)
        {
            if (left == null)
            {
                throw new ArgumentNullException(nameof(left));
            }

            if (right == null)
            {
                throw new ArgumentNullException(nameof(right));
            }

            for (int index = 0; index < left.Count; index++)
            {
                IFilter leftFilter = left.ElementAt(index);
                FilterExpression.And(leftFilter, right);
            }

            return left;
        }

        /// <summary>
        /// This method is used to convert the doubly-linked list into a collection of IFilter objects. There are three cases that may be encountered as the
        /// conversion proceeds through the linked list of clauses. Those cases are documented by comments below.
        /// </summary>
        /// <returns>Returns a collection of filters.</returns>
        /// <exception cref="NotSupportedException">Exception is thrown for unsupported operators other than AND or OR.</exception>
        private IReadOnlyCollection<IFilter> Convert()
        {
            List<IFilter> result = new List<IFilter>();
            IFilter thisFilter = this.ToFilter();

            result.Add(thisFilter);
            FilterExpression current = this.next;

            while (current != null)
            {
                if (this.Level == current.Level)
                {
                    // The current clause has the same level number as the initial clause, such as b eq 2 in the expression a eq 1 and b eq 2.
                    IFilter filter = current.ToFilter();

                    switch (current.Previous.logicalOperator)
                    {
                        case LogicalOperatorValue.and:
                            IFilter left = result.Last();
                            FilterExpression.And(left, filter);
                            break;

                        case LogicalOperatorValue.or:
                            result.Add(filter);
                            break;

                        default:
                            string notSupported = Enum.GetName(typeof(LogicalOperatorValue), this.logicalOperator);
                            throw new NotSupportedException(notSupported);
                    }

                    current = current.next;
                }
                else if (current.Level < this.Level)
                {
                    // The current clause has a lower level number than the initial clause, such as c eq 3 in the expression (a eq 1 and b eq 2) or c eq 3.
                    IReadOnlyCollection<IFilter> superiors = current.Convert();

                    switch (current.Previous.logicalOperator)
                    {
                        case LogicalOperatorValue.and:
                            IFilter superior = superiors.First();
                            result = FilterExpression.And(result, superior).ToList();
                            IReadOnlyCollection<IFilter> remainder = superiors.Skip(1).ToArray();
                            result.AddRange(remainder);
                            break;

                        case LogicalOperatorValue.or:
                            result.AddRange(superiors);
                            break;

                        default:
                            string notSupported = Enum.GetName(typeof(LogicalOperatorValue), this.logicalOperator);
                            throw new NotSupportedException(notSupported);
                    }
                    break;
                }
                else
                {
                    // The current clause has a higher level number than the initial clause, such as b eq 2 in the expression a eq 1 and (b eq 2 or c eq 3) and
                    // (d eq 4 or e eq 5)
                    //
                    // In this case, the linked list is edited, so that c eq 3 has no next link, while the next link of a eq 1 refers to d eq 4. Thereby, b eq 2
                    // or c eq 3 can be converted to filters and combined with the filter composed from a eq 1, after which conversion will continue with the
                    // conversion of d eq 4. It is the change in group number between c eq 3 and d eq 4 that identifies the end of current group, despite the
                    // two clauses having the same level number.
                    //
                    // It is because of the editing of the linked list that the public method, ToFilters(), makes a copy of the linked list before initiating
                    // conversion; so that, ToFilters() can be called on a FilterExpression any number of times, to yield the same output.
                    FilterExpression subordinate = current;

                    while (current != null && this.Level < current.Level && subordinate.Group == current.Group)
                    {
                        current = current.next;
                    }

                    if (current != null)
                    {
                        current.Previous.next = null;
                        subordinate.Previous.next = current;
                    }

                    IReadOnlyCollection<IFilter> subordinates = subordinate.Convert();

                    switch (subordinate.Previous.logicalOperator)
                    {
                        case LogicalOperatorValue.and:
                            IFilter superior = result.Last();
                            IReadOnlyCollection<IFilter> merged = FilterExpression.And(superior, subordinates);
                            result.AddRange(merged);
                            break;

                        case LogicalOperatorValue.or:
                            result.AddRange(subordinates);
                            break;

                        default:
                            string notSupported = Enum.GetName(typeof(LogicalOperatorValue), this.logicalOperator);
                            throw new NotSupportedException(notSupported);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// This method is used to initialize the filter object.
        /// </summary>
        /// <param name="left">Contains the left group object.</param>
        /// <param name="operator">Contains the group operator.</param>
        /// <param name="right">Contains the right filter group.</param>
        /// <exception cref="ArgumentNullException">Exception is thrown if argument is not specified.</exception>
        /// <exception cref="InvalidOperationException">Exception is thrown if a value is invalid.</exception>
        private void Initialize(Group left, Group @operator, Group right)
        {
            if (left == null)
            {
                throw new ArgumentNullException(nameof(left));
            }

            if (@operator == null)
            {
                throw new ArgumentNullException(nameof(@operator));
            }

            if (right == null)
            {
                throw new ArgumentNullException(nameof(right));
            }

            if (!left.Success || !right.Success || string.IsNullOrEmpty(left.Value) || string.IsNullOrEmpty(right.Value))
            {
                string message = string.Format(CultureInfo.InvariantCulture, Schema.Properties.Resources.ExceptionInvalidFilterTemplate, this.Text);
                throw new InvalidOperationException(message);
            }

            this.attributePath = left.Value;

            if (!Enum.TryParse<ComparisonOperatorValue>(@operator.Value, out ComparisonOperatorValue comparisonOperatorValue))
            {
                string message = string.Format(CultureInfo.InvariantCulture, Schema.Properties.Resources.ExceptionInvalidFilterTemplate, this.Text);
                throw new InvalidOperationException(message);
            }

            this.Operator = comparisonOperatorValue;

            if (!FilterExpression.TryParse(right.Value, out string comparisonValue))
            {
                string message = string.Format(CultureInfo.InvariantCulture, Schema.Properties.Resources.ExceptionInvalidFilterTemplate, this.Text);
                throw new InvalidOperationException(message);
            }

            this.value = new ComparisonValue(comparisonValue, FilterExpression.Quote == right.Value[0]);

            int indexRemainder = right.Value.IndexOf(comparisonValue, StringComparison.Ordinal) + comparisonValue.Length;

            if (indexRemainder < right.Value.Length)
            {
                string remainder = right.Value.Substring(indexRemainder);
                int indexAnd = remainder.IndexOf(FilterExpression.LogicalOperatorAnd.Value, StringComparison.Ordinal);
                int indexOr = remainder.IndexOf(FilterExpression.LogicalOperatorOr.Value, StringComparison.Ordinal);
                int indexNextFilter;
                int indexLogicalOperator;

                if (indexAnd >= 0 && (indexOr < 0 || indexAnd < indexOr))
                {
                    indexNextFilter = indexAnd + FilterExpression.LogicalOperatorAnd.Value.Length;
                    this.logicalOperator = LogicalOperatorValue.and;
                    indexLogicalOperator = indexAnd;
                }
                else if (indexOr >= 0)
                {
                    indexNextFilter = indexOr + FilterExpression.LogicalOperatorOr.Value.Length;
                    this.logicalOperator = LogicalOperatorValue.or;
                    indexLogicalOperator = indexOr;
                }
                else
                {
                    string tail = remainder.Trim().TrimEnd(FilterExpression.TrailingCharacters.Value);

                    if (!string.IsNullOrWhiteSpace(tail))
                    {
                        string message = string.Format(CultureInfo.InvariantCulture, Schema.Properties.Resources.ExceptionInvalidFilterTemplate, this.Text);
                        throw new InvalidOperationException(message);
                    }

                    return;
                }

                string nextExpression = remainder.Substring(indexNextFilter);
                int indexClosingBracket = remainder.IndexOf(FilterExpression.BracketClose, StringComparison.InvariantCulture);
                int nextExpressionLevel;
                int nextExpressionGroup;

                if (indexClosingBracket >= 0 && indexClosingBracket < indexLogicalOperator)
                {
                    nextExpressionLevel = this.Level - 1;
                    nextExpressionGroup = this.Group - 1;
                }
                else
                {
                    nextExpressionLevel = this.Level;
                    nextExpressionGroup = this.Group;
                }

                this.next = new FilterExpression(nextExpression, nextExpressionGroup, nextExpressionLevel);
                this.next.Previous = this;
            }
        }

        /// <summary>
        /// </summary>
        /// <typeparam name="TOperator"></typeparam>
        /// <returns></returns>
        private static string Initialize<TOperator>()
        {
            Array comparisonOperatorValues = Enum.GetValues(typeof(TOperator));
            StringBuilder buffer = new StringBuilder();
            foreach (TOperator value in comparisonOperatorValues)
            {
                if (buffer.Length > 0)
                {
                    buffer.Append(FilterExpression.RegularExpressionOperatorOr);
                }
                buffer.Append(value);
            }
            string result = buffer.ToString();
            return result;
        }

        /// <summary>
        /// This method is used to initialize the filter pattern.
        /// </summary>
        /// <returns></returns>
        private static string InitializeFilterPattern()
        {
            return string.Format(CultureInfo.InvariantCulture, FilterExpression.PatternTemplate, FilterExpression.ComparisonOperators.Value);
        }

        /// <summary>
        /// This method is used to convert to a filter interface.
        /// </summary>
        /// <returns>Returns an instance of the <see cref="IFilter" /> object.</returns>
        private IFilter ToFilter()
        {
            IFilter result = new Filter(this.attributePath, this.filterOperator, this.value.Value);
            result.DataType = this.value.DataType;
            return result;
        }

        /// <summary>
        /// This method is used to return a collection of <see cref="IFilter" /> objects.
        /// </summary>
        /// <returns>Returns a <see cref="IReadOnlyCollection{IFilter}" /> collection of filter objects.</returns>
        public IReadOnlyCollection<IFilter> ToFilters()
        {
            IReadOnlyCollection<IFilter> result = new FilterExpression(this).Convert();
            return result;
        }

        /// <summary>
        /// This method is used to serialize the filter to a string.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, FilterExpression.Template, this.attributePath, this.Operator, this.value);
        }

        /// <summary>
        /// This method is used to parse the input value into a filter expression.
        /// </summary>
        /// <param name="input">Contains the input value to parse.</param>
        /// <param name="comparisonValue">Contains the output comparison value.</param>
        /// <returns>Returns a value indicating whether the parse was successful.</returns>
        /// <exception cref="InvalidOperationException">Exception is thrown if the operation was invalid.</exception>
        /// <remarks>
        /// This function attempts to parse the comparison value out of the text to the right of a given comparison operator. For example, given the expression,
        /// a eq 1 and (b eq 2 or c eq 3) and (d eq 4 or e eq 5), the text to the right of the first comparison operator will be, " 1 and (b eq 2 or c eq 3) and
        /// (d eq 4 or e eq 5)," and this function should yield "1" as the comparison value.
        ///
        /// The function aims, first, to correctly parse out arbitrarily complex comparison values that are correctly formatted. Such values may include nested
        /// quotes, nested spaces and nested text matching the logical operators, "and" and "or." However, for compatibility with prior behavior, the function
        /// also accepts values that are not correctly formatted, but are within expressions that conform to certain assumptions. For example, a = Hello,
        /// World!, is accepted, whereas the expression should be, a = "Hello, World!".
        /// </remarks>
        private static bool TryParse(string input, out string comparisonValue)
        {
            comparisonValue = null;
            bool result = false;

            if (!string.IsNullOrWhiteSpace(input))
            {
                string buffer;

                if (input[0] == FilterExpression.Quote)
                {
                    int index;
                    int position = 1;

                    while (true)
                    {
                        index = input.IndexOf(FilterExpression.Quote, position);

                        if (index < 0)
                        {
                            throw new InvalidOperationException();
                        }

                        if (index > 1 && FilterExpression.Escape == input[index - 1])
                        {
                            position = index + 1;
                            continue;
                        }

                        // If incorrectly-escaped, string comparison values were to be rejected, which they should be, strictly, then the following check to
                        // verify that the current quote mark is the last character, or followed by a space or closing bracket, would not be necessary. Alas,
                        // invalid filters have been accepted in the past.
                        int nextCharacterIndex = index + 1;

                        if (nextCharacterIndex < input.Length && input[nextCharacterIndex] != FilterExpression.Space
                                                              && input[nextCharacterIndex] != FilterExpression.BracketClose)
                        {
                            position = nextCharacterIndex;
                            continue;
                        }

                        break;
                    }

                    buffer = input.Substring(1, index - 1);
                }
                else
                {
                    int index = input.IndexOf(FilterExpression.Space, StringComparison.InvariantCulture);

                    if (index >= 0)
                    {
                        // If unquoted string comparison values were to be rejected, which they should be, strictly, then the following check to verify that the
                        // current space is followed by a logical operator would not be necessary. Alas, invalid filters have been accepted in the past.
                        if (input.LastIndexOf(FilterExpression.LogicalOperatorAnd.Value, StringComparison.Ordinal) < index &&
                            input.LastIndexOf(FilterExpression.LogicalOperatorOr.Value, StringComparison.Ordinal) < index)
                        {
                            buffer = input;
                        }
                        else
                        {
                            buffer = input.Substring(0, index);
                        }
                    }
                    else
                    {
                        buffer = input;
                    }
                }

                comparisonValue = FilterExpression.Quote == input[0] ? buffer : buffer.TrimEnd(FilterExpression.TrailingCharacters.Value);
                result = true;
            }

            return result;
        }

        /// <summary>
        /// This class implements the comparison value interface for filter use.
        /// </summary>
        private class ComparisonValue : IComparisonValue
        {
            /// <summary>
            /// Contains the comparison template.
            /// </summary>
            private const string Template = "\"{0}\"";

            /// <summary>
            /// Initializes a new instance of the <see cref="ComparisonValue" /> class.
            /// </summary>
            /// <param name="value">Contains the value.</param>
            /// <param name="quoted">Contains a value indicating whether the value is in quotes.</param>
            /// <exception cref="ArgumentNullException">Exception is thrown when the value is not specified.</exception>
            public ComparisonValue(string value, bool quoted)
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentNullException(nameof(value));
                }

                this.Value = value;
                this.Quoted = quoted;

                if (this.Quoted)
                {
                    this.DataType = AttributeDataType.@string;
                }
                else if (bool.TryParse(this.Value, out bool _))
                {
                    this.DataType = AttributeDataType.boolean;
                }
                else if (long.TryParse(this.Value, out long _))
                {
                    this.DataType = AttributeDataType.integer;
                }
                else if (double.TryParse(this.Value, out double _))
                {
                    this.DataType = AttributeDataType.@decimal;
                }
                else
                {
                    this.DataType = AttributeDataType.@string;
                }
            }

            /// <summary>
            /// Gets the data type.
            /// </summary>
            public AttributeDataType DataType { get; }

            /// <summary>
            /// Gets a value indicating whether the value is in quotes.
            /// </summary>
            public bool Quoted { get; }

            /// <summary>
            /// Gets the value.
            /// </summary>
            public string Value { get; }

            /// <summary>
            /// This method is used to serialize the value to a string.
            /// </summary>
            /// <returns>Returns the serialized value.</returns>
            public override string ToString()
            {
                return this.Quoted ? string.Format(CultureInfo.InvariantCulture, ComparisonValue.Template, this.Value) : this.Value;
            }
        }
    }
}