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
    /// This class defines a minimal HTTP Response message factory for a given content type.
    /// </summary>
    /// <typeparam name="T">Contains the content type.</typeparam>
    internal abstract class HttpResponseMessageFactory<T>
    {
        /// <summary>
        /// This method is used to implement providing content.
        /// </summary>
        /// <param name="content">Contains the content.</param>
        /// <returns>Returns a new <see cref="HttpContent" /> object.</returns>
        public abstract HttpContent ProvideContent(T content);

        /// <summary>
        /// This method is used to create a new Http Response Message.
        /// </summary>
        /// <param name="statusCode">Contains the HTTP status code.</param>
        /// <param name="content">Contains the content.</param>
        /// <returns>Returns a new <see cref="HttpResponseMessage" /> object.</returns>
        public HttpResponseMessage CreateMessage(HttpStatusCode statusCode, T content)
        {
            HttpContent messageContent = null;
            HttpResponseMessage result = null;

            try
            {
                messageContent = this.ProvideContent(content);

                try
                {
                    result = new HttpResponseMessage(statusCode);
                    result.Content = messageContent;
                    messageContent = null;
                }
                catch
                {
                    if (result != null)
                    {
                        result.Dispose();
                        result = null;
                    };

                    throw;
                }
            }
            finally
            {
                if (messageContent != null)
                {
                    messageContent.Dispose();
                    messageContent = null;
                }
            }

            return result;
        }
    }
}