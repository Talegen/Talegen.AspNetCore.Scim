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
    using System.Collections.Specialized;
    using System.Globalization;
    using System.Linq;
    using System.Web;

    /// <summary>
    /// This class implements a query.
    /// </summary>
    public sealed class Query : IQuery
    {
        /// <summary>
        /// Contains the attribute name separator character.
        /// </summary>
        private const string AttributeNameSeparator = ",";

        /// <inheritdoc />
        public IReadOnlyCollection<IFilter> AlternateFilters { get; set; }

        /// <inheritdoc />
        public IReadOnlyCollection<string> ExcludedAttributePaths { get; set; }

        /// <inheritdoc />
        public IPaginationParameters PaginationParameters { get; set; }

        /// <inheritdoc />
        public string Path { get; set; }

        /// <inheritdoc />
        public IReadOnlyCollection<string> RequestedAttributePaths { get; set; }

        /// <inheritdoc />
        public string Compose()
        {
            string result = this.ToString();
            return result;
        }

        /// <summary>
        /// This method is used to close the filter object.
        /// </summary>
        /// <param name="filter">Contains the filter to clone.</param>
        /// <param name="placeHolders">Contains placeholders.</param>
        /// <returns>Returns a clone of filter object.</returns>
        private static Filter Clone(IFilter filter, Dictionary<string, string> placeHolders)
        {
            string placeHolder = Guid.NewGuid().ToString();
            placeHolders.Add(placeHolder, filter.ComparisonValueEncoded);
            Filter result = new Filter(filter.AttributePath, filter.FilterOperator, placeHolder);

            if (filter.AdditionalFilter != null)
            {
                result.AdditionalFilter = Query.Clone(filter.AdditionalFilter, placeHolders);
            }

            return result;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            NameValueCollection parameters = HttpUtility.ParseQueryString(string.Empty);

            if (this.RequestedAttributePaths?.Any() == true)
            {
                IReadOnlyCollection<string> encodedPaths = this.RequestedAttributePaths.Encode();
                string requestedAttributes = string.Join(Query.AttributeNameSeparator, encodedPaths);
                parameters.Add(QueryKeys.Attributes, requestedAttributes);
            }

            if (this.ExcludedAttributePaths?.Any() == true)
            {
                IReadOnlyCollection<string> encodedPaths = this.ExcludedAttributePaths.Encode();
                string excludedAttributes = string.Join(Query.AttributeNameSeparator, encodedPaths);
                parameters.Add(QueryKeys.ExcludedAttributes, excludedAttributes);
            }

            Dictionary<string, string> placeHolders;
            if (this.AlternateFilters?.Any() == true)
            {
                placeHolders = new Dictionary<string, string>(this.AlternateFilters.Count);
                IReadOnlyCollection<IFilter> clones = this.AlternateFilters.Select(item => Query.Clone(item, placeHolders)).ToArray();
                string filters = Filter.ToString(clones);
                NameValueCollection filterParameters = HttpUtility.ParseQueryString(filters);

                foreach (string key in filterParameters.AllKeys)
                {
                    parameters.Add(key, filterParameters[key]);
                }
            }
            else
            {
                placeHolders = new Dictionary<string, string>();
            }

            if (this.PaginationParameters != null)
            {
                if (this.PaginationParameters.StartIndex.HasValue)
                {
                    string startIndex = this.PaginationParameters.StartIndex.Value.ToString(CultureInfo.InvariantCulture);
                    parameters.Add(QueryKeys.StartIndex, startIndex);
                }

                if (this.PaginationParameters.Count.HasValue)
                {
                    string count = this.PaginationParameters.Count.Value.ToString(CultureInfo.InvariantCulture);
                    parameters.Add(QueryKeys.Count, count);
                }
            }

            string result = parameters.ToString();

            foreach (KeyValuePair<string, string> placeholder in placeHolders)
            {
                result = result.Replace(placeholder.Key, placeholder.Value, StringComparison.InvariantCulture);
            }

            return result;
        }
    }
}