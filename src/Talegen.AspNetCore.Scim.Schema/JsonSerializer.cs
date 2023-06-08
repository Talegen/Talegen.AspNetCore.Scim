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
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Json;

    /// <summary>
    /// Class JsonSerializer. Implements the <see cref="Talegen.AspNetCore.Scim.Schema.IJsonSerializable" />
    /// </summary>
    /// <seealso cref="Talegen.AspNetCore.Scim.Schema.IJsonSerializable" />
    internal class JsonSerializer : IJsonSerializable
    {
        /// <summary>
        /// Contains the serialize settings.
        /// </summary>
        private static readonly Lazy<DataContractJsonSerializerSettings> SerializerSettings = new Lazy<DataContractJsonSerializerSettings>(() =>
            new DataContractJsonSerializerSettings()
            {
                EmitTypeInformation = EmitTypeInformation.Never
            });

        /// <summary>
        /// Contains the data contract value.
        /// </summary>
        private readonly object dataContractValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonSerializer" /> class.
        /// </summary>
        /// <param name="dataContract">Contains the data contract object to serialize.</param>
        /// <exception cref="System.ArgumentNullException">dataContract</exception>
        public JsonSerializer(object dataContract)
        {
            this.dataContractValue = dataContract ?? throw new ArgumentNullException(nameof(dataContract));
        }

        /// <summary>
        /// This method is used to serialize the object to JSON.
        /// </summary>
        /// <returns>Returns the serialized data.</returns>
        public string Serialize()
        {
            IDictionary<string, object> json = this.ToJson();
            string result = JsonFactory.Instance.Create(json, true);
            return result;
        }

        /// <summary>
        /// This method is used to convert the object to JSON.
        /// </summary>
        /// <returns>Returns a dictionary of JSON</returns>
        public Dictionary<string, object> ToJson()
        {
            Type type = this.dataContractValue.GetType();
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(type, SerializerSettings.Value);
            string json;
            using MemoryStream stream = new MemoryStream();

            try
            {
                serializer.WriteObject(stream, this.dataContractValue);
                stream.Position = 0;
                using StreamReader streamReader = new StreamReader(stream);

                try
                {
                    json = streamReader.ReadToEnd();
                }
                finally
                {
                    streamReader.Close();
                }
            }
            finally
            {
                stream.Close();
            }

            return JsonFactory.Instance.Create(json, true);
        }
    }
}