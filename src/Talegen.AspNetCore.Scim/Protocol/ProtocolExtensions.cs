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
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using System.Web;
    using Newtonsoft.Json;
    using Schema;
    using Service;

    /// <summary>
    /// This class contains SCIM protocol helper extensions.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "None")]
    public static class ProtocolExtensions
    {
        /// <summary>
        /// Contains a bulk identifier regex pattern.
        /// </summary>
        private const string BulkIdentifierPattern = @"^((\s*)bulkId(\s*):(\s*)(?<" +
                                                     ProtocolExtensions.ExpressionGroupNameBulkIdentifier +
                                                     @">[^\s]*))";

        /// <summary>
        /// Contains the expression group bulk identifier.
        /// </summary>
        private const string ExpressionGroupNameBulkIdentifier = "identifier";

        /// <summary>
        /// Contains the DELETE method name.
        /// </summary>
        public const string MethodNameDelete = "DELETE";

        /// <summary>
        /// Contains the PATCH method name.
        /// </summary>
        public const string MethodNamePatch = "PATCH";

        /// <summary>
        /// Contains a Patch Http Method object.
        /// </summary>
        private static readonly Lazy<HttpMethod> MethodPatch = new Lazy<HttpMethod>(() => new HttpMethod(MethodNamePatch));

        /// <summary>
        /// Contains a RegEx expression for the bulk identifier.
        /// </summary>
        private static readonly Lazy<Regex> BulkIdentifierExpression = new Lazy<Regex>(() => new Regex(BulkIdentifierPattern, RegexOptions.IgnoreCase | RegexOptions.Compiled));

        /// <summary>
        /// This interface defines the minimum implementation of a request message writer.
        /// </summary>
        private interface IHttpRequestMessageWriter : IDisposable
        {
            /// <summary>
            /// Close the writer.
            /// </summary>
            void Close();

            /// <summary>
            /// Flush the stream.
            /// </summary>
            /// <returns>Returns a task object.</returns>
            Task FlushAsync();

            /// <summary>
            /// Write to the stream.
            /// </summary>
            /// <returns>Returns a task object.</returns>
            Task WriteAsync();
        }

        /// <summary>
        /// Gets a patch method object.
        /// </summary>
        public static HttpMethod PatchMethod => ProtocolExtensions.MethodPatch.Value;

        /// <summary>
        /// This method is used to apply a patch request to a specified core group.
        /// </summary>
        /// <param name="group">Contains the group to apply the patch.</param>
        /// <param name="patch">Contains the patch to apply.</param>
        /// <exception cref="ArgumentNullException">Exception is thrown if the group is not specified.</exception>
        public static void Apply(this Core2Group group, PatchRequest2 patch)
        {
            if (group == null)
            {
                throw new ArgumentNullException(nameof(group));
            }

            if (patch is { Operations: not null } && patch.Operations.Any())
            {
                foreach (PatchOperation2Combined operation in patch.Operations)
                {
                    PatchOperation2 operationInternal = new PatchOperation2
                    {
                        OperationName = operation.OperationName,
                        Path = operation.Path
                    };

                    OperationValue[] values = null;

                    if (operation?.Value != null)
                    {
                        values = JsonConvert.DeserializeObject<OperationValue[]>(operation.Value, ProtocolConstants.JsonSettings.Value);
                    }

                    if (values == null)
                    {
                        string value = null;

                        if (operation?.Value != null)
                        {
                            value = JsonConvert.DeserializeObject<string>(operation.Value, ProtocolConstants.JsonSettings.Value);
                        }

                        OperationValue valueSingle = new OperationValue
                        {
                            Value = value
                        };

                        operationInternal.AddValue(valueSingle);
                    }
                    else
                    {
                        foreach (OperationValue value in values)
                        {
                            operationInternal.AddValue(value);
                        }
                    }

                    group.Apply(operationInternal);
                }
            }
        }

        /// <summary>
        /// This method is used to apply an operation request to a specified core group.
        /// </summary>
        /// <param name="group">Contains the group to apply the patch.</param>
        /// <param name="patch">Contains the operation to apply.</param>
        private static void Apply(this Core2Group group, PatchOperation2 operation)
        {
            if (operation?.Path != null && !string.IsNullOrWhiteSpace(operation.Path.AttributePath))
            {
                switch (operation.Path.AttributePath)
                {
                    case AttributeNames.DisplayName:
                        OperationValue value = operation.Value.SingleOrDefault();

                        if (operation.Name == OperationName.Remove)
                        {
                            if (null == value || string.Equals(group.DisplayName, value.Value, StringComparison.OrdinalIgnoreCase))
                            {
                                value = null;
                            }
                            else
                            {
                                break;
                            }
                        }

                        group.DisplayName = value?.Value ?? null;
                        break;

                    case AttributeNames.Members:
                        switch (operation?.Name)
                        {
                            case OperationName.Add:
                                IEnumerable<Member> membersToAdd = operation.Value
                                    .Select(item => new Member
                                    {
                                        Value = item.Value
                                    }).ToArray();

                                ////IList<Member> buffer = new List<Member>();

                                ////foreach (Member member in membersToAdd)
                                ////{
                                ////    // O(n) with the number of group members, so for large groups this is not optimal
                                ////    if (!group.Members.Any((Member item) =>
                                ////            string.Equals(item.Value, member.Value, StringComparison.OrdinalIgnoreCase)))
                                ////    {
                                ////        buffer.Add(member);
                                ////    }
                                ////}
                                IList<Member> buffer = membersToAdd
                                    .Where(member => !group.Members.Any(item => string.Equals(item.Value, member.Value, StringComparison.OrdinalIgnoreCase)))
                                    .ToList();

                                group.Members = group.Members.Concat(buffer.ToArray());
                                break;

                            case OperationName.Remove:

                                if (operation?.Value?.FirstOrDefault()?.Value == null)
                                {
                                    group.Members = Enumerable.Empty<Member>();
                                }
                                else
                                {
                                    IDictionary<string, Member> members =
                                        new Dictionary<string, Member>(group.Members.Count());

                                    foreach (Member item in group.Members)
                                    {
                                        members.Add(item.Value, item);
                                    }

                                    foreach (OperationValue operationValue in operation.Value)
                                    {
                                        if (members.TryGetValue(operationValue.Value, out Member _))
                                        {
                                            members.Remove(operationValue.Value);
                                        }
                                    }

                                    group.Members = members.Values;
                                }

                                break;
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// This method is used to compose a delete request for the specified resource of the resource identifier.
        /// </summary>
        /// <param name="resource">Contains the resource.</param>
        /// <param name="baseResourceIdentifier">Contains the identifier of the resource to compose a delete request on.</param>
        /// <returns>Returns a new <see cref="HttpRequestMessage" /> object.</returns>
        /// <exception cref="ArgumentNullException">Exception is thrown if the resource identifier is not specified.</exception>
        public static HttpRequestMessage ComposeDeleteRequest(this Resource resource, Uri baseResourceIdentifier)
        {
            if (baseResourceIdentifier == null)
            {
                throw new ArgumentNullException(nameof(baseResourceIdentifier));
            }

            Uri resourceIdentifier = resource.GetResourceIdentifier(baseResourceIdentifier);
            HttpRequestMessage result = null;

            try
            {
                result = new HttpRequestMessage(HttpMethod.Delete, resourceIdentifier);
            }
            catch
            {
                if (result != null)
                {
                    result.Dispose();
#pragma warning disable IDE0059 // Unnecessary assignment of a value
                    result = null;
#pragma warning restore IDE0059 // Unnecessary assignment of a value
                }

                throw;
            }

            return result;
        }

        /// <summary>
        /// This method is used to compose a paginated Get request for the specified resource identifier.
        /// </summary>
        /// <param name="schematized">Contains a schematized object.</param>
        /// <param name="baseResourceIdentifier">Contains the identifier to get.</param>
        /// <param name="filters">Contains any filters related to the query.</param>
        /// <param name="requestedAttributePaths">Contains any requested attribute paths.</param>
        /// <param name="excludedAttributePaths">Contains any excluded attribute paths.</param>
        /// <param name="paginationParameters">Contains pagination parameters for the query.</param>
        /// <returns>Returns a new <see cref="HttpRequestMessage" /> object.</returns>
        /// <exception cref="ArgumentNullException">Exception is thrown if the resource identifier, filter, or attribute paths are not specified.</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "False analysis of 'this' parameter of an extension method")]
        public static HttpRequestMessage ComposeGetRequest(
            this Schematized schematized,
            Uri baseResourceIdentifier,
            IReadOnlyCollection<IFilter> filters,
            IReadOnlyCollection<string> requestedAttributePaths,
            IReadOnlyCollection<string> excludedAttributePaths,
            IPaginationParameters paginationParameters)
        {
            if (baseResourceIdentifier == null)
            {
                throw new ArgumentNullException(nameof(baseResourceIdentifier));
            }

            if (filters == null)
            {
                throw new ArgumentNullException(nameof(filters));
            }

            if (requestedAttributePaths == null)
            {
                throw new ArgumentNullException(nameof(requestedAttributePaths));
            }

            if (excludedAttributePaths == null)
            {
                throw new ArgumentNullException(nameof(excludedAttributePaths));
            }

            Uri resourceIdentifier = schematized.ComposeResourceIdentifier(baseResourceIdentifier, filters, requestedAttributePaths, excludedAttributePaths, paginationParameters);
            HttpRequestMessage result = null;

            try
            {
                result = new HttpRequestMessage(HttpMethod.Get, resourceIdentifier);
            }
            catch
            {
                if (result != null)
                {
                    result.Dispose();
#pragma warning disable IDE0059 // Unnecessary assignment of a value
                    result = null;
#pragma warning restore IDE0059 // Unnecessary assignment of a value
                }

                throw;
            }

            return result;
        }

        /// <summary>
        /// This method is used to compose a Get request for the specified resource identifier.
        /// </summary>
        /// <param name="schematized">Contains a schematized object.</param>
        /// <param name="baseResourceIdentifier">Contains the identifier to get.</param>
        /// <param name="filters">Contains any filters related to the query.</param>
        /// <param name="requestedAttributePaths">Contains any requested attribute paths.</param>
        /// <param name="excludedAttributePaths">Contains any excluded attribute paths.</param>
        /// <returns>Returns a new <see cref="HttpRequestMessage" /> object.</returns>
        /// <exception cref="ArgumentNullException">Exception is thrown if the resource identifier, filter, or attribute paths are not specified.</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "False analysis of 'this' parameter of an extension method")]
        public static HttpRequestMessage ComposeGetRequest(
            this Schematized schematized,
            Uri baseResourceIdentifier,
            IReadOnlyCollection<IFilter> filters,
            IReadOnlyCollection<string> requestedAttributePaths,
            IReadOnlyCollection<string> excludedAttributePaths)
        {
            HttpRequestMessage result = null;

            try
            {
                result = schematized.ComposeGetRequest(baseResourceIdentifier, filters, requestedAttributePaths, excludedAttributePaths, null);
            }
            catch
            {
                if (result != null)
                {
                    result.Dispose();
#pragma warning disable IDE0059 // Unnecessary assignment of a value
                    result = null;
#pragma warning restore IDE0059 // Unnecessary assignment of a value
                }

                throw;
            }

            return result;
        }

        /// <summary>
        /// This method is used to compose a Get request for the specified resource identifier.
        /// </summary>
        /// <param name="resource">Contains a resource object.</param>
        /// <param name="baseResourceIdentifier">Contains the identifier to get.</param>
        /// <param name="requestedAttributePaths">Contains any requested attribute paths.</param>
        /// <param name="excludedAttributePaths">Contains any excluded attribute paths.</param>
        /// <returns>Returns a new <see cref="HttpRequestMessage" /> object.</returns>
        /// <exception cref="ArgumentNullException">Exception is thrown if the resource identifier or attribute paths are not specified.</exception>

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "False analysis of 'this' parameter of an extension method")]
        public static HttpRequestMessage ComposeGetRequest(
            this Resource resource,
            Uri baseResourceIdentifier,
            IReadOnlyCollection<string> requestedAttributePaths,
            IReadOnlyCollection<string> excludedAttributePaths)
        {
            if (baseResourceIdentifier == null)
            {
                throw new ArgumentNullException(nameof(baseResourceIdentifier));
            }

            if (requestedAttributePaths == null)
            {
                throw new ArgumentNullException(nameof(requestedAttributePaths));
            }

            if (excludedAttributePaths == null)
            {
                throw new ArgumentNullException(nameof(excludedAttributePaths));
            }

            Uri resourceIdentifier = resource.ComposeResourceIdentifier(baseResourceIdentifier, requestedAttributePaths, excludedAttributePaths);
            HttpRequestMessage result = null;

            try
            {
                result = new HttpRequestMessage(HttpMethod.Get, resourceIdentifier);
            }
            catch
            {
                if (result != null)
                {
                    result.Dispose();
#pragma warning disable IDE0059 // Unnecessary assignment of a value
                    result = null;
#pragma warning restore IDE0059 // Unnecessary assignment of a value
                }

                throw;
            }

            return result;
        }

        /// <summary>
        /// This method is used to compose a Get request for the specified resource identifier.
        /// </summary>
        /// <param name="resource">Contains a resource object.</param>
        /// <param name="baseResourceIdentifier">Contains the identifier to get.</param>
        /// <returns>Returns a new <see cref="HttpRequestMessage" /> object.</returns>
        /// <exception cref="ArgumentNullException">Exception is thrown if the resource identifier is not specified.</exception>
        public static HttpRequestMessage ComposeGetRequest(this Resource resource, Uri baseResourceIdentifier)
        {
            if (baseResourceIdentifier == null)
            {
                throw new ArgumentNullException(nameof(baseResourceIdentifier));
            }

            HttpRequestMessage result = null;

            try
            {
                IReadOnlyCollection<string> requestedAttributePaths = Array.Empty<string>();
                IReadOnlyCollection<string> excludedAttributePaths = Array.Empty<string>();
                result = resource.ComposeGetRequest(baseResourceIdentifier, requestedAttributePaths, excludedAttributePaths);
            }
            catch
            {
                if (result != null)
                {
                    result.Dispose();
#pragma warning disable IDE0059 // Unnecessary assignment of a value
                    result = null;
#pragma warning restore IDE0059 // Unnecessary assignment of a value
                }

                throw;
            }

            return result;
        }

        /// <summary>
        /// This method is used to compose a resource identifier URI.
        /// </summary>
        /// <param name="resource">Contains the resource object.</param>
        /// <param name="baseResourceIdentifier">Contains the resource identifier to compose.</param>
        /// <returns>Returns a <see cref="UriBuilder" /> object.</returns>
        /// <exception cref="ArgumentNullException">Exception is thrown if the resource identifier is not specified.</exception>
        /// <exception cref="InvalidOperationException">Exception is thrown if the identifier is invalid.</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "False analysis of 'this' parameter of an extension method")]
        public static UriBuilder ComposeResourceIdentifier(this Resource resource, Uri baseResourceIdentifier)
        {
            if (baseResourceIdentifier == null)
            {
                throw new ArgumentNullException(nameof(baseResourceIdentifier));
            }

            if (string.IsNullOrWhiteSpace(resource.Identifier))
            {
                throw new InvalidOperationException(Schema.Properties.Resources.ExceptionInvalidResource);
            }

            Uri foundation = resource.GetResourceIdentifier(baseResourceIdentifier);
            UriBuilder result = new UriBuilder(foundation);
            return result;
        }

        /// <summary>
        /// This method is used to compose a resource identifier URI.
        /// </summary>
        /// <param name="resource">Contains the resource object.</param>
        /// <param name="baseResourceIdentifier">Contains the resource identifier to compose.</param>
        /// <param name="requestedAttributePaths">Contains requested attribute paths.</param>
        /// <param name="excludedAttributePaths">Contains excluded attribute paths.</param>
        /// <returns>Returns a <see cref="Uri" /> object.</returns>
        /// <exception cref="ArgumentNullException">Exception is thrown if the resource identifier is not specified.</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "False analysis of 'this' parameter of an extension method")]
        public static Uri ComposeResourceIdentifier(
            this Resource resource,
            Uri baseResourceIdentifier,
            IReadOnlyCollection<string> requestedAttributePaths,
            IReadOnlyCollection<string> excludedAttributePaths)
        {
            if (baseResourceIdentifier == null)
            {
                throw new ArgumentNullException(nameof(baseResourceIdentifier));
            }

            if (requestedAttributePaths == null)
            {
                throw new ArgumentNullException(nameof(requestedAttributePaths));
            }

            if (excludedAttributePaths == null)
            {
                throw new ArgumentNullException(nameof(excludedAttributePaths));
            }

            if (!resource.TryGetSchemaIdentifier(out string schemaIdentifier))
            {
                schemaIdentifier = resource.GetSchemaIdentifier();
            }

            if (!resource.TryGetPath(out string path))
            {
                path = resource.GetPath();
            }

            IResourceRetrievalParameters retrievalParameters = new ResourceRetrievalParameters(schemaIdentifier, path, resource.Identifier, requestedAttributePaths, excludedAttributePaths);
            string query = retrievalParameters.ToString();
            UriBuilder resourceIdentifier = resource.ComposeResourceIdentifier(baseResourceIdentifier);
            resourceIdentifier.Query = query;
            Uri result = resourceIdentifier.Uri;
            return result;
        }

        /// <summary>
        /// This method is used to compose a resource identifier URI.
        /// </summary>
        /// <param name="schematized">Contains a schematized object.</param>
        /// <param name="baseResourceIdentifier">Contains the resource identifier to compose.</param>
        /// <param name="parameters">Contains query parameters.</param>
        /// <returns>Returns a <see cref="Uri" /> object.</returns>
        /// <exception cref="ArgumentNullException">Exception is thrown if the resource identifier is not specified.</exception>
        public static Uri ComposeResourceIdentifier(this Schematized schematized, Uri baseResourceIdentifier, IQueryParameters parameters)
        {
            if (baseResourceIdentifier == null)
            {
                throw new ArgumentNullException(nameof(baseResourceIdentifier));
            }

            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            Uri typeIdentifier = schematized.GetTypeIdentifier(baseResourceIdentifier);
            UriBuilder resourceIdentifier = new UriBuilder(typeIdentifier)
            {
                Query = parameters.ToString()
            };
            Uri result = resourceIdentifier.Uri;
            return result;
        }

        /// <summary>
        /// This method is used to compose paginated resource identifier URI.
        /// </summary>
        /// <param name="schematized">Contains a schematized object.</param>
        /// <param name="baseResourceIdentifier">Contains the resource identifier to compose.</param>
        /// <param name="filters">Contains a collection of query filters.</param>
        /// <param name="requestedAttributePaths">Contains requested attribute paths.</param>
        /// <param name="excludedAttributePaths">Contains excluded attribute paths.</param>
        /// <param name="paginationParameters">Contains pagination parameters for the query.</param>
        /// <returns>Returns a <see cref="Uri" /> object.</returns>
        /// <exception cref="ArgumentNullException">Exception is thrown if the resource identifier is not specified.</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "False analysis of an extension method")]
        public static Uri ComposeResourceIdentifier(
            this Schematized schematized,
            Uri baseResourceIdentifier,
            IReadOnlyCollection<IFilter> filters,
            IReadOnlyCollection<string> requestedAttributePaths,
            IReadOnlyCollection<string> excludedAttributePaths,
            IPaginationParameters paginationParameters)
        {
            if (baseResourceIdentifier == null)
            {
                throw new ArgumentNullException(nameof(baseResourceIdentifier));
            }

            if (filters == null)
            {
                throw new ArgumentNullException(nameof(filters));
            }

            if (requestedAttributePaths == null)
            {
                throw new ArgumentNullException(nameof(requestedAttributePaths));
            }

            if (excludedAttributePaths == null)
            {
                throw new ArgumentNullException(nameof(excludedAttributePaths));
            }

            if (!schematized.TryGetSchemaIdentifier(out string schemaIdentifier))
            {
                schemaIdentifier = schematized.GetSchemaIdentifier();
            }

            if (!schematized.TryGetPath(out string path))
            {
                path = schematized.GetPath();
            }

            IQueryParameters queryParameters =
                new QueryParameters(schemaIdentifier, path, filters, requestedAttributePaths, excludedAttributePaths);
            queryParameters.PaginationParameters = paginationParameters;
            Uri result = schematized.ComposeResourceIdentifier(baseResourceIdentifier, queryParameters);
            return result;
        }

        /// <summary>
        /// This method is used to compose a resource identifier URI.
        /// </summary>
        /// <param name="schematized">Contains a schematized object.</param>
        /// <param name="baseResourceIdentifier">Contains the resource identifier to compose.</param>
        /// <param name="filters">Contains a collection of query filters.</param>
        /// <param name="requestedAttributePaths">Contains requested attribute paths.</param>
        /// <param name="excludedAttributePaths">Contains excluded attribute paths.</param>
        /// <returns>Returns a <see cref="Uri" /> object.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "False analysis of an extension method")]
        public static Uri ComposeResourceIdentifier(
            this Schematized schematized,
            Uri baseResourceIdentifier,
            IReadOnlyCollection<IFilter> filters,
            IReadOnlyCollection<string> requestedAttributePaths,
            IReadOnlyCollection<string> excludedAttributePaths)
        {
            Uri result =
                schematized.ComposeResourceIdentifier(
                    baseResourceIdentifier,
                    filters,
                    requestedAttributePaths,
                    excludedAttributePaths,
                    null);
            return result;
        }

        /// <summary>
        /// This method is used to compose a type identifier URI.
        /// </summary>
        /// <param name="baseResourceIdentifier">Contains the base identifier.</param>
        /// <param name="path">Contains the path.</param>
        /// <returns>Returns a <see cref="Uri" /> object.</returns>
        /// <exception cref="ArgumentNullException">Exception is thrown if the base identifier or path are not specified.</exception>
        private static Uri ComposeTypeIdentifier(Uri baseResourceIdentifier, string path)
        {
            if (baseResourceIdentifier == null)
            {
                throw new ArgumentNullException(nameof(baseResourceIdentifier));
            }

            if (path == null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            string baseResourceIdentifierValue = baseResourceIdentifier.ToString();
            string resultValue =
                baseResourceIdentifierValue +
                SchemaConstants.PathInterface +
                ServiceConstants.SeparatorSegments +
                path;

            Uri result = new Uri(resultValue);
            return result;
        }

        /// <summary>
        /// This method is used to get an identifier for a resource.
        /// </summary>
        /// <param name="resource">Contains the resource.</param>
        /// <returns>Returns the resource identifier.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "False analysis of 'this' parameter of an extension method")]
        public static IResourceIdentifier GetIdentifier(this Resource resource)
        {
            if (!resource.TryGetSchemaIdentifier(out string schemaIdentifier))
            {
                schemaIdentifier = resource.GetSchemaIdentifier();
            }

            IResourceIdentifier result = new ResourceIdentifier(schemaIdentifier, resource.Identifier);
            return result;
        }

        /// <summary>
        /// This method is used to get a path for for the schematized object.
        /// </summary>
        /// <param name="schematized">Contains the schematized object.</param>
        /// <returns>Returns the path.</returns>
        /// <exception cref="NotSupportedException">Exception is thrown if the path is not supported.</exception>
        private static string GetPath(this Schematized schematized)
        {
            string result;

            if (schematized.TryGetPath(out string path))
            {
                result = path;
            }
            else if (schematized.Is(SchemaIdentifiers.Core2EnterpriseUser))
            {
                result = ProtocolConstants.PathUsers;
            }
            else if (schematized.Is(SchemaIdentifiers.Core2User))
            {
                result = ProtocolConstants.PathUsers;
            }
            else if (schematized.Is(SchemaIdentifiers.Core2Group))
            {
                result = ProtocolConstants.PathGroups;
            }
            else
            {
                switch (schematized)
                {
                    case UserBase _:
                        result = ProtocolConstants.PathUsers;
                        break;

                    case GroupBase _:
                        result = ProtocolConstants.PathGroups;
                        break;

                    default:
                        string unsupportedTypeName = schematized.GetType().FullName;
                        throw new NotSupportedException(unsupportedTypeName);
                }
            }

            return result;
        }

        /// <summary>
        /// This method is used to get a resource identifier from the resource object.
        /// </summary>
        /// <param name="resource">Contains the resource object.</param>
        /// <param name="baseResourceIdentifier">Contains the resource identifier.</param>
        /// <returns>Returns a <see cref="Uri" /> object.</returns>
        /// <exception cref="ArgumentNullException">Exception is thrown if the base identifier or path are not specified.</exception>
        /// <exception cref="InvalidOperationException">Exception is thrown if the identifier is invalid.</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "False analysis of 'this' parameter of an extension method")]
        public static Uri GetResourceIdentifier(this Resource resource, Uri baseResourceIdentifier)
        {
            if (baseResourceIdentifier == null)
            {
                throw new ArgumentNullException(nameof(baseResourceIdentifier));
            }

            if (string.IsNullOrWhiteSpace(resource.Identifier))
            {
                throw new InvalidOperationException(Schema.Properties.Resources.ExceptionInvalidResource);
            }

            if (!resource.TryGetIdentifier(baseResourceIdentifier, out Uri result))
            {
                Uri typeResource = resource.GetTypeIdentifier(baseResourceIdentifier);
                string escapedIdentifier = Uri.EscapeDataString(resource.Identifier);
                string resultValue = typeResource.ToString() + ServiceConstants.SeparatorSegments + escapedIdentifier;
                result = new Uri(resultValue);
            }

            return result;
        }

        /// <summary>
        /// This method is used to get a schema identifier.
        /// </summary>
        /// <param name="schemaIdentifiers">Contains a collection of schema identifiers.</param>
        /// <returns>Returns a schema identifier found.</returns>
        /// <exception cref="ArgumentNullException">Exception thrown if schema identifiers are not specified.</exception>
        /// <exception cref="ArgumentException">Exception thrown if no schema identifiers are specified.</exception>
        /// <exception cref="NotSupportedException">Exception thrown if the schema identifier is not supported.</exception>
        private static string GetSchemaIdentifier(IReadOnlyCollection<string> schemaIdentifiers)
        {
            string result = null;

            if (schemaIdentifiers == null)
            {
                throw new ArgumentNullException(nameof(schemaIdentifiers));
            }

            if (!schemaIdentifiers.Any())
            {
                throw new ArgumentException(Schema.Properties.Resources.ExceptionUnidentifiableSchema);
            }

            foreach (string schema in schemaIdentifiers)
            {
                switch (schema)
                {
                    case SchemaIdentifiers.Core2User:
                    case SchemaIdentifiers.Core2EnterpriseUser:
                        result = SchemaIdentifiers.Core2EnterpriseUser;
                        break;

                    case SchemaIdentifiers.Core2Group:
                        result = SchemaIdentifiers.Core2Group;
                        break;
                }
            }

            if (string.IsNullOrWhiteSpace(result))
            {
                throw new NotSupportedException(string.Join(Environment.NewLine, schemaIdentifiers));
            }

            return result;
        }

        /// <summary>
        /// This method is used to get a schema identifier.
        /// </summary>
        /// <param name="schematized">Contains a schematized object.</param>
        /// <returns>Returns the schema identifier.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "False analysis of 'this' parameter of an extension method")]
        public static string GetSchemaIdentifier(this Schematized schematized)
        {
            if (!schematized.TryGetSchemaIdentifier(out string result))
            {
                result = ProtocolExtensions.GetSchemaIdentifier(schematized.Schemas);
            }

            return result;
        }

        /// <summary>
        /// This method is used to get a type identifier.
        /// </summary>
        /// <param name="schematized">Contains a schematized object.</param>
        /// <param name="baseResourceIdentifier">Contains the resource identifier to get.</param>
        /// <returns>Returns a <see cref="Uri" /> object.</returns>
        /// <exception cref="ArgumentNullException">Exception is thrown if the base identifier or path are not specified.</exception>
        /// <exception cref="InvalidOperationException">Exception is thrown if the identifier is invalid.</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "False analysis of 'this' parameter of an extension method")]
        public static Uri GetTypeIdentifier(this Schematized schematized, Uri baseResourceIdentifier)
        {
            if (baseResourceIdentifier == null)
            {
                throw new ArgumentNullException(nameof(baseResourceIdentifier));
            }

            if (schematized.Schemas == null)
            {
                throw new InvalidOperationException(Schema.Properties.Resources.ExceptionInvalidResource);
            }

            string path = schematized.GetPath();
            Uri result = ProtocolExtensions.ComposeTypeIdentifier(baseResourceIdentifier, path);

            return result;
        }

        /// <summary>
        /// This method is used to match a schema identifier.
        /// </summary>
        /// <param name="extension">Contains the extension object.</param>
        /// <param name="schemaIdentifier">Contains the schema identifier to match.</param>
        /// <returns>Returns a value indicating whether the identifier is a match.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "False analysis of 'this' parameter of an extension method")]
        public static bool Matches(this IExtension extension, string schemaIdentifier)
        {
            bool result = string.Equals(schemaIdentifier, extension.SchemaIdentifier, StringComparison.OrdinalIgnoreCase);
            return result;
        }

        /// <summary>
        /// This method is used to patch e-mail address for a specified operation.
        /// </summary>
        /// <param name="electronicMailAddresses">Contains the email addresses to patch.</param>
        /// <param name="operation">Contains the operation to apply.</param>
        /// <returns>Returns the patched result of email addresses.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "None")]
        internal static IEnumerable<ElectronicMailAddress> PatchElectronicMailAddresses(
            IEnumerable<ElectronicMailAddress> electronicMailAddresses,
            PatchOperation2 operation)
        {
            IEnumerable<ElectronicMailAddress> result = electronicMailAddresses;

            var subAttribute = operation.Path.SubAttributes.SingleOrDefault();
            string electronicMailAddressType = subAttribute?.ComparisonValue ?? string.Empty;

            if (!string.Equals(AttributeNames.ElectronicMailAddresses, operation.Path.AttributePath, StringComparison.OrdinalIgnoreCase)
               || string.IsNullOrWhiteSpace(operation.Path.ValuePath.AttributePath) ||
               subAttribute == null ||
               ((operation.Value != null && operation.Value.Count != 1) || (null == operation.Value && operation.Name != OperationName.Remove)) ||
               !string.Equals(AttributeNames.Type, subAttribute.AttributePath, StringComparison.OrdinalIgnoreCase) ||
               (electronicMailAddressType != ElectronicMailAddressBase.Home && electronicMailAddressType != ElectronicMailAddressBase.Work))
            {
                result = electronicMailAddresses;
            }
            else
            {
                ElectronicMailAddress electronicMailAddress;
                ElectronicMailAddress electronicMailAddressExisting;

                if (electronicMailAddresses != null)
                {
                    electronicMailAddressExisting =
                        electronicMailAddress =
                            electronicMailAddresses.SingleOrDefault(item => subAttribute.ComparisonValue == item.ItemType);
                }
                else
                {
                    electronicMailAddressExisting = null;
                    electronicMailAddress = new ElectronicMailAddress
                    {
                        ItemType = electronicMailAddressType
                    };
                }

                string value = operation.Value?.Single().Value;

                if (value != null &&
                    OperationName.Remove == operation.Name &&
                    string.Equals(value, electronicMailAddress.Value, StringComparison.OrdinalIgnoreCase))
                {
                    value = null;
                }

                electronicMailAddress.Value = value;

                if (string.IsNullOrWhiteSpace(electronicMailAddress.Value))
                {
                    if (electronicMailAddressExisting != null)
                    {
                        result =
                            electronicMailAddresses
                                .Where(
                                    (ElectronicMailAddress item) =>
                                        !string.Equals(subAttribute.ComparisonValue, item.ItemType, StringComparison.Ordinal))
                                .ToArray();
                    }
                    else
                    {
                        result = electronicMailAddresses;
                    }
                }
                else if (electronicMailAddressExisting != null)
                {
                    result = electronicMailAddresses;
                }
                else
                {
                    result = new ElectronicMailAddress[]
                    {
                        electronicMailAddress
                    };

                    if (electronicMailAddresses != null)
                    {
                        result = electronicMailAddresses.Union(electronicMailAddresses).ToArray();
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// This method is used to patch roles for a specified operation.
        /// </summary>
        /// <param name="roles">Contains the roles to patch.</param>
        /// <param name="operation">Contains the operation to apply.</param>
        /// <returns>Returns the patched result of roles.</returns>
        internal static IEnumerable<Role> PatchRoles(IEnumerable<Role> roles, PatchOperation2 operation)
        {
            IEnumerable<Role> result = roles;

            if (operation != null)
            {
                IFilter subAttribute = operation.Path.SubAttributes.SingleOrDefault();

                if (!string.Equals(Talegen.AspNetCore.Scim.Schema.AttributeNames.Roles, operation.Path.AttributePath, StringComparison.OrdinalIgnoreCase) ||
                    operation.Path.ValuePath == null ||
                    string.IsNullOrWhiteSpace(operation.Path.ValuePath.AttributePath) ||
                    subAttribute == null ||
                    ((operation.Value != null && operation.Value.Count != 1) || (null == operation.Value && operation.Name != OperationName.Remove)))
                {
                    result = roles;
                }
                else
                {
                    Role role;
                    Role roleExisting;

                    if (roles != null)
                    {
                        roleExisting = role = roles.SingleOrDefault(item => subAttribute.ComparisonValue == item.ItemType);
                    }
                    else
                    {
                        roleExisting = null;

                        role = new Role
                        {
                            Primary = true
                        };
                    }

                    string value = operation.Value?.Single().Value;

                    if (value != null &&
                        OperationName.Remove == operation.Name &&
                        string.Equals(value, role.Value, StringComparison.OrdinalIgnoreCase))
                    {
                        value = null;
                    }

                    role.Value = value;

                    if (string.IsNullOrWhiteSpace(role.Value))
                    {
                        if (roleExisting != null)
                        {
                            result = roles.Where(item => subAttribute.ComparisonValue != item.ItemType).ToArray();
                        }
                        else
                        {
                            result = roles;
                        }
                    }
                    else if (roleExisting != null)
                    {
                        result = roles;
                    }
                    else
                    {
                        result = new Role[]
                        {
                            role
                        };

                        if (roles != null)
                        {
                            result = roles.Union(roles).ToArray();
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// This method is used to implement a serialization of a resource identifier to a URI.
        /// </summary>
        /// <param name="resourceIdentifier">Contains the resource identifier to serialize.</param>
        /// <param name="resource">Contains the resource.</param>
        /// <param name="baseResourceIdentifier">Contains the base resource identifier.</param>
        /// <returns>Returns a new <see cref="Uri" /> object.</returns>
        /// <exception cref="ArgumentNullException">Exception is thrown if the resource or base identifier are not specified.</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "resourceIdentifier", Justification = "False analysis of extension method")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "False analysis of 'this' parameter of an extension method")]
        public static Uri Serialize(this IResourceIdentifier resourceIdentifier, Resource resource, Uri baseResourceIdentifier)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            if (resource == null)
            {
                throw new ArgumentNullException(nameof(resource));
            }

            if (baseResourceIdentifier == null)
            {
                throw new ArgumentNullException(nameof(baseResourceIdentifier));
            }

            Uri typeResource = resource.GetTypeIdentifier(baseResourceIdentifier);
            string escapedIdentifier = Uri.EscapeDataString(resource.Identifier);
            string resultValue = typeResource.ToString() + ServiceConstants.SeparatorSegments + escapedIdentifier;

            Uri result = new Uri(resultValue);
            return result;
        }

        /// <summary>
        /// This async method is used to serialize a message body to the request object.
        /// </summary>
        /// <param name="request">Contains the request object.</param>
        /// <param name="acceptLargeObjects">Contains a value indicating whether large objects are accepted</param>
        /// <returns>Returns a serialized object string.</returns>
        /// <exception cref="ArgumentNullException">Exception is thrown if the request is not specified.</exception>
        public static async Task<string> SerializeAsync(this HttpRequestMessage request, bool acceptLargeObjects = false)
        {
            string result = null;

            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            StringBuilder buffer = new StringBuilder();
            TextWriter textWriter = null;

            try
            {
#pragma warning disable CA2000 // Dispose objects before losing scope
                textWriter = new StringWriter(buffer);
#pragma warning restore CA2000 // Dispose objects before losing scope

                IHttpRequestMessageWriter requestWriter = null;
                try
                {
                    requestWriter = new HttpRequestMessageWriter(request, textWriter, acceptLargeObjects);
                    textWriter = null;
                    await requestWriter.WriteAsync();
                    await requestWriter.FlushAsync();
                    result = buffer.ToString();
                }
                finally
                {
                    if (requestWriter != null)
                    {
                        requestWriter.Close();
                        requestWriter = null;
                    }
                }
            }
            finally
            {
                if (textWriter != null)
                {
                    await textWriter.FlushAsync();
                    textWriter.Close();
#pragma warning disable IDE0059 // Unnecessary assignment of a value
                    textWriter = null;
#pragma warning restore IDE0059 // Unnecessary assignment of a value
                }
            }

            return result;
        }

        /// <summary>
        /// Converts an <see cref="IEnumerable" /> of type <typeparamref name="T" /> to a <see cref="IReadOnlyCollection{T}" /> object.
        /// </summary>
        /// <typeparam name="T">Contains the collection type.</typeparam>
        /// <param name="enumerable">Contains the enumerable to convert.</param>
        /// <returns>Returns a new collection type.</returns>
        /// <exception cref="ArgumentNullException">Exception is thrown if the enumerable is not specified.</exception>
        public static IReadOnlyCollection<T> ToCollection<T>(this IEnumerable enumerable)
        {
            if (enumerable == null)
            {
                throw new ArgumentNullException(nameof(enumerable));
            }

            IList<T> list = enumerable.Cast<T>().ToList();
            IReadOnlyCollection<T> result = list.ToArray();

            return result;
        }

        /// <summary>
        /// Converts an <see cref="ArrayList" /> of type <typeparamref name="T" /> to a <see cref="IReadOnlyCollection{T}" /> object.
        /// </summary>
        /// <typeparam name="T">Contains the collection type.</typeparam>
        /// <param name="array">Contains the array to convert.</param>
        /// <returns>Returns a new collection type.</returns>
        /// <exception cref="ArgumentNullException">Exception is thrown if the enumerable is not specified.</exception>
        public static IReadOnlyCollection<T> ToCollection<T>(this ArrayList array)
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }

            IList<T> list = new List<T>(array.Count);

            foreach (object item in array)
            {
                T typed = (T)item;
                list.Add(typed);
            }

            IReadOnlyCollection<T> result = list.ToArray();

            return result;
        }

        /// <summary>
        /// Converts the single item to a collection.
        /// </summary>
        /// <typeparam name="T">Contains the type of the item.</typeparam>
        /// <param name="item">Contains the item.</param>
        /// <returns>Returns the item in a new <see cref="IReadOnlyCollection{T}" /> object.</returns>
        public static IReadOnlyCollection<T> ToCollection<T>(this T item)
        {
            IReadOnlyCollection<T> result = new T[] { item };
            return result;
        }

        /// <summary>
        /// This method is used to find the first extension matching in the schema identifiers collection.
        /// </summary>
        /// <param name="schemaIdentifiers">Contains the schema identifiers.</param>
        /// <param name="extensions">Contains the extensions.</param>
        /// <param name="matchingExtension">Contains the matching extension found.</param>
        /// <returns>Returns a value indicating whether the extension matched.</returns>
        private static bool TryMatch(IReadOnlyCollection<string> schemaIdentifiers, IReadOnlyCollection<IExtension> extensions, out IExtension matchingExtension)
        {
            bool result = false;
            matchingExtension = null;

            if (extensions != null && schemaIdentifiers != null)
            {
                foreach (IExtension extension in extensions)
                {
                    if (schemaIdentifiers.Any(schemaIdentifier => extension.Matches(schemaIdentifier)))
                    {
                        matchingExtension = extension;
                        result = true;
                        break;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// This method is used to find the first schema identifier matching in the extensions collection.
        /// </summary>
        /// <param name="extensions">Contains the extensions.</param>
        /// <param name="schemaIdentifiers">Contains the schema identifiers.</param>
        /// <param name="matchingExtension">Contains the matching extension found.</param>
        /// <returns>Returns a value indicating whether the identifier matched.</returns>
        public static bool TryMatch(this IReadOnlyCollection<IExtension> extensions, IReadOnlyCollection<string> schemaIdentifiers, out IExtension matchingExtension)
        {
            bool result = ProtocolExtensions.TryMatch(schemaIdentifiers, extensions, out matchingExtension);
            return result;
        }

        /// <summary>
        /// This method is used to find the first schema identifier matching in the extensions collection.
        /// </summary>
        /// <param name="extensions">Contains the extensions.</param>
        /// <param name="schemaIdentifier">Contains the schema identifier.</param>
        /// <param name="matchingExtension">Contains the matching extension found.</param>
        /// <returns>Returns a value indicating whether the identifier matched.</returns>
        public static bool TryMatch(this IReadOnlyCollection<IExtension> extensions, string schemaIdentifier, out IExtension matchingExtension)
        {
            bool result = false;
            matchingExtension = null;

            if (!string.IsNullOrWhiteSpace(schemaIdentifier))
            {
                IReadOnlyCollection<string> schemaIdentifiers = schemaIdentifier.ToCollection();
                result = extensions.TryMatch(schemaIdentifiers, out matchingExtension);
            }

            return result;
        }

        /// <summary>
        /// This method is used to determine if a reference exists in the patch request.
        /// </summary>
        /// <param name="patch">Contains the patch request to search.</param>
        /// <param name="referee">Contains the reference to find.</param>
        /// <returns>Returns a value indicating whether the reference was found.</returns>
        /// <exception cref="ArgumentNullException">Exception is thrown if a patch or reference are not specified.</exception>
        public static bool References(this PatchRequest2Base<PatchOperation2Combined> patch, string referee)
        {
            if (patch == null)
            {
                throw new ArgumentNullException(nameof(patch));
            }

            if (string.IsNullOrWhiteSpace(referee))
            {
                throw new ArgumentNullException(nameof(referee));
            }

            bool result = patch.TryFindReference(referee, out IReadOnlyCollection<OperationValue> _);
            return result;
        }

        /// <summary>
        /// This method is used to try and find a reference in a patch request.
        /// </summary>
        /// <param name="patch">Contains the patch request.</param>
        /// <param name="referee">Contains the reference.</param>
        /// <param name="references">Contains references found.</param>
        /// <returns>Returns a value indicating whether the reference was found.</returns>
        /// <exception cref="ArgumentNullException">Exception is thrown if a patch or reference are not specified.</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "False analysis of the 'this' parameter of an extension method")]
        public static bool TryFindReference(this PatchRequest2Base<PatchOperation2Combined> patch, string referee, out IReadOnlyCollection<OperationValue> references)
        {
            if (patch == null)
            {
                throw new ArgumentNullException(nameof(patch));
            }

            references = null;

            if (string.IsNullOrWhiteSpace(referee))
            {
                throw new ArgumentNullException(nameof(referee));
            }

            List<OperationValue> patchOperation2Values = new List<OperationValue>();

            foreach (PatchOperation2Combined operation in patch.Operations)
            {
                OperationValue[] values = null;

                if (operation?.Value != null)
                {
                    values = JsonConvert.DeserializeObject<OperationValue[]>(operation.Value, ProtocolConstants.JsonSettings.Value);
                }

                if (values == null)
                {
                    string value = null;

                    if (operation?.Value != null)
                    {
                        value = JsonConvert.DeserializeObject<string>(operation.Value, ProtocolConstants.JsonSettings.Value);
                    }

                    OperationValue valueSingle = new OperationValue()
                    {
                        Value = value
                    };

                    patchOperation2Values.Add(valueSingle);
                }
                else
                {
                    foreach (OperationValue value in values)
                    {
                        patchOperation2Values.Add(value);
                    }
                }
            }

            IReadOnlyCollection<OperationValue> patchOperationValues = patchOperation2Values.AsReadOnly();

            IList<OperationValue> referencesBuffer = new List<OperationValue>(patchOperationValues.Count);

            foreach (OperationValue patchOperationValue in patchOperationValues)
            {
                if (!patchOperationValue.TryParseBulkIdentifierReferenceValue(out string value))
                {
                    value = patchOperationValue.Value;
                }

                if (string.Equals(referee, value, StringComparison.InvariantCulture))
                {
                    referencesBuffer.Add(patchOperationValue);
                }
            }

            references = referencesBuffer.ToArray();
            bool result = references.Any();
            return result;
        }

        /// <summary>
        /// This method is used to try and parse a bulk identifier reference value.
        /// </summary>
        /// <param name="value">Contains the value to parse.</param>
        /// <param name="bulkIdentifier">Contains the bulk identifier found.</param>
        /// <returns>Returns a value indicating whether the identifier was found.</returns>
        private static bool TryParseBulkIdentifierReferenceValue(string value, out string bulkIdentifier)
        {
            bulkIdentifier = null;
            bool result = false;

            if (!string.IsNullOrEmpty(value))
            {
                Match match = ProtocolExtensions.BulkIdentifierExpression.Value.Match(value);
                result = match.Success;

                if (result)
                {
                    bulkIdentifier = match.Groups[ProtocolExtensions.ExpressionGroupNameBulkIdentifier].Value;
                }
            }

            return result;
        }

        /// <summary>
        /// This method is used to try and parse a bulk identifier reference value.
        /// </summary>
        /// <param name="value">Contains the operation value to parse.</param>
        /// <param name="bulkIdentifier">Contains the bulk identifier found.</param>
        /// <returns>Returns a value indicating whether the identifier was found.</returns>
        public static bool TryParseBulkIdentifierReferenceValue(this OperationValue value, out string bulkIdentifier)
        {
            bulkIdentifier = null;
            bool result = ProtocolExtensions.TryParseBulkIdentifierReferenceValue(value.Value, out bulkIdentifier);
            return result;
        }

        /// <summary>
        /// This class is used for Request message writing.
        /// </summary>
        private sealed class HttpRequestMessageWriter : IHttpRequestMessageWriter
        {
            /// <summary>
            /// Contains the header template.
            /// </summary>
            private const string TemplateHeader = "{0}: {1}";

            /// <summary>
            /// Contains a lock.
            /// </summary>
            private readonly object thisLock = new object();

            /// <summary>
            /// Contains the inner text writer.
            /// </summary>
            private TextWriter innerWriter;

            /// <summary>
            /// Initializes a new instance of the <see cref="HttpRequestMessageWriter" /> class.
            /// </summary>
            /// <param name="message">Contains the Request message.</param>
            /// <param name="writer">Contains the text writer.</param>
            /// <param name="acceptLargeObjects">Contains a value indicating whether large objects are accepted.</param>
            /// <exception cref="ArgumentNullException">Exception is thrown if message or writer are not specified.</exception>
            public HttpRequestMessageWriter(HttpRequestMessage message, TextWriter writer, bool acceptLargeObjects)
            {
                this.Message = message ?? throw new ArgumentNullException(nameof(message));
                this.innerWriter = writer ?? throw new ArgumentNullException(nameof(writer));
                this.AcceptLargeObjects = acceptLargeObjects;
            }

            /// <summary>
            /// Gets a value indicating whether large objects are accepted.
            /// </summary>
            private bool AcceptLargeObjects { get; }

            /// <summary>
            /// Gets or sets the request message.
            /// </summary>
            private HttpRequestMessage Message { get; set; }

            /// <summary>
            /// This method is used to close the text writer.
            /// </summary>
            public void Close()
            {
                this.innerWriter.Flush();
                this.innerWriter.Close();
            }

            /// <summary>
            /// Disposes of inner objects.
            /// </summary>
            public void Dispose()
            {
                if (this.innerWriter != null)
                {
                    lock (this.thisLock)
                    {
                        if (this.innerWriter != null)
                        {
                            this.Close();
                            this.innerWriter = null;
                        }
                    }
                }
            }

            /// <summary>
            /// This method is used to flush the text writer stream.
            /// </summary>
            /// <returns>Returns an async task object.</returns>
            public async Task FlushAsync()
            {
                await this.innerWriter.FlushAsync().ConfigureAwait(false);
            }

            /// <summary>
            /// This method is used to write to the text stream.
            /// </summary>
            /// <returns>Returns an async task object.</returns>
            public async Task WriteAsync()
            {
                if (this.Message.RequestUri != null)
                {
                    string line = HttpUtility.UrlDecode(this.Message.RequestUri.AbsoluteUri);
                    await this.innerWriter.WriteLineAsync(line).ConfigureAwait(false);
                }

                if (this.Message.Headers != null)
                {
                    foreach (KeyValuePair<string, IEnumerable<string>> header in this.Message.Headers)
                    {
                        if (!header.Value.Any())
                        {
                            continue;
                        }

                        string value;

                        if (header.Value.LongCount() == 1)
                        {
                            value = header.Value.Single();
                        }
                        else
                        {
                            string[] values = header.Value.ToArray();
                            value = JsonFactory.Instance.Create(values, this.AcceptLargeObjects);
                        }

                        string line = string.Format(CultureInfo.InvariantCulture, HttpRequestMessageWriter.TemplateHeader, header.Key, value);
                        await this.innerWriter.WriteLineAsync(line);
                    }
                }

                if (this.Message.Content != null)
                {
                    string line = await this.Message.Content.ReadAsStringAsync();
                    await this.innerWriter.WriteLineAsync(line);
                }
            }
        }
    }
}