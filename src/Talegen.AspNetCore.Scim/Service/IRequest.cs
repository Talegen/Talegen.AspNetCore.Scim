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
    using System.Collections.Generic;
    using System.Net.Http;
    using Protocol;

    /// <summary>
    /// This interface defines the minimum implementation of a request.
    /// </summary>
    public interface IRequest
    {
        /// <summary>
        /// Get the base resource identifier.
        /// </summary>
        Uri BaseResourceIdentifier { get; }

        /// <summary>
        /// Gets correlation identifier.
        /// </summary>
        string CorrelationIdentifier { get; }

        /// <summary>
        /// Gets a collection of extensions.
        /// </summary>
        IReadOnlyCollection<IExtension> Extensions { get; }

        /// <summary>
        /// Gets the HTTP request message.
        /// </summary>
        HttpRequestMessage Request { get; }
    }

    /// <summary>
    /// This interface defines the minimum implementation of a request with payload.
    /// </summary>
    /// <typeparam name="TPayload">Contains the payload type.</typeparam>
    public interface IRequest<out TPayload> : IRequest where TPayload : class
    {
        /// <summary>
        /// Gets the payload.
        /// </summary>
        TPayload Payload { get; }
    }
}