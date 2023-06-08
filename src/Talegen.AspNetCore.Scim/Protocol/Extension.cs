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
    using System.Net.Http;

    /// <summary>
    /// This class implements the minimum implementation of an extension.
    /// </summary>
    public abstract class Extension : IExtension
    {
        /// <summary>
        /// Contains the argument name controller.
        /// </summary>
        private const string ArgumentNameController = "controller";

        /// <summary>
        /// Contains the argument name deserializing factory.
        /// </summary>
        private const string ArgumentNameJsonDeserializingFactory = "jsonDeserializingFactory";

        /// <summary>
        /// Contains the argument name path.
        /// </summary>
        private const string ArgumentNamePath = "path";

        /// <summary>
        /// Contains the argument name schema identifier.
        /// </summary>
        private const string ArgumentNameSchemaIdentifier = "schemaIdentifier";

        /// <summary>
        /// Contains the argument name type name.
        /// </summary>
        private const string ArgumentNameTypeName = "typeName";

        /// <summary>
        /// Initializes a new instance of the <see cref="Extension" /> class.
        /// </summary>
        /// <param name="schemaIdentifier">Contains the schema identifier.</param>
        /// <param name="typeName">Contains the type name.</param>
        /// <param name="path">Contains the path.</param>
        /// <param name="controller">Contains the controller.</param>
        /// <param name="jsonDeserializingFactory">Contains the JSON deserializing factory.</param>
        /// <exception cref="ArgumentNullException">Exception is thrown if any arguments are not specified.</exception>
        protected Extension(string schemaIdentifier, string typeName, string path, Type controller, JsonDeserializingFactory jsonDeserializingFactory)
        {
            if (string.IsNullOrWhiteSpace(schemaIdentifier))
            {
                throw new ArgumentNullException(Extension.ArgumentNameSchemaIdentifier);
            }

            if (string.IsNullOrWhiteSpace(typeName))
            {
                throw new ArgumentNullException(Extension.ArgumentNameTypeName);
            }

            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentNullException(Extension.ArgumentNamePath);
            }

            this.SchemaIdentifier = schemaIdentifier;
            this.TypeName = typeName;
            this.Path = path;
            this.Controller = controller ?? throw new ArgumentNullException(Extension.ArgumentNameController);
            this.JsonDeserializingFactory = jsonDeserializingFactory ?? throw new ArgumentNullException(Extension.ArgumentNameJsonDeserializingFactory);
        }

        /// <summary>
        /// Gets the controller type.
        /// </summary>
        public Type Controller { get; }

        /// <summary>
        /// Gets the JSON deserializing factory.
        /// </summary>
        public JsonDeserializingFactory JsonDeserializingFactory { get; }

        /// <summary>
        /// Gets the path.
        /// </summary>
        public string Path { get; }

        /// <summary>
        /// Gets the schema identifier.
        /// </summary>
        public string SchemaIdentifier { get; }

        /// <summary>
        /// Gets the type name.
        /// </summary>
        public string TypeName { get; }

        /// <summary>
        /// This method is used to determine if the Http Request is supported.
        /// </summary>
        /// <param name="request">Contains the Http request message.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Exception is thrown if there is not request message specified.</exception>
        public virtual bool Supports(HttpRequestMessage request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            return request.RequestUri?.AbsolutePath?.EndsWith(this.Path, StringComparison.OrdinalIgnoreCase) == true;
        }
    }
}