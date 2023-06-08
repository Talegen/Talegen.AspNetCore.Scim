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
    using System.Runtime.Serialization;
    using Talegen.Common.Core.Extensions;

    /// <summary>
    /// This class represents the minimum representation of a bulk operation.
    /// </summary>
    [DataContract]
    public abstract class BulkOperation
    {
        /// <summary>
        /// Contains the HTTP method.
        /// </summary>
        private HttpMethod method;

        /// <summary>
        /// Contains the method name.
        /// </summary>
        private string methodName;

        /// <summary>
        /// Initializes a new instance of the <see cref="BulkOperation" /> class.
        /// </summary>
        protected BulkOperation()
            : this(Guid.NewGuid().ToString())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BulkOperation" /> class.
        /// </summary>
        /// <param name="identifier">Contains the operation identifier.</param>
        protected BulkOperation(string identifier)
        {
            this.Identifier = identifier;
        }

        /// <summary>
        /// Gets the identifier of the operation.
        /// </summary>
        [DataMember(Name = ProtocolAttributeNames.BulkOperationIdentifier, Order = 1)]
        public string Identifier { get; }

        /// <summary>
        /// Gets or sets the HTTP method.
        /// </summary>
        public HttpMethod Method
        {
            get => this.method;

            set
            {
                this.method = value;
                this.methodName = this.method.ConvertToString();
            }
        }

        /// <summary>
        /// Gets or sets the Method name.
        /// </summary>
        [DataMember(Name = ProtocolAttributeNames.Method, Order = 0)]
        private string MethodName
#pragma warning restore IDE0051 // Remove unused private members
        {
            get => this.methodName;

            set
            {
                this.method = new HttpMethod(value);
                this.methodName = value;
            }
        }
    }
}