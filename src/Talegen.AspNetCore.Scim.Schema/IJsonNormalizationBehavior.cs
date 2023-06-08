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

namespace Talegen.AspNetCore.Scim.Schema
{
    using System.Collections.Generic;

    /// <summary>
    /// Interface IJsonNormalizationBehavior
    /// </summary>
    public interface IJsonNormalizationBehavior
    {
        /// <summary>
        /// Normalizes the specified json.
        /// </summary>
        /// <param name="json">The json.</param>
        /// <returns>IReadOnlyDictionary&lt;System.String, System.Object&gt;.</returns>
        IReadOnlyDictionary<string, object> Normalize(IReadOnlyDictionary<string, object> json);
    }
}