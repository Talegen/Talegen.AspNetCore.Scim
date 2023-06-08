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
    /// Interface ISchematizedJsonDeserializingFactory
    /// </summary>
    /// <typeparam name="TOutput">The type of the t output.</typeparam>
    public interface ISchematizedJsonDeserializingFactory<out TOutput> where TOutput : Schematized
    {
        /// <summary>
        /// This method is used to create a new deserializer of the type <typeparamref name="TOutput" />.
        /// </summary>
        /// <param name="json">Contains the dictionary to deserialize.</param>
        /// <returns>Returns a deserialized instance of type <typeparamref name="TOutput" />.</returns>
        TOutput Create(IReadOnlyDictionary<string, object> json);
    }
}