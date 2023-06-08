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
    /// <summary>
    /// Contains an enumerated list of comparison operators.
    /// </summary>
    public enum ComparisonOperator
    {
        /// <summary>
        /// The bit and
        /// </summary>
        BitAnd,

        /// <summary>
        /// The ends with
        /// </summary>
        EndsWith,

        /// <summary>
        /// The equals
        /// </summary>
        Equals,

        /// <summary>
        /// The equal or greater than
        /// </summary>
        EqualOrGreaterThan,

        /// <summary>
        /// The greater than
        /// </summary>
        GreaterThan,

        /// <summary>
        /// The equal or less than
        /// </summary>
        EqualOrLessThan,

        /// <summary>
        /// The less than
        /// </summary>
        LessThan,

        /// <summary>
        /// The includes
        /// </summary>
        Includes,

        /// <summary>
        /// The is member of
        /// </summary>
        IsMemberOf,

        /// <summary>
        /// The matches expression
        /// </summary>
        MatchesExpression,

        /// <summary>
        /// The not bit and
        /// </summary>
        NotBitAnd,

        /// <summary>
        /// The not equals
        /// </summary>
        NotEquals,

        /// <summary>
        /// The not matches expression
        /// </summary>
        NotMatchesExpression
    }
}