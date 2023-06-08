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
    using System.Net;
    using System.Net.Http;

    /// <summary>
    /// This class defines a minimal HTTP Response exception factory for a given content type.
    /// </summary>
    /// <typeparam name="T">Contains the content type.</typeparam>
    internal abstract class HttpResponseExceptionFactory<T>
    {
        /// <summary>
        /// This method is used to implement a response message for a given HTTP status code.
        /// </summary>
        /// <param name="statusCode"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public abstract HttpResponseMessage ProvideMessage(HttpStatusCode statusCode, T content);

        /// <summary>
        /// This method is used to create a new HTTP Response exception for a given status code.
        /// </summary>
        /// <param name="statusCode">Contains the status code.</param>
        /// <param name="content">Contains content.</param>
        /// <returns>Returns a new <see cref="HttpResponseException" /> object.</returns>
        public HttpResponseException CreateException(HttpStatusCode statusCode, T content)
        {
            HttpResponseMessage message = null;
            HttpResponseException result;

            try
            {
                message = this.ProvideMessage(statusCode, content);
                result = new HttpResponseException(message);
                message = null; // this was an error with result = null;
            }
            finally
            {
                if (message != null)
                {
                    message.Dispose();
                    message = null;
                }
            }

            return result;
        }
    }
}