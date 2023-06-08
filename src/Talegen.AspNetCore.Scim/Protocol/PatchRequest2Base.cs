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
    using System.Runtime.Serialization;

    /// <summary>
    /// This class defines the base abstract class for SCIM v2 patch requests of a specified operation type.
    /// </summary>
    /// <typeparam name="TOperation">Defines the operation type.</typeparam>
    [DataContract]
    public abstract class PatchRequest2Base<TOperation> : PatchRequestBase
        where TOperation : PatchOperation2Base
    {
        /// <summary>
        /// Contains the operations values.
        /// </summary>
        [DataMember(Name = ProtocolAttributeNames.Operations, Order = 2)]
        private List<TOperation> operationsValue;

        /// <summary>
        /// Contains a collection of operations.
        /// </summary>
        private IReadOnlyCollection<TOperation> operationsWrapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="PatchRequest2Base{TOperation}" /> class.
        /// </summary>
        protected PatchRequest2Base()
        {
            this.OnInitialization();
            this.OnInitialized();
            this.AddSchema(ProtocolSchemaIdentifiers.Version2PatchOperation);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PatchRequest2Base{TOperation}" /> class.
        /// </summary>
        /// <param name="operations">Contains the initial patch operations.</param>
        protected PatchRequest2Base(IReadOnlyCollection<TOperation> operations)
            : this()
        {
            this.operationsValue.AddRange(operations);
        }

        /// <summary>
        /// Gets the patch operations.
        /// </summary>
        public IReadOnlyCollection<TOperation> Operations => this.operationsWrapper;

        /// <summary>
        /// This method is used to add an operation value.
        /// </summary>
        /// <param name="operation">Contains the operation value to add.</param>
        /// <exception cref="ArgumentNullException">Exception is thrown if the value is not specified.</exception>
        public void AddOperation(TOperation operation)
        {
            if (null == operation)
            {
                throw new ArgumentNullException(nameof(operation));
            }

            this.operationsValue.Add(operation);
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
            this.operationsValue = new List<TOperation>();
        }

        /// <summary>
        /// This method is called after initialization.
        /// </summary>
        private void OnInitialized()
        {
            this.operationsWrapper = this.operationsValue.AsReadOnly();
        }
    }
}