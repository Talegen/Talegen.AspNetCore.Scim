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
    using System.ComponentModel.Design;
    using System.Linq;
    using System.Threading;
    using Schema;

    /// <inheritdoc />
    public sealed class SchematizedJsonDeserializingFactory : SchematizedJsonDeserializingFactoryBase
    {
        /// <summary>
        /// Contains the patch serializer.
        /// </summary>
        private ISchematizedJsonDeserializingFactory<PatchRequest2> patchSerializer;

        /// <inheritdoc />
        public override IReadOnlyCollection<IExtension> Extensions { get; set; }

        /// <inheritdoc />
        public override IResourceJsonDeserializingFactory<GroupBase> GroupDeserializationBehavior { get; set; }

        /// <inheritdoc />
        public override ISchematizedJsonDeserializingFactory<PatchRequest2> PatchRequest2DeserializationBehavior
        {
            get => LazyInitializer.EnsureInitialized(ref this.patchSerializer, InitializePatchSerializer);
            set => this.patchSerializer = value;
        }

        /// <inheritdoc />
        public override IResourceJsonDeserializingFactory<Core2UserBase> UserDeserializationBehavior { get; set; }

        /// <summary>
        /// This method creates a group resource.
        /// </summary>
        /// <param name="schemaIdentifiers">Contains the schema identifiers.</param>
        /// <param name="json">Contains the serialized object.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Exception is thrown if the parameters are not specified.</exception>
        /// <exception cref="ArgumentException">Exception is thrown if the schema identifiers do not equate to one.</exception>
        private Resource CreateGroup(IReadOnlyCollection<string> schemaIdentifiers, IReadOnlyDictionary<string, object> json)
        {
            if (schemaIdentifiers == null)
            {
                throw new ArgumentNullException(nameof(schemaIdentifiers));
            }

            if (json == null)
            {
                throw new ArgumentNullException(nameof(json));
            }

            Resource group;

            if (this.GroupDeserializationBehavior != null)
            {
                group = this.GroupDeserializationBehavior.Create(json);
            }
            else if (schemaIdentifiers.Count != 1)
            {
                throw new ArgumentException(Schema.Properties.Resources.ExceptionInvalidResource);
            }
            else
            {
                group = new Core2GroupJsonDeserializingFactory().Create(json);
            }

            return group;
        }

        /// <summary>
        /// This method is used to create a new patch request.
        /// </summary>
        /// <param name="json">Contains the serialized request.</param>
        /// <returns>Returns a new <see cref="Schematized" /> object for the patch request.</returns>
        /// <exception cref="ArgumentNullException">Exception is thrown if the parameter is not specified.</exception>
        /// <exception cref="InvalidOperationException">Exception thrown if unable to parse.</exception>
        private Schematized CreatePatchRequest(IReadOnlyDictionary<string, object> json)
        {
            if (json == null)
            {
                throw new ArgumentNullException(nameof(json));
            }

            if (!this.TryCreatePatchRequest2Legacy(json, out Schematized result))
            {
                if (!TryCreatePatchRequest2Compliant(json, out result))
                {
                    throw new InvalidOperationException(Schema.Properties.Resources.ExceptionInvalidRequest);
                }
            }

            return result;
        }

        /// <summary>
        /// Creates a new user.
        /// </summary>
        /// <param name="schemaIdentifiers">Contains the schema identifiers.</param>
        /// <param name="json">Contains the serialized request.</param>
        /// <returns>Returns a new <see cref="Resource" /> object for the user.</returns>
        /// <exception cref="ArgumentNullException">Exception is thrown if the parameter is not specified.</exception>
        /// <exception cref="ArgumentException">Exception is thrown if the schema identifiers do not equate to one.</exception>
        private Resource CreateUser(IReadOnlyCollection<string> schemaIdentifiers, IReadOnlyDictionary<string, object> json)
        {
            if (schemaIdentifiers == null)
            {
                throw new ArgumentNullException(nameof(schemaIdentifiers));
            }

            if (json == null)
            {
                throw new ArgumentNullException(nameof(json));
            }

            Resource result;

            if (this.UserDeserializationBehavior != null)
            {
                result = this.UserDeserializationBehavior.Create(json);
            }
            else if (schemaIdentifiers.Any(item => item.Equals(SchemaIdentifiers.Core2EnterpriseUser, StringComparison.OrdinalIgnoreCase)))
            {
                result = new Core2EnterpriseUserJsonDeserializingFactory().Create(json);
            }
            else if (schemaIdentifiers.Count != 1)
            {
                throw new ArgumentException(Schema.Properties.Resources.ExceptionInvalidResource);
            }
            else
            {
                result = new Core2UserJsonDeserializingFactory().Create(json);
            }

            return result;
        }

        /// <summary>
        /// This method is used to create a schematized object.
        /// </summary>
        /// <param name="json">Contains the serialized request.</param>
        /// <returns>Returns a <see cref="Schematized" /> object.</returns>
        /// <exception cref="ArgumentException">Exception is thrown if the schema is not identified.</exception>
        /// <exception cref="NotSupportedException">Exception is thrown if the schema value is not supported.</exception>
        public override Schematized Create(IReadOnlyDictionary<string, object> json)
        {
            IReadOnlyDictionary<string, object> normalizedJson = this.Normalize(json);

            if (!normalizedJson.TryGetValue(AttributeNames.Schemas, out object value))
            {
                throw new ArgumentException(Schema.Properties.Resources.ExceptionUnidentifiableSchema);
            }

            IReadOnlyCollection<string> schemaIdentifiers;

            switch (value)
            {
                case IEnumerable schemas:
                    schemaIdentifiers = schemas.ToCollection<string>();
                    break;

                default:
                    throw new ArgumentException(Schema.Properties.Resources.ExceptionUnidentifiableSchema);
            }

            if (!this.TryCreateResourceFrom(normalizedJson, schemaIdentifiers, out Schematized result))
            {
                if (!this.TryCreateProtocolObjectFrom(normalizedJson, schemaIdentifiers, out result))
                {
                    if (!this.TryCreateExtensionObjectFrom(normalizedJson, schemaIdentifiers, out result))
                    {
                        string allSchemaIdentifiers = string.Join(Environment.NewLine, schemaIdentifiers);
                        throw new NotSupportedException(allSchemaIdentifiers);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// This method is used to initialize the patch serializer.
        /// </summary>
        /// <returns>Returns a new <see cref="PatchRequest2JsonDeserializingFactory" /> class.</returns>
        private static ISchematizedJsonDeserializingFactory<PatchRequest2> InitializePatchSerializer()
        {
            return new PatchRequest2JsonDeserializingFactory();
        }

        /// <summary>
        /// This method is used to try and create an extension object from JSON.
        /// </summary>
        /// <param name="json">Contains the serialized JSON.</param>
        /// <param name="schemaIdentifiers">Contains schema identifiers.</param>
        /// <param name="schematized">Contains the schematized output.</param>
        /// <returns>Returns a value indicating success.</returns>
        /// <exception cref="ArgumentNullException">Exception is thrown if JSON or identifiers are not specified.</exception>
        private bool TryCreateExtensionObjectFrom(IReadOnlyDictionary<string, object> json, IReadOnlyCollection<string> schemaIdentifiers, out Schematized schematized)
        {
            bool result = false;
            schematized = null;

            if (json == null)
            {
                throw new ArgumentNullException(nameof(json));
            }

            if (schemaIdentifiers == null)
            {
                throw new ArgumentNullException(nameof(schemaIdentifiers));
            }

            if (this.Extensions.TryMatch(schemaIdentifiers, out IExtension matchingExtension))
            {
                schematized = matchingExtension.JsonDeserializingFactory(json);
                result = true;
            }

            return result;
        }

        /// <summary>
        /// This method is used to try and create an patch request object from JSON.
        /// </summary>
        /// <param name="json">Contains the serialized JSON.</param>
        /// <param name="schematized">Contains the schematized output.</param>
        /// <returns>Returns a value indicating success.</returns>
        private static bool TryCreatePatchRequest2Compliant(IReadOnlyDictionary<string, object> json, out Schematized schematized)
        {
            schematized = null;
            ISchematizedJsonDeserializingFactory<PatchRequest2> deserializer = new PatchRequest2JsonDeserializingFactory();
            schematized = deserializer.Create(json);

            return true;
        }

        /// <summary>
        /// This method is used to try and create a legacy patch request object from JSON.
        /// </summary>
        /// <param name="json">Contains the serialized JSON.</param>
        /// <param name="schematized">Contains the schematized output.</param>
        /// <returns>Returns a value indicating success.</returns>
        private bool TryCreatePatchRequest2Legacy(IReadOnlyDictionary<string, object> json, out Schematized schematized)
        {
            schematized = null;

            ISchematizedJsonDeserializingFactory<PatchRequest2> deserializer = this.PatchRequest2DeserializationBehavior ?? new PatchRequest2JsonDeserializingFactory();
            schematized = deserializer.Create(json);

            return true;
        }

        /// <summary>
        /// This method is used to try and create an protocol object from JSON.
        /// </summary>
        /// <param name="json">Contains the serialized JSON.</param>
        /// <param name="schemaIdentifiers"></param>
        /// <param name="schematized">Contains the schematized output.</param>
        /// <returns>Returns a value indicating success.</returns>
        /// <exception cref="ArgumentNullException">Exception is thrown if the arguments are not specified.</exception>
        private bool TryCreateProtocolObjectFrom(IReadOnlyDictionary<string, object> json, IReadOnlyCollection<string> schemaIdentifiers, out Schematized schematized)
        {
            schematized = null;
            bool result = false;

            if (json == null)
            {
                throw new ArgumentNullException(nameof(json));
            }

            if (schemaIdentifiers == null)
            {
                throw new ArgumentNullException(nameof(schemaIdentifiers));
            }

            if (schemaIdentifiers.Count == 1)
            {
                if (schemaIdentifiers.Any(item => item.Equals(ProtocolSchemaIdentifiers.Version2PatchOperation, StringComparison.OrdinalIgnoreCase)))
                {
                    schematized = this.CreatePatchRequest(json);
                    result = true;
                }
                else if (schemaIdentifiers.Any(item => item.Equals(ProtocolSchemaIdentifiers.Version2Error, StringComparison.OrdinalIgnoreCase)))
                {
                    schematized = new ErrorResponseJsonDeserializingFactory().Create(json);
                    result = true;
                }
            }

            return result;
        }

        /// <summary>
        /// This method is used to try and create an resource object from JSON.
        /// </summary>
        /// <param name="json">Contains the serialized JSON.</param>
        /// <param name="schemaIdentifiers"></param>
        /// <param name="schematized">Contains the schematized output.</param>
        /// <returns>Returns a value indicating success.</returns>
        /// <exception cref="ArgumentNullException">Exception is thrown if the arguments are not specified.</exception>
        private bool TryCreateResourceFrom(IReadOnlyDictionary<string, object> json, IReadOnlyCollection<string> schemaIdentifiers, out Schematized schematized)
        {
            schematized = null;
            bool result = false;

            if (json == null)
            {
                throw new ArgumentNullException(nameof(json));
            }

            if (schemaIdentifiers == null)
            {
                throw new ArgumentNullException(nameof(schemaIdentifiers));
            }

            if (schemaIdentifiers.Any(item => item.Equals(SchemaIdentifiers.Core2User, StringComparison.OrdinalIgnoreCase)))
            {
                schematized = this.CreateUser(schemaIdentifiers, json);
                result = true;
            }
            else if (schemaIdentifiers.Any(item => item.Equals(SchemaIdentifiers.Core2Group, StringComparison.OrdinalIgnoreCase)))
            {
                schematized = this.CreateGroup(schemaIdentifiers, json);
                result = true;
            }

            return result;
        }
    }
}