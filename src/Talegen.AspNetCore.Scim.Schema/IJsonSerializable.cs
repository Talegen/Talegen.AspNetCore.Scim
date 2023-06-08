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
    using System.Collections.Generic;

    /// <summary>
    /// This interface defines the minimum implementation of a JSON serialized object.
    /// </summary>
    public interface IJsonSerializable
    {
        /// <summary>
        /// This method is used to convert the object to JSON.
        /// </summary>
        /// <returns>Returns a dictionary of JSON</returns>
        Dictionary<string, object> ToJson();

        /// <summary>
        /// This method is used to serialize the object to JSON.
        /// </summary>
        /// <returns>Returns the serialized data.</returns>
        string Serialize();
    }
}