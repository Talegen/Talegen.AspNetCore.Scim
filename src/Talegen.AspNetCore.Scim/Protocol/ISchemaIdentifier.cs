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
    /// This interface defines the minimum implementation of a schema identifier.
    /// </summary>
    public interface ISchemaIdentifier
    {
        /// <summary>
        /// Gets the value.
        /// </summary>
        string Value { get; }

        /// <summary>
        /// This method is used to find the identifier path.
        /// </summary>
        /// <returns>Returns the identifier path.</returns>
        string FindPath();

        /// <summary>
        /// This method is used to find the path within the identifier.
        /// </summary>
        /// <param name="path">Returns the path if found.</param>
        /// <returns>Returns a value indicating whether the path was found.</returns>
        bool TryFindPath(out string path);
    }
}