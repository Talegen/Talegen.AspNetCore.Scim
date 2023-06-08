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
    using System.Collections.Generic;
    using System.Globalization;
    using System.Runtime.Serialization;
    using Schema;

    /// <summary>
    /// This class implements a SCIM v2 Patch operation object.
    /// </summary>
    [DataContract]
    public sealed class PatchOperation2 : PatchOperation2Base
    {
        /// <summary>
        /// Contains the class serialization template.
        /// </summary>
        private const string Template = "{0}: [{1}]";

        /// <summary>
        /// Contains the operation values.
        /// </summary>
        [DataMember(Name = AttributeNames.Value, Order = 2)]
        private List<OperationValue> values;

        /// <summary>
        /// Contains the values in a collection wrapper.
        /// </summary>
        private IReadOnlyCollection<OperationValue> valuesWrapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="PatchOperation2" /> class.
        /// </summary>
        public PatchOperation2()
        {
            this.OnInitialization();
            this.OnInitialized();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PatchOperation2" /> class.
        /// </summary>
        /// <param name="operationName">Contains the operation name.</param>
        /// <param name="pathExpression">Contains the patch expression.</param>
        public PatchOperation2(OperationName operationName, string pathExpression)
            : base(operationName, pathExpression)
        {
            this.OnInitialization();
            this.OnInitialized();
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        public IReadOnlyCollection<OperationValue> Value => this.valuesWrapper;

        /// <summary>
        /// This method is used to add an operation value.
        /// </summary>
        /// <param name="value">Contains the operation value to add.</param>
        /// <exception cref="ArgumentNullException">Exception is thrown if the value is not specified.</exception>
        public void AddValue(OperationValue value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            this.values.Add(value);
        }

        /// <summary>
        /// This method is used to create a new operation value.
        /// </summary>
        /// <param name="operationName">Contains the operation name.</param>
        /// <param name="pathExpression">Contains the path expression.</param>
        /// <param name="value">Contains the value.</param>
        /// <returns>Returns a new <see cref="PatchOperation2" /> object.</returns>
        /// <exception cref="ArgumentNullException">Exception is thrown if a path expression or value are not specified.</exception>
        public static PatchOperation2 Create(OperationName operationName, string pathExpression, string value)
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

            PatchOperation2 result = new PatchOperation2(operationName, pathExpression);
            result.AddValue(operationValue);

            return result;
        }

        /// <summary>
        /// This method is executed after the object is deserialized.
        /// </summary>
        /// <param name="context">Contains the streaming context.</param>
        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            this.OnInitialized();
        }

        /// <summary>
        /// This method is executed during deserialization.
        /// </summary>
        /// <param name="context">Contains the streaming context.</param>
        [OnDeserializing]
        private void OnDeserializing(StreamingContext context)
        {
            this.OnInitialization();
        }

        /// <summary>
        /// This method is called upon initialization.
        /// </summary>
        private void OnInitialization()
        {
            this.values = new List<OperationValue>();
        }

        /// <summary>
        /// This method is called after initialization.
        /// </summary>
        /// <exception cref="NotSupportedException">Exception is thrown if an operation value is not supported.</exception>
        private void OnInitialized()
        {
            switch (this.values)
            {
                case List<OperationValue> valueList:
                    this.valuesWrapper = valueList.AsReadOnly();
                    break;

                default:
                    throw new NotSupportedException(Schema.Properties.Resources.ExceptionInvalidValue);
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
            return string.Format(CultureInfo.InvariantCulture, PatchOperation2.Template, operation, allValues);
        }
    }
}