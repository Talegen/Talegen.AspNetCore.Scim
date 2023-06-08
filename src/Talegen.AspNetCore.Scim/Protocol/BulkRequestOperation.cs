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
    using System.Net.Http;
    using System.Runtime.Serialization;
    using Schema;

    /// <summary>
    /// This class represents a bulk request operation.
    /// </summary>
    [DataContract]
    public sealed class BulkRequestOperation : BulkOperation
    {
        /// <summary>
        /// Contains the URI path.
        /// </summary>
        private Uri path;

        /// <summary>
        /// Contains the path value used for data contract serialization.
        /// </summary>
        [DataMember(Name = ProtocolAttributeNames.Path, Order = 0)]
        private string pathValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="BulkRequestOperation" /> class.
        /// </summary>
        private BulkRequestOperation()
        {
        }

        /// <summary>
        /// Gets or sets the data property.
        /// </summary>
        [DataMember(Name = ProtocolAttributeNames.Data, Order = 4)]
        public object Data { get; set; }

        /// <summary>
        /// Gets or sets the URI path.
        /// </summary>
        public Uri Path
        {
            get => this.path;

            set
            {
                this.path = value;
                this.pathValue = new SystemForCrossDomainIdentityManagementResourceIdentifier(value).RelativePath;
            }
        }

        /// <summary>
        /// This method is used for creating a delete operation.
        /// </summary>
        /// <param name="resource">Contains the resource URI to delete.</param>
        /// <returns>Returns a new <see cref="BulkRequestOperation" /> object.</returns>
        /// <exception cref="ArgumentNullException">Exception is thrown if the resource is not specified.</exception>
        public static BulkRequestOperation CreateDeleteOperation(Uri resource) =>
            new BulkRequestOperation
            {
                Method = HttpMethod.Delete,
                Path = resource ?? throw new ArgumentNullException(nameof(resource))
            };

        /// <summary>
        /// This method is used for creating a delete operation.
        /// </summary>
        /// <param name="resource">Contains the resource URI to operate on.</param>
        /// <param name="data">Contains the patch request data.</param>
        /// <returns>Returns a new <see cref="BulkRequestOperation" /> object.</returns>
        /// <exception cref="ArgumentNullException">Exception is thrown if the resource is not specified.</exception>
        public static BulkRequestOperation CreatePatchOperation(Uri resource, PatchRequest2 data)
        {
            if (resource == null)
            {
                throw new ArgumentNullException(nameof(resource));
            }

            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            PatchRequest2 patchRequest = new PatchRequest2(data.Operations);
            BulkRequestOperation result = new BulkRequestOperation
            {
                Method = ProtocolExtensions.PatchMethod,
                Path = resource,
                Data = patchRequest
            };

            return result;
        }

        /// <summary>
        /// This method is used to create a Post operation.
        /// </summary>
        /// <param name="data">Contains the patch request data.</param>
        /// <returns>Returns a new <see cref="BulkRequestOperation" /> object.</returns>
        /// <exception cref="ArgumentNullException">Exception is thrown if the resource is not specified.</exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        public static BulkRequestOperation CreatePostOperation(Resource data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            if (data.Schemas == null)
            {
                throw new ArgumentException(Schema.Properties.Resources.ExceptionUnidentifiableSchema);
            }

            if (!data.Schemas.Any())
            {
                throw new ArgumentException(Schema.Properties.Resources.ExceptionUnidentifiableSchema);
            }

            IList<Uri> paths = new List<Uri>(1);
            IEnumerable<ISchemaIdentifier> schemaIdentifiers = data.Schemas.Select(item => new SchemaIdentifier(item));

            foreach (ISchemaIdentifier schemaIdentifier in schemaIdentifiers)
            {
                Uri schemaIdentifierPath;

                if (schemaIdentifier.TryFindPath(out string pathValue))
                {
                    schemaIdentifierPath = new Uri(pathValue, UriKind.Relative);

                    if (paths.All(item => 0 != Uri.Compare(item, schemaIdentifierPath, UriComponents.AbsoluteUri, UriFormat.UriEscaped, StringComparison.OrdinalIgnoreCase)))
                    {
                        paths.Add(schemaIdentifierPath);
                    }
                }

                if (data.TryGetPathIdentifier(out Uri resourcePath))
                {
                    if (paths.All(item => 0 != Uri.Compare(item, resourcePath, UriComponents.AbsoluteUri, UriFormat.UriEscaped, StringComparison.OrdinalIgnoreCase)))
                    {
                        paths.Add(resourcePath);
                    }
                }
            }

            if (paths.Count != 1)
            {
                string schemas = string.Join(Environment.NewLine, data.Schemas);
                throw new NotSupportedException(schemas);
            }

            BulkRequestOperation result = new BulkRequestOperation
            {
                path = paths.Single(),
                Method = HttpMethod.Post,
                Data = data
            };

            return result;
        }

        /// <summary>
        /// This method is used to initialize a path.
        /// </summary>
        /// <param name="value">Contains the initial path value.</param>
        private void InitializePath(string value)
        {
            this.path = !string.IsNullOrWhiteSpace(value) ? new Uri(value, UriKind.Relative) : null;
        }

        /// <summary>
        /// This method is used to initialize a path.
        /// </summary>
        private void InitializePath() => this.InitializePath(this.pathValue);

        /// <summary>
        /// This method is called when the object is deserialized
        /// </summary>
        /// <param name="_">Contains the streaming context.</param>
        [OnDeserialized]
        private void OnDeserialized(StreamingContext _) => this.InitializePath();
    }
}