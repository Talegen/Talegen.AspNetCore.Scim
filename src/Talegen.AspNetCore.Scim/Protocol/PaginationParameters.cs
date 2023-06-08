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
    /// <summary>
    /// This class implements a pagination parameters object.
    /// </summary>
    public class PaginationParameters : IPaginationParameters
    {
        /// <summary>
        /// Contains the item count.
        /// </summary>
        private int? count;

        /// <summary>
        /// Contains the page start index.
        /// </summary>
        private int? startIndex;

        /// <summary>
        /// Gets or sets the item count.
        /// </summary>
        public int? Count
        {
            get => this.count;
            set => this.count = value is > 0 ? value.Value : 0;
        }

        /// <summary>
        /// </summary>
        public int? StartIndex
        {
            get => this.startIndex;
            set => this.startIndex = value is > 1 ? value.Value : 1;
        }
    }
}