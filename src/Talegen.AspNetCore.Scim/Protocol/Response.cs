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
    using System.Globalization;
    using System.Linq;
    using System.Net;
    using Talegen.Common.Core.Extensions;

    /// <summary>
    /// This class implements a response.
    /// </summary>
    internal class Response : IResponse
    {
        /// <summary>
        /// Contains the lock.
        /// </summary>
        private readonly object thisLock = new object();

        /// <summary>
        /// Contains an HTTP response class.
        /// </summary>
        private HttpResponseClass responseClass;

        /// <summary>
        /// Contains a HTTP status code.
        /// </summary>
        private HttpStatusCode statusCode;

        /// <summary>
        /// Contains the HTTP status code value.
        /// </summary>
        private string statusCodeValue;

        /// <summary>
        /// Contains an enumerated list of Response classes.
        /// </summary>
        private enum HttpResponseClass
        {
            /// <summary>
            /// Informational.
            /// </summary>
            Informational = 1,

            /// <summary>
            /// Success.
            /// </summary>
            Success = 2,

            /// <summary>
            /// Redirection.
            /// </summary>
            Redirection = 3,

            /// <summary>
            /// Client error.
            /// </summary>
            ClientError = 4,

            /// <summary>
            /// Server error.
            /// </summary>
            ServerError = 5
        }

        /// <summary>
        /// Gets or sets the HTTP status code.
        /// </summary>
        public HttpStatusCode Status
        {
            get => this.statusCode;

            set => this.StatusCodeValue = ((int)value).ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Gets or sets the status code value.
        /// </summary>
        public string StatusCodeValue
        {
            get => this.statusCodeValue;

            set
            {
                lock (this.thisLock)
                {
                    this.statusCode = (HttpStatusCode)Enum.Parse(typeof(HttpStatusCode), value);
                    this.statusCodeValue = value;
                    char responseClassSignifier = this.statusCodeValue.First();
                    double responseClassNumber = char.GetNumericValue(responseClassSignifier);
                    int responseClassCode = Convert.ToInt32(responseClassNumber);
                    this.responseClass = (HttpResponseClass)Enum.ToObject(typeof(HttpResponseClass), responseClassCode);
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether the response is an error.
        /// </summary>
        /// <returns>Returns a value indicating whether the response is an error.</returns>
        public bool IsError()
        {
            bool result = this.responseClass is HttpResponseClass.ClientError or HttpResponseClass.ServerError;
            return result;
        }
    }
}