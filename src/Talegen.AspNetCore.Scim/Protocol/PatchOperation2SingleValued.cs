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
    using Schema;

    /// <summary>
    /// This class implements the base patch operation for a single value.
    /// </summary>
    [DataContract]
    public sealed class PatchOperation2SingleValued : PatchOperation2Base
    {
        /// <summary>
        /// Contains the serialization template.
        /// </summary>
        private const string Template = "{0}: [{1}]";

        /// <summary>
        /// Contains the value.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Justification = "The serialized value is consumed.")]
        [DataMember(Name = AttributeNames.Value, Order = 2)]
        private string valueValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="PatchOperation2SingleValued" /> class.
        /// </summary>
        public PatchOperation2SingleValued()
        {
            this.OnInitialization();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PatchOperation2SingleValued" /> class.
        /// </summary>
        /// <param name="operationName">Contains the operation name.</param>
        /// <param name="pathExpression">Contains the path expression.</param>
        /// <param name="value">Contains the value.</param>
        /// <exception cref="ArgumentNullException">Exception is thrown if the value is not specified.</exception>
        public PatchOperation2SingleValued(OperationName operationName, string pathExpression, string value)
            : base(operationName, pathExpression)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentNullException(nameof(value));
            }

            this.valueValue = value;
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        public string Value => this.valueValue;

        /// <summary>
        /// This method is executed after the object is deserializing.
        /// </summary>
        /// <param name="context">Contains the streaming context.</param>
        [OnDeserializing]
        private void OnDeserializing(StreamingContext context)
        {
            this.OnInitialization();
        }

        /// <summary>
        /// This method is executed during initialization.
        /// </summary>
        private void OnInitialization()
        {
            this.valueValue = string.Empty;
        }

        /// <summary>
        /// This method is used to serialize the object to a string.
        /// </summary>
        /// <returns>Returns the serialized object string.</returns>
        public override string ToString()
        {
            string operation = base.ToString();
            return string.Format(CultureInfo.InvariantCulture, PatchOperation2SingleValued.Template, operation, this.valueValue);
        }
    }
}