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
    using Schema;

    /// <summary>
    /// This interface defines the minimum implementation of a filter.
    /// </summary>
    public interface IFilter
    {
        /// <summary>
        /// Gets or sets an additional filter.
        /// </summary>
        IFilter AdditionalFilter { get; set; }

        /// <summary>
        /// Gets the attribute path.
        /// </summary>
        string AttributePath { get; }

        /// <summary>
        /// Gets the comparison value.
        /// </summary>
        string ComparisonValue { get; }

        /// <summary>
        /// Gets the comparison value encoded.
        /// </summary>
        string ComparisonValueEncoded { get; }

        /// <summary>
        /// Gets or sets the data type.
        /// </summary>
        AttributeDataType? DataType { get; set; }

        /// <summary>
        /// Gets or sets the filter operator.
        /// </summary>
        ComparisonOperator FilterOperator { get; }

        /// <summary>
        /// This method is used to serialize the filter to a string.
        /// </summary>
        /// <returns>Returns the serialized filter.</returns>
        string Serialize();
    }
}