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
    using System.Linq;
    using System.Runtime.Serialization;
    using Schema;

    /// <summary>
    /// This class represents the minimum implementation of a bulk operations class.
    /// </summary>
    /// <typeparam name="TOperation">Contains the type of operation.</typeparam>
    [DataContract]
    public abstract class BulkOperations<TOperation> : Schematized where TOperation : BulkOperation
    {
        /// <summary>
        /// Contains a list of operations.
        /// </summary>
        [DataMember(Name = ProtocolAttributeNames.Operations, Order = 2)]
        private List<TOperation> operations;

        /// <summary>
        /// Contains a collection of operations of type TOperation.
        /// </summary>
        private IReadOnlyCollection<TOperation> operationsWrapper;

        /// <summary>
        /// Contains a thread safe lock.
        /// </summary>
        private object thisLock;

        /// <summary>
        /// Initializes a new instance of the <see cref="BulkOperations{TOperation}" /> class.
        /// </summary>
        /// <param name="schemaIdentifier">Contains the schema identifier.</param>
        /// <exception cref="ArgumentNullException">Exception is thrown if no schema identifier is specified.</exception>
        protected BulkOperations(string schemaIdentifier)
        {
            if (string.IsNullOrWhiteSpace(schemaIdentifier))
            {
                throw new ArgumentNullException(nameof(schemaIdentifier));
            }

            this.AddSchema(schemaIdentifier);
            this.OnInitialization();
            this.OnInitialized();
        }

        /// <summary>
        /// Gets the collection of operations.
        /// </summary>
        public IReadOnlyCollection<TOperation> Operations => this.operationsWrapper;

        /// <summary>
        /// This method is used to add an operation.
        /// </summary>
        /// <param name="operation">Contains the operation to add.</param>
        /// <exception cref="ArgumentNullException">Exception is thrown if no operation is specified.</exception>
        /// <exception cref="ArgumentException">Exception is thrown if no identifier in the operation is specified.</exception>
        public void AddOperation(TOperation operation)
        {
            if (operation == null)
            {
                throw new ArgumentNullException(nameof(operation));
            }

            if (string.IsNullOrWhiteSpace(operation.Identifier))
            {
                throw new ArgumentException(Schema.Properties.Resources.ExceptionUnidentifiableOperation);
            }

            // create a local check operation...
            bool Contains() => this.operations.Any((BulkOperation item) => string.Equals(item.Identifier, operation.Identifier, StringComparison.OrdinalIgnoreCase));

            if (!Contains())
            {
                lock (this.thisLock)
                {
                    if (!Contains())
                    {
                        this.operations.Add(operation);
                    }
                }
            }
        }

        /// <summary>
        /// This operation calls initialized upon deserialization.
        /// </summary>
        /// <param name="_">Streaming Context.</param>
        [OnDeserialized]
        private void OnDeserialized(StreamingContext _) => this.OnInitialized();

        /// <summary>
        /// This operation calls initialize upon deserializing.
        /// </summary>
        /// <param name="_">Streaming Context.</param>
        [OnDeserializing]
        private void OnDeserializing(StreamingContext _) => this.OnInitialization();

        /// <summary>
        /// This method is used to initialize the object upon deserialization.
        /// </summary>
        private void OnInitialization()
        {
            this.thisLock = new object();
            this.operations = new List<TOperation>();
        }

        /// <summary>
        /// This method is called to initialize operations upon deserialization completion.
        /// </summary>
        private void OnInitialized() => this.operationsWrapper = this.operations.AsReadOnly();
    }
}