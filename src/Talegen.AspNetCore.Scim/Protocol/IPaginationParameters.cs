﻿/*
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
    /// This interface defines the minimum implementation of the pagination parameters class.
    /// </summary>
    public interface IPaginationParameters
    {
        /// <summary>
        /// Gets or sets the count of items.
        /// </summary>
        int? Count { get; set; }

        /// <summary>
        /// Gets or sets the starting index of items.
        /// </summary>
        int? StartIndex { get; set; }
    }
}