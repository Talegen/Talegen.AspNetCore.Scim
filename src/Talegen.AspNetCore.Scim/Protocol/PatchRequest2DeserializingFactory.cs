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
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Schema;

    /// <summary>
    /// This class implements a base abstraction for a SCIM v2 Patch request deserialization factory for given patch request and operation types.
    /// </summary>
    /// <typeparam name="TPatchRequest">The patch request type.</typeparam>
    /// <typeparam name="TOperation">The operation type.</typeparam>
    public abstract class PatchRequest2DeserializingFactory<TPatchRequest, TOperation> :
        ProtocolJsonDeserializingFactory<TPatchRequest>,
        ISchematizedJsonDeserializingFactory<TPatchRequest>
        where TOperation : PatchOperation2Base
        where TPatchRequest : PatchRequest2Base<TOperation>
    {
        /// <summary>
        /// This method is used to create a new deserializer of the type <typeparamref name="TPatchRequest" />.
        /// </summary>
        /// <param name="json">Contains the dictionary to deserialize.</param>
        /// <returns>Returns a deserialized instance of type <typeparamref name="TPatchRequest" />.</returns>
        public override TPatchRequest Create(IReadOnlyDictionary<string, object> json)
        {
            Dictionary<string, object> normalized = this.Normalize(json).ToDictionary(item => item.Key, item => item.Value);

            if (normalized.TryGetValue(ProtocolAttributeNames.Operations, out object operations))
            {
                normalized.Remove(ProtocolAttributeNames.Operations);
            }

            TPatchRequest result = base.Create(normalized);

            if (operations != null)
            {
                IReadOnlyCollection<PatchOperation2Base> patchOperations = Deserialize(operations);

                foreach (PatchOperation2Base patchOperation in patchOperations)
                {
                    result.AddOperation(patchOperation as TOperation);
                }
            }

            return result;
        }

        /// <summary>
        /// This method is used to try and deserialize the specified dictionary to a <see cref="PatchOperation2Base" /> operation object.
        /// </summary>
        /// <param name="json">Contains the dictionary to deserialize.</param>
        /// <param name="operation">Contains the output operation result.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Exception is thrown if the source dictionary is not specified.</exception>
        /// <exception cref="NotSupportedException">Exception is thrown if a given value is not supported.</exception>
        private static bool TryDeserialize(Dictionary<string, object> json, out PatchOperation2Base operation)
        {
            operation = null;
            bool result;

            if (json == null)
            {
                throw new ArgumentNullException(nameof(json));
            }

            if (result = json.TryGetValue(AttributeNames.Value, out object value))
            {
                switch (value)
                {
                    case string scalar:
                        operation = new PatchOperation2SingleValuedJsonDeserializingFactory().Create(json);

                        result = true;
                        break;

                    case ArrayList _:
                    case not null:
                        operation = new PatchOperation2JsonDeserializingFactory().Create(json);

                        result = true;
                        break;

                    default:
                        string unsupported = value.GetType().FullName;
                        throw new NotSupportedException(unsupported);
                }
            }

            return result;
        }

        /// <summary>
        /// This method is used to deserialize the array of operations.
        /// </summary>
        /// <param name="operations">Contains the array of operations to deserialize.</param>
        /// <returns>Returns a new collection of patch operations.</returns>
        /// <exception cref="ArgumentNullException">Exception is thrown if the operations are not specified.</exception>
        private static IReadOnlyCollection<PatchOperation2Base> Deserialize(ArrayList operations)
        {
            if (operations == null)
            {
                throw new ArgumentNullException(nameof(operations));
            }

            List<PatchOperation2Base> result = new List<PatchOperation2Base>(operations.Count);

            foreach (Dictionary<string, object> json in operations)
            {
                if (TryDeserialize(json, out PatchOperation2Base patchOperation))
                {
                    result.Add(patchOperation);
                }
            }

            return result;
        }

        /// <summary>
        /// This method is used to deserialize the array of operations.
        /// </summary>
        /// <param name="operations">Contains the array of operations to deserialize.</param>
        /// <returns>Returns a new collection of patch operations.</returns>
        /// <exception cref="ArgumentNullException">Exception is thrown if the operations are not specified.</exception>
        private static IReadOnlyCollection<PatchOperation2Base> Deserialize(object[] operations)
        {
            if (operations == null)
            {
                throw new ArgumentNullException(nameof(operations));
            }

            List<PatchOperation2Base> result = new List<PatchOperation2Base>(operations.Length);

            foreach (Dictionary<string, object> json in operations)
            {
                if (TryDeserialize(json, out PatchOperation2Base patchOperation))
                {
                    result.Add(patchOperation);
                }
            }

            return result;
        }

        /// <summary>
        /// This method is used to deserialize the object of operations.
        /// </summary>
        /// <param name="operations">Contains the object of operations to deserialize.</param>
        /// <returns>Returns a new collection of patch operations.</returns>
        /// <exception cref="ArgumentNullException">Exception is thrown if the operations are not specified.</exception>
        private static IReadOnlyCollection<PatchOperation2Base> Deserialize(object operations)
        {
            IReadOnlyCollection<PatchOperation2Base> result;

            switch (operations)
            {
                case ArrayList list:
                    result = Deserialize(list);

                    break;

                case object[] array:
                    result = Deserialize(array);
                    break;

                default:
                    string unsupported = operations.GetType().FullName;
                    throw new NotSupportedException(unsupported);
            }

            return result;
        }
    }
}