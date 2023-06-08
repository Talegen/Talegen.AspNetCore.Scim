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

namespace Talegen.AspNetCore.Scim.Service.Controllers
{
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using Microsoft.AspNetCore.Http;

    /// <summary>
    /// This class is used to convert context to request messages.
    /// </summary>
    public class HttpRequestMessageFeature
    {
        /// <summary>
        /// Contains the context object to convert.
        /// </summary>
        private readonly HttpContext httpContext;

        /// <summary>
        /// Contains the new request message.
        /// </summary>
        private HttpRequestMessage httpRequestMessage;

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpRequestMessageFeature" /> class.
        /// </summary>
        /// <param name="httpContext">Contains the context object to convert.</param>
        public HttpRequestMessageFeature(HttpContext httpContext)
        {
            this.httpContext = httpContext;
        }

        /// <summary>
        /// Gets or sets the HTTP request message object.
        /// </summary>
        public HttpRequestMessage HttpRequestMessage
        {
            get => this.httpRequestMessage ??= CreateHttpRequestMessage(this.httpContext);

            set => this.httpRequestMessage = value;
        }

        /// <summary>
        /// This method is used to create a request message object from a context object.
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        private static HttpRequestMessage CreateHttpRequestMessage(HttpContext httpContext)
        {
            var httpRequest = httpContext.Request;
            var uriString =
                httpRequest.Scheme + "://" +
                httpRequest.Host +
                httpRequest.PathBase +
                httpRequest.Path +
                httpRequest.QueryString;

            var message = new HttpRequestMessage(new HttpMethod(httpRequest.Method), uriString);

            // NOTE: Cumbersome as it is, Properties has been deprecated and Options is now the mechanism to use.
            ////message.Properties[nameof(HttpContext)] = httpContext;
            // This allows us to pass the message through APIs defined in legacy code and then operate on the HttpContext inside.
            if (!message.Options.TryAdd(nameof(HttpContext), httpContext))
            {
                message.Options.Remove(nameof(HttpContext), out _);
                if (!message.Options.TryAdd(nameof(HttpContext), httpContext))
                {
                    // give up if we fail to add.
                    throw new InvalidOperationException("Error attempting to add HTTP context to message Options.");
                }
            }

            message.Content = new StreamContent(httpRequest.Body);

            foreach (var header in httpRequest.Headers)
            {
                // Every header should be able to fit into one of the two header collections. Try message.Headers first since that accepts more of them.
                if (!message.Headers.TryAddWithoutValidation(header.Key, (string?)header.Value))
                {
                    var added = message.Content.Headers.TryAddWithoutValidation(header.Key, (string?)header.Value);
                    Contract.Assert(added);
                }
            }

            return message;
        }
    }
}