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
    using System.Runtime.Serialization;

    /// <summary>
    /// This abstract class implements the base implementation of the <see cref="IPatchOperation2Base" /> interface.
    /// </summary>
    [DataContract]
    public abstract class PatchOperation2Base : IPatchOperation2Base
    {
        /// <summary>
        /// Contains the template for serialization.
        /// </summary>
        private const string Template = "{0} {1}";

        /// <summary>
        /// Contains the operation name.
        /// </summary>
        private OperationName name;

        /// <summary>
        /// Contains the string version of the operation name.
        /// </summary>
        private string operationName;

        /// <summary>
        /// Contains the path.
        /// </summary>
        private IPath path;

        /// <summary>
        /// Contains the path expression.
        /// </summary>
        [DataMember(Name = ProtocolAttributeNames.Path, Order = 1)]
        private string pathExpression;

        /// <summary>
        /// Initializes a new instance of the <see cref="PatchOperation2Base" /> class.
        /// </summary>
        protected PatchOperation2Base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PatchOperation2Base" /> class.
        /// </summary>
        /// <param name="operationName">Contains the operation name.</param>
        /// <param name="pathExpression">Contains the path expression.</param>
        /// <exception cref="ArgumentNullException">Exception is thrown if the path expression is not specified.</exception>
        protected PatchOperation2Base(OperationName operationName, string pathExpression)
        {
            if (string.IsNullOrWhiteSpace(pathExpression))
            {
                throw new ArgumentNullException(nameof(pathExpression));
            }

            this.Name = operationName;
            this.Path = Protocol.Path.Create(pathExpression);
        }

        /// <summary>
        /// Gets or sets the operation name.
        /// </summary>
        public OperationName Name
        {
            get => this.name;

            set
            {
                this.name = value;
                this.operationName = Enum.GetName(typeof(OperationName), value);
            }
        }

        /// <summary>
        /// Gets or sets the operation name.
        /// </summary>
        /// <remarks>It's the value of 'op' parameter within the JSON of request body.</remarks>
        [DataMember(Name = ProtocolAttributeNames.Patch2Operation, Order = 0)]
        public string OperationName
        {
            get => this.operationName;

            set
            {
                if (!Enum.TryParse(value, true, out this.name))
                {
                    throw new NotSupportedException();
                }

                this.operationName = value;
            }
        }

        /// <summary>
        /// Gets or sets the path.
        /// </summary>
        public IPath Path
        {
            get
            {
                if (this.path == null && !string.IsNullOrWhiteSpace(this.pathExpression))
                {
                    this.path = Protocol.Path.Create(this.pathExpression);
                }

                return this.path;
            }

            set
            {
                this.pathExpression = value?.ToString();
                this.path = value;
            }
        }

        /// <summary>
        /// This method is used to serialize the object to a string.
        /// </summary>
        /// <returns>Returns the serialized string.</returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, PatchOperation2Base.Template, this.operationName, this.pathExpression);
        }
    }
}