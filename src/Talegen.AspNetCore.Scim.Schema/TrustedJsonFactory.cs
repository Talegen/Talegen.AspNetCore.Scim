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

namespace Talegen.AspNetCore.Scim.Schema
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    /// <summary>
    /// Class TrustedJsonFactory. Implements the <see cref="Talegen.AspNetCore.Scim.Schema.JsonFactory" />
    /// </summary>
    /// <seealso cref="Talegen.AspNetCore.Scim.Schema.JsonFactory" />
    public class TrustedJsonFactory : JsonFactory
    {
        /// <summary>
        /// This method is used to serialize an object to a dis
        /// </summary>
        /// <param name="json">Contains the JSON to convert to dictionary.</param>
        /// <returns>Returns a dictionary of key value pairs derived from the JSON model.</returns>
        public override Dictionary<string, object> Create(string json)
        {
            return JsonConvert.DeserializeObject(json) as Dictionary<string, object>;
        }

        /// <summary>
        /// This method is used to serialize an array of JSON values into a JSON model.
        /// </summary>
        /// <param name="input">Contains the JSON string values.</param>
        /// <returns>Returns a serialized JSON model.</returns>
        public override string Create(string[] input)
        {
            return JsonConvert.SerializeObject(input);
        }

        /// <summary>
        /// This method is used to serialize a dictionary of key value pairs into a JSON model.
        /// </summary>
        /// <param name="input">Contains the dictionary to serialize.</param>
        /// <returns>Returns a JSON model.</returns>
        public override string Create(Dictionary<string, object> input)
        {
            return JsonConvert.SerializeObject(input);
        }

        /// <summary>
        /// This method is used to serialize a dictionary of key value pairs into a JSON model.
        /// </summary>
        /// <param name="input">Contains the dictionary to serialize.</param>
        /// <returns>Returns a JSON model.</returns>
        public override string Create(IDictionary<string, object> input)
        {
            return JsonConvert.SerializeObject(input);
        }

        /// <summary>
        /// This method is used to serialize a dictionary of key value pairs into a JSON model.
        /// </summary>
        /// <param name="input">Contains the dictionary to serialize.</param>
        /// <returns>Returns a JSON model.</returns>
        public override string Create(IReadOnlyDictionary<string, object> input)
        {
            return JsonConvert.SerializeObject(input);
        }
    }
}