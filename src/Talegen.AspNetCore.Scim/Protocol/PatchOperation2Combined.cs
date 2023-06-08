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
    using System.Runtime.Serialization;
    using Newtonsoft.Json;
    using Schema;

    /// <summary>
    /// This class implements the base patch operation into a combined operation.
    /// </summary>
    [DataContract]
    public sealed class PatchOperation2Combined : PatchOperation2Base
    {
        /// <summary>
        /// Contains the serialization template.
        /// </summary>
        private const string Template = "{0}: [{1}]";

        /// <summary>
        /// Contains the values.
        /// </summary>
        [DataMember(Name = AttributeNames.Value, Order = 2)]
        private object values;

        /// <summary>
        /// Initializes a new instance of the <see cref="PatchOperation2Combined" /> class.
        /// </summary>
        public PatchOperation2Combined()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PatchOperation2Combined" /> class.
        /// </summary>
        /// <param name="operationName">Contains the operation name.</param>
        /// <param name="pathExpression">Contains the path expression.</param>
        public PatchOperation2Combined(OperationName operationName, string pathExpression)
            : base(operationName, pathExpression)
        {
        }

        /// <summary>
        /// This method is used to create a new combined path operation.
        /// </summary>
        /// <param name="operationName">Contains the operation name.</param>
        /// <param name="pathExpression">Contains the path expression.</param>
        /// <param name="value">Contains the value.</param>
        /// <returns>Returns a new <see cref="PatchOperation2Combined" /> object.</returns>
        /// <exception cref="ArgumentNullException">Exception is thrown if the path expression or value are not specified.</exception>
        public static PatchOperation2Combined Create(OperationName operationName, string pathExpression, string value)
        {
            if (string.IsNullOrWhiteSpace(pathExpression))
            {
                throw new ArgumentNullException(nameof(pathExpression));
            }

            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentNullException(nameof(value));
            }

            OperationValue operationValue = new OperationValue
            {
                Value = value
            };

            PatchOperation2Combined result = new PatchOperation2Combined(operationName, pathExpression)
            {
                Value = JsonConvert.SerializeObject(operationValue)
            };

            return result;
        }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        public string Value
        {
            get => JsonConvert.SerializeObject(this.values);
            set => this.values = value;
        }

        /// <summary>
        /// This method is executed after the object is deserialized.
        /// </summary>
        /// <param name="context">Contains the streaming context.</param>
        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            if (this?.Path?.AttributePath != null && this.Path.AttributePath.Contains(AttributeNames.Members, StringComparison.OrdinalIgnoreCase) &&
                this.Name == Protocol.OperationName.Remove && this.Path?.SubAttributes?.Count == 1)
            {
                this.Value = this.Path.SubAttributes.First().ComparisonValue;
                IPath path = Protocol.Path.Create(AttributeNames.Members);
                this.Path = path;
            }
        }

        /// <summary>
        /// This method is used to serialize the object to a string.
        /// </summary>
        /// <returns>Returns the serialized object string.</returns>
        public override string ToString()
        {
            string allValues = string.Join(Environment.NewLine, this.Value);
            string operation = base.ToString();
            return string.Format(CultureInfo.InvariantCulture, PatchOperation2Combined.Template, operation, allValues);
        }
    }
}