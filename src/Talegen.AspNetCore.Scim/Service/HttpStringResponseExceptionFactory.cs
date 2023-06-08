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
    using System.Net;
    using System.Net.Http;

    /// <summary>
    /// This class implements a new String response exception factory.
    /// </summary>
    internal class HttpStringResponseExceptionFactory : HttpResponseExceptionFactory<string>
    {
        /// <summary>
        /// Contains the content argument name.
        /// </summary>
        private const string ArgumentNameContent = "content";

        /// <summary>
        /// Contains the response message factory.
        /// </summary>
        private static readonly Lazy<HttpResponseMessageFactory<string>> ResponseMessageFactory = new(() => new HttpStringResponseMessageFactory());

        /// <summary>
        /// This method is used to provide a new response message.
        /// </summary>
        /// <param name="statusCode">Contains the status code.</param>
        /// <param name="content">Contains the content to set in message.</param>
        /// <returns>Returns a new <see cref="HttpResponseMessage" /> object.</returns>
        /// <exception cref="ArgumentNullException">Exception is thrown if the content is not specified.</exception>
        public override HttpResponseMessage ProvideMessage(HttpStatusCode statusCode, string content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                throw new ArgumentNullException(HttpStringResponseExceptionFactory.ArgumentNameContent);
            }

            HttpResponseMessage result = null;

            try
            {
                result = HttpStringResponseExceptionFactory.ResponseMessageFactory.Value.CreateMessage(statusCode, content);
            }
            catch
            {
                if (result != null)
                {
                    result.Dispose();
                    result = null;
                }

                throw;
            }

            return result;
        }
    }
}