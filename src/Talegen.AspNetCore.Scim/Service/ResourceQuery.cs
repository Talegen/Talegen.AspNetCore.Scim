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
    using System.Collections.Specialized;
    using System.Globalization;
    using System.Linq;
    using System.Net;
    using System.Web;
    using Protocol;

    /// <summary>
    /// This class implements a resource query.
    /// </summary>
    public sealed class ResourceQuery : IResourceQuery
    {
        /// <summary>
        /// Contains separator attributes character.
        /// </summary>
        private const char SeparatorAttributes = ',';

        private static readonly Lazy<char[]> SeparatorsAttributes = new(() => new char[] { SeparatorAttributes });

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceQuery" /> class.
        /// </summary>
        public ResourceQuery()
        {
            this.Filters = Array.Empty<Filter>();
            this.Attributes = Array.Empty<string>();
            this.ExcludedAttributes = Array.Empty<string>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceQuery" /> class.
        /// </summary>
        /// <param name="filters">Contains query filters.</param>
        /// <param name="attributes">Contains requested attributes.</param>
        /// <param name="excludedAttributes">Contains excluded attributes.</param>
        /// <exception cref="ArgumentNullException">Exception is thrown if parameters are not specified.</exception>
        public ResourceQuery(IReadOnlyCollection<IFilter> filters, IReadOnlyCollection<string> attributes, IReadOnlyCollection<string> excludedAttributes)
        {
            this.Filters = filters ?? throw new ArgumentNullException(nameof(filters));
            this.Attributes = attributes ?? throw new ArgumentNullException(nameof(attributes));
            this.ExcludedAttributes = excludedAttributes ?? throw new ArgumentNullException(nameof(excludedAttributes));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceQuery" /> class.
        /// </summary>
        /// <param name="resource">Contains the resource Uri.</param>
        /// <exception cref="ArgumentNullException">Exception is thrown if parameters are not specified.</exception>
        public ResourceQuery(Uri resource)
        {
            if (resource == null)
            {
                throw new ArgumentNullException(nameof(resource));
            }

            string query = resource.Query;

            if (!string.IsNullOrWhiteSpace(query))
            {
                NameValueCollection keyedValues = HttpUtility.ParseQueryString(query);
                IEnumerable<string> keys = keyedValues.AllKeys;

                foreach (string key in keys)
                {
                    if (string.Equals(key, QueryKeys.Attributes, StringComparison.OrdinalIgnoreCase))
                    {
                        string attributeExpression = keyedValues[key];

                        if (!string.IsNullOrWhiteSpace(attributeExpression))
                        {
                            this.Attributes = ParseAttributes(attributeExpression);
                        }
                    }

                    if (string.Equals(key, QueryKeys.Count, StringComparison.OrdinalIgnoreCase))
                    {
                        void SetCountAction(IPaginationParameters pagination, int paginationValue) => pagination.Count = paginationValue;
                        this.ApplyPaginationParameter(keyedValues[key], SetCountAction);
                    }

                    if (string.Equals(key, QueryKeys.ExcludedAttributes, StringComparison.OrdinalIgnoreCase))
                    {
                        string attributeExpression = keyedValues[key];

                        if (!string.IsNullOrWhiteSpace(attributeExpression))
                        {
                            this.ExcludedAttributes = ParseAttributes(attributeExpression);
                        }
                    }

                    if (string.Equals(key, QueryKeys.Filter, StringComparison.OrdinalIgnoreCase))
                    {
                        string filterExpression = keyedValues[key];

                        if (!string.IsNullOrWhiteSpace(filterExpression))
                        {
                            this.Filters = ParseFilters(filterExpression);
                        }
                    }

                    if (string.Equals(key, QueryKeys.StartIndex, StringComparison.OrdinalIgnoreCase))
                    {
                        void SetStartIndexAction(IPaginationParameters pagination, int paginationValue) => pagination.StartIndex = paginationValue;
                        this.ApplyPaginationParameter(keyedValues[key], SetStartIndexAction);
                    }
                }
            }

            this.Filters ??= Array.Empty<Filter>();
            this.Attributes ??= Array.Empty<string>();
            this.ExcludedAttributes ??= Array.Empty<string>();
        }

        /// <inheritdoc />
        public IReadOnlyCollection<string> Attributes { get; }

        /// <inheritdoc />
        public IReadOnlyCollection<string> ExcludedAttributes { get; }

        /// <inheritdoc />
        public IReadOnlyCollection<IFilter> Filters { get; }

        /// <inheritdoc />
        public IPaginationParameters PaginationParameters { get; set; }

        /// <summary>
        /// This method is used to apply pagination parameter change.
        /// </summary>
        /// <param name="value">Contains the pagination value.</param>
        /// <param name="action">Contains a pagination parameter action.</param>
        /// <exception cref="ArgumentNullException"></exception>
        private void ApplyPaginationParameter(string value, Action<IPaginationParameters, int> action)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            if (!string.IsNullOrWhiteSpace(value))
            {
                int parsedValue = int.Parse(value, CultureInfo.InvariantCulture);
                this.PaginationParameters ??= new PaginationParameters();
                action(this.PaginationParameters, parsedValue);
            }
        }

        /// <summary>
        /// This method is used to parse attributes for a given expression.
        /// </summary>
        /// <param name="attributeExpression">Contains the attribute expression.</param>
        /// <returns>Returns the returned attributes found.</returns>
        /// <exception cref="ArgumentNullException">Exception is thrown if attribute expression is not specified.</exception>
        private static IReadOnlyCollection<string> ParseAttributes(string attributeExpression)
        {
            if (string.IsNullOrWhiteSpace(attributeExpression))
            {
                throw new ArgumentNullException(nameof(attributeExpression));
            }

            IReadOnlyCollection<string> results = attributeExpression.Split(SeparatorsAttributes.Value)
                .Select(item => item.Trim())
                .ToArray();

            return results;
        }

        /// <summary>
        /// This method is used to parse filters for a given expression.
        /// </summary>
        /// <param name="filterExpression">Contains the filter expression.</param>
        /// <returns>Returns the returned filters found.</returns>
        /// <exception cref="ArgumentNullException">Exception is thrown if filter expression is not specified.</exception>
        /// <exception cref="HttpResponseException">Expression is thrown if the expression cannot be parsed.</exception>
        private static IReadOnlyCollection<IFilter> ParseFilters(string filterExpression)
        {
            if (string.IsNullOrWhiteSpace(filterExpression))
            {
                throw new ArgumentNullException(nameof(filterExpression));
            }

            if (!Filter.TryParse(filterExpression, out IReadOnlyCollection<IFilter> results))
            {
                throw new HttpResponseException(HttpStatusCode.NotAcceptable);
            }

            return results;
        }
    }
}