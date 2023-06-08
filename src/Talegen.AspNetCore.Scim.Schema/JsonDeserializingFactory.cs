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
    using System.Threading;

    /// <summary>
    /// Class JsonDeserializingFactory. Implements the <see cref="Talegen.AspNetCore.Scim.Schema.IJsonNormalizationBehavior" />
    /// </summary>
    /// <typeparam name="TDataContract">The type of the t data contract.</typeparam>
    /// <seealso cref="Talegen.AspNetCore.Scim.Schema.IJsonNormalizationBehavior" />
    public abstract class JsonDeserializingFactory<TDataContract> : IJsonNormalizationBehavior
    {
        /// <summary>
        /// The json serializer settings
        /// </summary>
        private static readonly Lazy<DataContractJsonSerializerSettings> JsonSerializerSettings = new Lazy<DataContractJsonSerializerSettings>(() =>
                    new DataContractJsonSerializerSettings()
                    {
                        EmitTypeInformation = EmitTypeInformation.Never
                    });

        /// <summary>
        /// The json serializer
        /// </summary>
        private static readonly Lazy<DataContractJsonSerializer> JsonSerializer = new Lazy<DataContractJsonSerializer>(() =>
                    new DataContractJsonSerializer(typeof(TDataContract), JsonSerializerSettings.Value));

        /// <summary>
        /// The json normalizer
        /// </summary>
        private IJsonNormalizationBehavior jsonNormalizer;

        /// <summary>
        /// Gets or sets a value indicating whether [accept large objects].
        /// </summary>
        /// <value><c>true</c> if [accept large objects]; otherwise, <c>false</c>.</value>
        public bool AcceptLargeObjects { get; set; }

        /// <summary>
        /// Gets the json normalizer.
        /// </summary>
        /// <value>The json normalizer.</value>
        public virtual IJsonNormalizationBehavior JsonNormalizer
        {
            get
            {
                IJsonNormalizationBehavior result = LazyInitializer.EnsureInitialized<IJsonNormalizationBehavior>(ref this.jsonNormalizer, () => new JsonNormalizer());
                return result;
            }
        }

        /// <summary>
        /// Creates the specified json.
        /// </summary>
        /// <param name="json">The json.</param>
        /// <returns>TDataContract.</returns>
        /// <exception cref="System.ArgumentNullException">json</exception>
        public virtual TDataContract Create(IReadOnlyDictionary<string, object> json)
        {
            if (null == json)
            {
                throw new ArgumentNullException(nameof(json));
            }

            IReadOnlyDictionary<string, object> normalizedJson = this.Normalize(json);
            string serialized = JsonFactory.Instance.Create(normalizedJson, this.AcceptLargeObjects);

            MemoryStream stream = null;

            try
            {
                stream = new MemoryStream();
                Stream streamed = stream;
                StreamWriter writer = null;
                try
                {
                    writer = new StreamWriter(stream);
                    stream = null;
                    writer.Write(serialized);
                    writer.Flush();

                    streamed.Position = 0;
                    TDataContract result =
                        (TDataContract)JsonSerializer.Value.ReadObject(
                            streamed);
                    return result;
                }
                finally
                {
                    if (writer != null)
                    {
                        writer.Close();
                        writer = null;
                    }
                }
            }
            finally
            {
                if (stream != null)
                {
                    stream.Close();
#pragma warning disable IDE0059 // Value assigned to symbol is never used
                    stream = null;
#pragma warning restore IDE0059 // Value assigned to symbol is never used
                }
            }
        }

        /// <summary>
        /// Creates the specified json.
        /// </summary>
        /// <param name="json">The json.</param>
        /// <returns>TDataContract.</returns>
        /// <exception cref="System.ArgumentNullException">json</exception>
        public virtual TDataContract Create(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                throw new ArgumentNullException(nameof(json));
            }

            IReadOnlyDictionary<string, object> keyedValues = JsonFactory.Instance.Create(json, this.AcceptLargeObjects);
            TDataContract result = this.Create(keyedValues);
            return result;
        }

        /// <summary>
        /// Normalizes the specified json.
        /// </summary>
        /// <param name="json">The json.</param>
        /// <returns>IReadOnlyDictionary&lt;System.String, System.Object&gt;.</returns>
        /// <exception cref="System.ArgumentNullException">json</exception>
        public IReadOnlyDictionary<string, object> Normalize(IReadOnlyDictionary<string, object> json)
        {
            if (null == json)
            {
                throw new ArgumentNullException(nameof(json));
            }

            IReadOnlyDictionary<string, object> result = this.JsonNormalizer.Normalize(json);
            return result;
        }

        /// <summary>
        /// Reads the specified json.
        /// </summary>
        /// <param name="json">The json.</param>
        /// <returns>TDataContract.</returns>
        public virtual TDataContract Read(string json)
        {
            MemoryStream stream = null;
            try
            {
                stream = new MemoryStream();
                Stream streamed = stream;
                StreamWriter writer = null;
                try
                {
                    writer = new StreamWriter(stream);
                    stream = null;
                    writer.Write(json);
                    writer.Flush();

                    streamed.Position = 0;
                    TDataContract result = (TDataContract)JsonSerializer.Value.ReadObject(streamed);
                    return result;
                }
                finally
                {
                    if (writer != null)
                    {
                        writer.Close();
                        writer = null;
                    }
                }
            }
            finally
            {
                if (stream != null)
                {
                    stream.Close();
#pragma warning disable IDE0059 // Value assigned to symbol is never used
                    stream = null;
#pragma warning restore IDE0059 // Value assigned to symbol is never used
                }
            }
        }
    }
}