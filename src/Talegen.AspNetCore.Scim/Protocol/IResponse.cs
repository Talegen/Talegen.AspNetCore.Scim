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
    using System.Net;

    /// <summary>
    /// This interface defines the minimum implementation of a response object.
    /// </summary>
    internal interface IResponse
    {
        /// <summary>
        /// Gets or sets the HTTP status.
        /// </summary>
        HttpStatusCode Status { get; set; }

        /// <summary>
        /// Gets or sets the HTTP status code string value.
        /// </summary>
        string StatusCodeValue { get; set; }

        /// <summary>
        /// This method determines whether the response is an error.
        /// </summary>
        /// <returns>Returns a value indicating whether the response is an error.</returns>
        bool IsError();
    }
}