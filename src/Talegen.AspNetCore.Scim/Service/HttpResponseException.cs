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

namespace Talegen.AspNetCore.Scim.Service
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Net;

    /// <summary>
    /// An exception that allows for a given <see cref="HttpResponseMessage" /> to be returned to the client.
    /// </summary>
    [SuppressMessage("Microsoft.Usage", "CA2237:MarkISerializableTypesWithSerializable", Justification = "This type is not meant to be serialized")]
    [SuppressMessage("Microsoft.Usage", "CA2240:Implement ISerializable correctly", Justification = "This type has no serializable state")]
    [SuppressMessage("Microsoft.Design", "CA1032:ImplementStandardExceptionConstructors", Justification = "HttpResponseException is not a real exception and is just an easy way to return HttpResponseMessage")]
    public class HttpResponseException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HttpResponseException" /> class.
        /// </summary>
        /// <param name="statusCode">Contains the HTTP status code.</param>
        public HttpResponseException(HttpStatusCode statusCode)
            : this(new HttpResponseMessage(statusCode))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpResponseException" /> class.
        /// </summary>
        /// <param name="response">The response message.</param>
        public HttpResponseException(HttpResponseMessage response)
            : base(Properties.Resources.HttpResponseExceptionMessage)
        {
            this.Response = response ?? throw new ArgumentNullException(nameof(response));
        }

        /// <summary>
        /// Gets the <see cref="HttpResponseMessage" /> to return to the client.
        /// </summary>
        public HttpResponseMessage Response { get; }
    }
}