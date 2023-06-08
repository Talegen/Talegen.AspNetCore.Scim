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
    /// This class implements a SCIM request for a specific payload type.
    /// </summary>
    /// <typeparam name="TPayload">Defines the payload type.</typeparam>
    public abstract class SystemForCrossDomainIdentityManagementRequest<TPayload> : IRequest<TPayload>
        where TPayload : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SystemForCrossDomainIdentityManagementRequest{TPayload}" /> class.
        /// </summary>
        /// <param name="request">Contains the HTTP request message.</param>
        /// <param name="payload">Contains the payload.</param>
        /// <param name="correlationIdentifier">Contains a correlation identifier.</param>
        /// <param name="extensions">Contains extensions.</param>
        /// <exception cref="ArgumentNullException">Exception is thrown if a parameter is not specified.</exception>
        protected SystemForCrossDomainIdentityManagementRequest(HttpRequestMessage request, TPayload payload, string correlationIdentifier, IReadOnlyCollection<IExtension> extensions)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (string.IsNullOrWhiteSpace(correlationIdentifier))
            {
                throw new ArgumentNullException(nameof(extensions));
            }

            this.BaseResourceIdentifier = request.GetBaseResourceIdentifier();
            this.Request = request;
            this.Payload = payload ?? throw new ArgumentNullException(nameof(payload));
            this.CorrelationIdentifier = correlationIdentifier;
            this.Extensions = extensions;
        }

        /// <inheritdoc />
        public Uri BaseResourceIdentifier { get; }

        /// <inheritdoc />
        public string CorrelationIdentifier { get; }

        /// <inheritdoc />
        public IReadOnlyCollection<IExtension> Extensions { get; }

        /// <inheritdoc />
        public TPayload Payload { get; }

        /// <inheritdoc />
        public HttpRequestMessage Request { get; }
    }
}