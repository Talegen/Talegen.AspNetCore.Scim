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
    using System.Web;
    using Schema;

    /// <summary>
    /// Contains an enumerated list of comparison operators.
    /// </summary>
    public enum ComparisonOperatorValue
    {
        /// <summary>
        /// Bit And.
        /// </summary>
        bitAnd,

        /// <summary>
        /// Equal to.
        /// </summary>
        eq,

        /// <summary>
        /// Not Equal to.
        /// </summary>
        ne,

        /// <summary>
        /// </summary>
        co,

        /// <summary>
        /// </summary>
        sw,

        /// <summary>
        /// </summary>
        ew,

        /// <summary>
        /// Greater than or equal to.
        /// </summary>
        ge,

        /// <summary>
        /// Greater than.
        /// </summary>
        gt,

        /// <summary>
        /// Includes.
        /// </summary>
        includes,

        /// <summary>
        /// Is a member of.
        /// </summary>
        isMemberOf,

        /// <summary>
        /// Less than.
        /// </summary>
        lt,

        /// <summary>
        /// Matches expression.
        /// </summary>
        matchesExpression,

        /// <summary>
        /// Less than or equal to.
        /// </summary>
        le,

        /// <summary>
        /// Not Bit And.
        /// </summary>
        notBitAnd,

        /// <summary>
        /// Does not match expression.
        /// </summary>
        notMatchesExpression
    }

    /// <summary>
    /// Contains an enumerated list of operator values.
    /// </summary>
    public enum LogicalOperatorValue
    {
        /// <summary>
        /// And.
        /// </summary>
        and,

        /// <summary>
        /// Or.
        /// </summary>
        or
    }

    /// <summary>
    /// This method implements the minimum implementation of a filter.
    /// </summary>
    public sealed class Filter : IFilter
    {
        /// <summary>
        /// Contains the comparison value template.
        /// </summary>
        private const string ComparisonValueTemplate = "\"{0}\"";

        /// <summary>
        /// Contains the encoding space character.
        /// </summary>
        private const string EncodingSpacePer2396 = "+";

        /// <summary>
        /// Contains the null value string representation.
        /// </summary>
        public const string NullValue = "null";

        /// <summary>
        /// Contains reserved characters.
        /// </summary>
        private const string ReservedPerRfc2396 = ";/?:@&=+$,";

        /// <summary>
        /// Contains additional reserved characters.
        /// </summary>
        private const string ReservedPerRfc3986 = Filter.ReservedPerRfc2396 + "#[]!'()*";

        /// <summary>
        /// Contains a space string representation.
        /// </summary>
        private const string Space = " ";

        /// <summary>
        /// Contains a SCIM filter parameter template.
        /// </summary>
        private const string Template = "filter={0}";

        /// <summary>
        /// Contains the comparison template.
        /// </summary>
        private const string TemplateComparison = "{0} {1} {2}";

        /// <summary>
        /// Contains the conjunction template.
        /// </summary>
        private const string TemplateConjunction = "{0} {1} {2}";

        /// <summary>
        /// Contains the nesting template.
        /// </summary>
        private const string TemplateNesting = "({0})";

        /// <summary>
        /// Contains an array of reserved characters.
        /// </summary>
        private static readonly Lazy<char[]> ReservedCharactersPerRfc3986 = new(() => Filter.ReservedPerRfc3986.ToCharArray());

        /// <summary>
        /// Contains an array of reserved characters.
        /// </summary>
        private static readonly Lazy<IReadOnlyDictionary<string, string>> ReservedCharacterEncodingsPerRfc3986 = new(Filter.InitializeReservedCharacter3986Encodings);

        /// <summary>
        /// Contains an array of reserved characters
        /// </summary>
        private static readonly Lazy<IReadOnlyDictionary<string, string>> ReservedCharacterEncodingsPerRfc2396 = new(Filter.InitializeReservedCharacter2396Encodings);

        /// <summary>
        /// Contains the comparison value.
        /// </summary>
        private string comparisonValue;

        /// <summary>
        /// Contains the encoded comparison value.
        /// </summary>
        private string comparisonValueEncoded;

        /// <summary>
        /// Contains the attribute data type.
        /// </summary>
        private AttributeDataType? dataType;

        /// <summary>
        /// Initializes a new instance of the <see cref="Filter" /> class.
        /// </summary>
        private Filter()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Filter" /> class.
        /// </summary>
        /// <param name="attributePath">Contains the attribute path.</param>
        /// <param name="filterOperator">Contains the filter operator.</param>
        /// <param name="comparisonValue">Contains the comparison value.</param>
        /// <exception cref="ArgumentNullException">Exception is thrown when attribute or comparison value are not specified.</exception>
        public Filter(string attributePath, ComparisonOperator filterOperator, string comparisonValue)
        {
            if (string.IsNullOrWhiteSpace(attributePath))
            {
                throw new ArgumentNullException(nameof(attributePath));
            }

            if (string.IsNullOrWhiteSpace(comparisonValue))
            {
                throw new ArgumentNullException(nameof(comparisonValue));
            }

            this.AttributePath = attributePath;
            this.FilterOperator = filterOperator;
            this.ComparisonValue = comparisonValue;
            this.DataType = AttributeDataType.@string;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Filter" /> class.
        /// </summary>
        /// <param name="other">Contains other filter argument.</param>
        /// <exception cref="ArgumentNullException">Exception is thrown if the filter is not specified.</exception>
        public Filter(IFilter other)
            : this(other?.AttributePath!, other!.FilterOperator, other?.ComparisonValue!)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            this.DataType = other.DataType;

            if (other.AdditionalFilter != null)
            {
                this.AdditionalFilter = new Filter(other.AdditionalFilter);
            }
        }

        /// <summary>
        /// Gets or sets the additional filter.
        /// </summary>
        public IFilter AdditionalFilter { get; set; }

        /// <summary>
        /// Gets attribute path.
        /// </summary>
        public string AttributePath { get; }

        /// <summary>
        /// Gets or sets the comparison value.
        /// </summary>
        public string ComparisonValue
        {
            get => this.comparisonValue;

            private set
            {
                Filter.Validate(this.DataType, value);
                this.comparisonValue = value;
                string encodedValue = this.comparisonValue;

                encodedValue = Filter.ReservedCharacterEncodingsPerRfc2396.Value
                    .Aggregate(encodedValue, (current, encoding) =>
                        current.Replace(encoding.Key, encoding.Value, StringComparison.InvariantCulture));

                this.comparisonValueEncoded = encodedValue;
            }
        }

        /// <summary>
        /// Gets the encoded comparison value.
        /// </summary>
        public string ComparisonValueEncoded => this.comparisonValueEncoded;

        /// <summary>
        /// Gets or sets the data type.
        /// </summary>
        public AttributeDataType? DataType
        {
            get => this.dataType;

            set
            {
                Filter.Validate(value, this.ComparisonValue);
                this.dataType = value;
            }
        }

        /// <summary>
        /// Gets or ses the filter operator.
        /// </summary>
        public ComparisonOperator FilterOperator { get; set; }

        /// <summary>
        /// This method is used to initialize reserved encodings.
        /// </summary>
        /// <returns>Returns a dictionary of encodings.</returns>
        private static IReadOnlyDictionary<string, string> InitializeReservedCharacter2396Encodings()
        {
            Dictionary<string, string> result =
                Filter.ReservedCharacterEncodingsPerRfc3986.Value
                .ToDictionary(
                    (KeyValuePair<string, string> item) => item.Key,
                    (KeyValuePair<string, string> item) => item.Value);

            result.Add(Filter.Space, Filter.EncodingSpacePer2396);

            return result;
        }

        /// <summary>
        /// This method is used to initialize reserved character encodings.
        /// </summary>
        /// <returns>Returns a dictionary of encodings.</returns>
        private static IReadOnlyDictionary<string, string> InitializeReservedCharacter3986Encodings()
        {
            Dictionary<string, string> result = new Dictionary<string, string>(Filter.ReservedCharactersPerRfc3986.Value.Length);

            foreach (char character in Filter.ReservedCharactersPerRfc3986.Value)
            {
                string from = character.ToString(CultureInfo.InvariantCulture);
                string to = HttpUtility.UrlEncode(from);
                result.Add(from, to);
            }

            return result;
        }

        /// <summary>
        /// This method is used to serialize the filter.
        /// </summary>
        /// <returns>Returns the serialized object.</returns>
        /// <exception cref="NotSupportedException">Exception is thrown if the filter type is not supported.</exception>
        public string Serialize()
        {
            ComparisonOperatorValue operatorValue;

            switch (this.FilterOperator)
            {
                case ComparisonOperator.BitAnd:
                    operatorValue = ComparisonOperatorValue.bitAnd;
                    break;

                case ComparisonOperator.EndsWith:
                    operatorValue = ComparisonOperatorValue.ew;
                    break;

                case ComparisonOperator.Equals:
                    operatorValue = ComparisonOperatorValue.eq;
                    break;

                case ComparisonOperator.EqualOrGreaterThan:
                    operatorValue = ComparisonOperatorValue.ge;
                    break;

                case ComparisonOperator.GreaterThan:
                    operatorValue = ComparisonOperatorValue.gt;
                    break;

                case ComparisonOperator.EqualOrLessThan:
                    operatorValue = ComparisonOperatorValue.le;
                    break;

                case ComparisonOperator.LessThan:
                    operatorValue = ComparisonOperatorValue.lt;
                    break;

                case ComparisonOperator.Includes:
                    operatorValue = ComparisonOperatorValue.includes;
                    break;

                case ComparisonOperator.IsMemberOf:
                    operatorValue = ComparisonOperatorValue.isMemberOf;
                    break;

                case ComparisonOperator.MatchesExpression:
                    operatorValue = ComparisonOperatorValue.matchesExpression;
                    break;

                case ComparisonOperator.NotBitAnd:
                    operatorValue = ComparisonOperatorValue.notBitAnd;
                    break;

                case ComparisonOperator.NotEquals:
                    operatorValue = ComparisonOperatorValue.ne;
                    break;

                case ComparisonOperator.NotMatchesExpression:
                    operatorValue = ComparisonOperatorValue.notMatchesExpression;
                    break;

                default:
                    string notSupportedValue = Enum.GetName(typeof(ComparisonOperator), this.FilterOperator);
                    throw new NotSupportedException(notSupportedValue);
            }

            string rightHandSide;
            AttributeDataType effectiveDataType = this.DataType ?? AttributeDataType.@string;

            switch (effectiveDataType)
            {
                case AttributeDataType.boolean:
                case AttributeDataType.@decimal:
                case AttributeDataType.integer:
                    rightHandSide = this.ComparisonValue;
                    break;

                default:
                    rightHandSide = string.Format(CultureInfo.InvariantCulture, Filter.ComparisonValueTemplate, this.ComparisonValue);
                    break;
            }

            string filter = string.Format(CultureInfo.InvariantCulture, Filter.TemplateComparison, this.AttributePath, operatorValue, rightHandSide);
            string result;

            if (this.AdditionalFilter != null)
            {
                string additionalFilter = this.AdditionalFilter.Serialize();
                result = string.Format(CultureInfo.InvariantCulture, Filter.TemplateConjunction, filter, LogicalOperatorValue.and, additionalFilter);
            }
            else
            {
                result = filter;
            }

            return result;
        }

        /// <summary>
        /// This method is used to serialize the filter to a string.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.Serialize();
        }

        /// <summary>
        /// This method is used to serialize a list of filters to a string.
        /// </summary>
        /// <param name="filters">Contains a list of filters.</param>
        /// <returns>Returns a serialized object of filters.</returns>
        /// <exception cref="ArgumentNullException">Exception is thrown if the collection is not specified.</exception>
        public static string ToString(IReadOnlyCollection<IFilter> filters)
        {
            if (filters == null)
            {
                throw new ArgumentNullException(nameof(filters));
            }

            string placeholder = Guid.NewGuid().ToString();
            string allFilters = null;

            foreach (IFilter filter in filters)
            {
                Filter clone = new Filter(filter);
                clone.ComparisonValue = placeholder;
                string currentFilter = clone.Serialize();
                string encodedFilter = HttpUtility.UrlEncode(currentFilter).Replace(placeholder, filter.ComparisonValueEncoded, StringComparison.InvariantCulture);

                if (string.IsNullOrWhiteSpace(allFilters))
                {
                    allFilters = filters.Count > 1 ? string.Format(CultureInfo.InvariantCulture, Filter.TemplateNesting, encodedFilter) : encodedFilter;
                }
                else
                {
                    string rightHandSide = filter.AdditionalFilter != null || filters.Count > 1 ?
                        string.Format(CultureInfo.InvariantCulture, Filter.TemplateNesting, encodedFilter) :
                        encodedFilter;

                    allFilters = string.Format(CultureInfo.InvariantCulture, Filter.TemplateConjunction, allFilters, LogicalOperatorValue.or, rightHandSide);
                }
            }

            string result = string.Format(CultureInfo.InvariantCulture, Filter.Template, allFilters);

            return result;
        }

        /// <summary>
        /// This method is used to support the parsing of a filter expression into a collection of filters.
        /// </summary>
        /// <param name="filterExpression">Contains the filter expression to parse.</param>
        /// <param name="filters">Contains the output of filter collection items.</param>
        /// <returns>Returns a value indicating whether the expression was parsed successfully.</returns>
        /// <exception cref="ArgumentNullException">Exception is thrown if the filter expression is not specified.</exception>
        public static bool TryParse(string filterExpression, out IReadOnlyCollection<IFilter> filters)
        {
            string expression = filterExpression?.Trim()?.Unquote();
            bool result = false;

            if (string.IsNullOrWhiteSpace(expression))
            {
                throw new ArgumentNullException(nameof(filterExpression));
            }

            try
            {
                filters = new FilterExpression(expression).ToFilters();
                result = true;
            }
            catch (ArgumentOutOfRangeException)
            {
                filters = null;
            }
            catch (ArgumentException)
            {
                filters = null;
            }
            catch (InvalidOperationException)
            {
                filters = null;
            }

            return result;
        }

        /// <summary>
        /// This method is used to validate the data type with a value.
        /// </summary>
        /// <param name="dataType">Contains the data type.</param>
        /// <param name="value">Contains the value.</param>
        /// <exception cref="InvalidOperationException">Exception is thrown if the value cannot be parsed or is invalid..</exception>
        /// <exception cref="NotSupportedException">Exception thrown if the data type is not supported.</exception>
        private static void Validate(AttributeDataType? dataType, string value)
        {
            if (dataType.HasValue && !string.IsNullOrWhiteSpace(value))
            {
                switch (dataType.Value)
                {
                    case AttributeDataType.boolean:
                        if (!bool.TryParse(value, out bool _))
                        {
                            throw new InvalidOperationException(Schema.Properties.Resources.ExceptionInvalidValue);
                        }

                        break;

                    case AttributeDataType.@decimal:
                        if (!double.TryParse(value, out double _))
                        {
                            throw new InvalidOperationException(Schema.Properties.Resources.ExceptionInvalidValue);
                        }

                        break;

                    case AttributeDataType.integer:
                        if (!long.TryParse(value, out long _))
                        {
                            throw new InvalidOperationException(Schema.Properties.Resources.ExceptionInvalidValue);
                        }

                        break;

                    case AttributeDataType.binary:
                    case AttributeDataType.complex:
                    case AttributeDataType.dateTime:
                    case AttributeDataType.reference:
                    case AttributeDataType.@string:
                        break;

                    default:
                        string unsupported = Enum.GetName(typeof(AttributeDataType), dataType.Value);
                        throw new NotSupportedException(unsupported);
                }
            }
        }
    }
}