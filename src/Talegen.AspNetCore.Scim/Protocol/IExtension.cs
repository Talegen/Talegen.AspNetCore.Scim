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
    using System.Net.Http;

    /// <summary>
    /// This interface defines the minimum implementation of an extension.
    /// </summary>
    public interface IExtension
    {
        /// <summary>
        /// Gets the controller type.
        /// </summary>
        Type Controller { get; }

        /// <summary>
        /// Gets the deserialization factory.
        /// </summary>
        JsonDeserializingFactory JsonDeserializingFactory { get; }

        /// <summary>
        /// Gets the path.
        /// </summary>
        string Path { get; }

        /// <summary>
        /// Gets the schema identifier.
        /// </summary>
        string SchemaIdentifier { get; }

        /// <summary>
        /// Gets the type name.
        /// </summary>
        string TypeName { get; }

        /// <summary>
        /// This method determines if a request is supported.
        /// </summary>
        /// <param name="request">Contains the HTTP request message to analyze.</param>
        /// <returns>Returns a value indicating whether the request is supported.</returns>
        bool Supports(HttpRequestMessage request);
    }
}