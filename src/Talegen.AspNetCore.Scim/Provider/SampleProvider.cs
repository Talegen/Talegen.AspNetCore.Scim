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

namespace Talegen.AspNetCore.Scim.Provider
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using Protocol;
    using Schema;
    using Service;

    /// <summary>
    /// This is an in-memory sample provider used for testing purposes.
    /// </summary>
    public sealed class SampleProvider : ProviderBase
    {
        /// <summary>
        /// Contains a test user home email address.
        /// </summary>
        public const string ElectronicMailAddressHome = "babs@jensen.org";

        /// <summary>
        /// Contains a test user work email address.
        /// </summary>
        public const string ElectronicMailAddressWork = "bjensen@example.com";

        /// <summary>
        /// Contains the test user cost center.
        /// </summary>
        public const string ExtensionAttributeEnterpriseUserCostCenter = "4130";

        /// <summary>
        /// Contains the test user department.
        /// </summary>
        public const string ExtensionAttributeEnterpriseUserDepartment = "Tour Operations";

        /// <summary>
        /// Contains the test user division.
        /// </summary>
        public const string ExtensionAttributeEnterpriseUserDivision = "Theme Park";

        /// <summary>
        /// Contains the test user employee number.
        /// </summary>
        public const string ExtensionAttributeEnterpriseUserEmployeeNumber = "701984";

        /// <summary>
        /// Contains the test user organization.
        /// </summary>
        public const string ExtensionAttributeEnterpriseUserOrganization = "Universal Studios";

        /// <summary>
        /// Contains the test group name.
        /// </summary>
        public const string GroupName = "Creative & Skinning";

        /// <summary>
        /// Contains the test user external identifier.
        /// </summary>
        public const string IdentifierExternal = "bjensen";

        /// <summary>
        /// Contains the test group identifier.
        /// </summary>
        public const string IdentifierGroup = "acbf3ae7-8463-4692-b4fd-9b4da3f908ce";

        /// <summary>
        /// Contains a test role identifier.
        /// </summary>
        public const string IdentifierRole = "DA3B77DF-F495-45C7-9AAC-EC083B99A9D3";

        /// <summary>
        /// Contains a test user identifier.
        /// </summary>
        public const string IdentifierUser = "2819c223-7f76-453a-919d-413861904646";

        /// <summary>
        /// Contains the max page limit.
        /// </summary>
        public const int LimitPageSize = 6;

        /// <summary>
        /// Contains the test locale.
        /// </summary>
        public const string Locale = "en-Us";

        /// <summary>
        /// Contains the test user manager display name.
        /// </summary>
        public const string ManagerDisplayName = "John Smith";

        /// <summary>
        /// Contains the test manager identifier.
        /// </summary>
        public const string ManagerIdentifier = "26118915-6090-4610-87e4-49d8ca9f808d";

        /// <summary>
        /// Contains the test user photo.
        /// </summary>
        public const string PhotoValue = "https://photos.example.com/profilephoto/72930000000Ccne/F";

        /// <summary>
        /// Contains the test user profile url.
        /// </summary>
        public const string ProfileUrl = "https://login.example.com/bjensen";

        /// <summary>
        /// Contains the test user description.
        /// </summary>
        public const string RoleDescription = "Attends an educational institution";

        /// <summary>
        /// Contains the test role display name.
        /// </summary>
        public const string RoleDisplay = "Student";

        /// <summary>
        /// Contains the test role value.
        /// </summary>
        public const string RoleValue = "student";

        /// <summary>
        /// Contains the test time zone.
        /// </summary>
        public const string TimeZone = "America/New_York";

        /// <summary>
        /// Contains the test user type.
        /// </summary>
        public const string UserType = "Employee";

        /// <summary>
        /// Contains the test manager family name.
        /// </summary>
        private const string NameFamily = "Jensen";

        /// <summary>
        /// Contains the test manager formatted name.
        /// </summary>
        private const string NameFormatted = "Ms. Barbara J Jensen III";

        /// <summary>
        /// Contains the test user given name.
        /// </summary>
        private const string NameGiven = "Barbara";

        /// <summary>
        /// Contains the test user name prefix.
        /// </summary>
        private const string NameHonorificPrefix = "Ms.";

        /// <summary>
        /// Contains the test user name suffix.
        /// </summary>
        private const string NameHonorificSuffix = "III";

        /// <summary>
        /// Contains the test user name.
        /// </summary>
        private const string NameUser = "bjensen";

        /// <summary>
        /// Contains the sample email addresses.
        /// </summary>
        private readonly IReadOnlyCollection<ElectronicMailAddress> sampleElectronicMailAddresses;

        /// <summary>
        /// Contains the sample home email address.
        /// </summary>
        private readonly ElectronicMailAddress sampleElectronicMailAddressHome;

        /// <summary>
        /// Contains the sample work email address.
        /// </summary>
        private readonly ElectronicMailAddress sampleElectronicMailAddressWork;

        /// <summary>
        /// Contains a sample group.
        /// </summary>
        private readonly Core2Group sampleGroup;

        /// <summary>
        /// Contains the sample manager.
        /// </summary>
        private readonly Manager sampleManager;

        /// <summary>
        /// Contains the sample name.
        /// </summary>
        private readonly Name sampleName;

        /// <summary>
        /// Contains a sample operation.
        /// </summary>
        private readonly PatchOperation2Combined sampleOperation;

        /// <summary>
        /// Contains the sample operation value.
        /// </summary>
        private readonly OperationValue sampleOperationValue;

        /// <summary>
        /// Contains a sample patch.
        /// </summary>
        private readonly PatchRequest2 samplePatch;

        /// <summary>
        /// Contains a sample user.
        /// </summary>
        private readonly Core2EnterpriseUser sampleUser;

        /// <summary>
        /// Initializes a new instance of the <see cref="SampleProvider" /> class.
        /// </summary>
        public SampleProvider()
        {
            this.sampleElectronicMailAddressHome = new ElectronicMailAddress
            {
                ItemType = ElectronicMailAddressBase.Home,
                Value = SampleProvider.ElectronicMailAddressHome
            };

            this.sampleElectronicMailAddressWork = new ElectronicMailAddress
            {
                ItemType = ElectronicMailAddressWork,
                Primary = true,
                Value = SampleProvider.ElectronicMailAddressWork
            };

            this.sampleElectronicMailAddresses = new ElectronicMailAddress[]
            {
                this.sampleElectronicMailAddressHome,
                this.sampleElectronicMailAddressWork
            };

            this.sampleManager = new Manager
            {
                Value = SampleProvider.ManagerIdentifier,
            };

            this.sampleName = new Name
            {
                FamilyName = SampleProvider.NameFamily,
                Formatted = SampleProvider.NameFormatted,
                GivenName = SampleProvider.NameGiven,
                HonorificPrefix = SampleProvider.NameHonorificPrefix,
                HonorificSuffix = SampleProvider.NameHonorificSuffix
            };

            this.sampleOperationValue = new OperationValue
            {
                Value = SampleProvider.IdentifierUser
            };

            this.sampleOperation = this.ConstructOperation();

            this.samplePatch = this.ConstructPatch();

            this.sampleUser = new Core2EnterpriseUser
            {
                Active = true,
                ElectronicMailAddresses = this.sampleElectronicMailAddresses,
                ExternalIdentifier = SampleProvider.IdentifierExternal,
                Identifier = SampleProvider.IdentifierUser,
                Name = this.sampleName,
                UserName = SampleProvider.NameUser
            };

            ExtensionAttributeEnterpriseUser2 enterpriseExtensionAttributeEnterpriseUser2 = new ExtensionAttributeEnterpriseUser2
            {
                CostCenter = SampleProvider.ExtensionAttributeEnterpriseUserCostCenter,
                Department = SampleProvider.ExtensionAttributeEnterpriseUserDepartment,
                Division = SampleProvider.ExtensionAttributeEnterpriseUserDivision,
                EmployeeNumber = SampleProvider.ExtensionAttributeEnterpriseUserEmployeeNumber,
                Manager = this.sampleManager,
                Organization = SampleProvider.ExtensionAttributeEnterpriseUserOrganization
            };

            this.SampleUser.EnterpriseExtension = enterpriseExtensionAttributeEnterpriseUser2;

            this.sampleGroup = new Core2Group
            {
                DisplayName = SampleProvider.GroupName,
            };
        }

        /// <summary>
        /// Gets the sample group.
        /// </summary>
        public Core2Group SampleGroup => this.sampleGroup;

        /// <summary>
        /// Gets the sample patch.
        /// </summary>
        public PatchRequest2 SamplePatch => this.samplePatch;

        /// <summary>
        /// Gets the sample resource.
        /// </summary>
        public Core2EnterpriseUser SampleResource => this.SampleUser;

        /// <summary>
        /// Gets the sample user.
        /// </summary>
        public Core2EnterpriseUser SampleUser => this.sampleUser;

        /// <inheritdoc />
        public override Task<Resource> CreateAsync(Resource resource, string correlationIdentifier)
        {
            if (resource == null)
            {
                throw new ArgumentNullException(nameof(resource));
            }

            resource.Identifier = SampleProvider.IdentifierUser;

            Task<Resource> result = Task.FromResult(resource);
            return result;
        }

        /// <inheritdoc />
        public override Task DeleteAsync(IResourceIdentifier resourceIdentifier, string correlationIdentifier)
        {
            if (resourceIdentifier == null)
            {
                throw new ArgumentNullException(nameof(resourceIdentifier));
            }

            Task result = Task.WhenAll();
            return result;
        }

        /// <inheritdoc />
        public override async Task<QueryResponseBase> PaginateQueryAsync(IRequest<IQueryParameters> request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (request.Payload == null)
            {
                throw new ArgumentException(Scim.Schema.Properties.Resources.ExceptionInvalidRequest);
            }

            if (string.IsNullOrWhiteSpace(request.CorrelationIdentifier))
            {
                throw new ArgumentException(Scim.Schema.Properties.Resources.ExceptionInvalidRequest);
            }

            IReadOnlyCollection<Resource> resources = await this.QueryAsync(request).ConfigureAwait(false);
            QueryResponseBase result = new QueryResponse(resources);

            if (request.Payload.PaginationParameters == null)
            {
                result.TotalResults = result.ItemsPerPage = resources.Count;
            }

            return result;
        }

        /// <inheritdoc />
        public override Task<Resource[]> QueryAsync(IQueryParameters parameters, string correlationIdentifier)
        {
            Resource[] resources = this.Query(parameters);
            Task<Resource[]> result = Task.FromResult(resources);
            return result;
        }

        /// <inheritdoc />
        public override Task<Resource> ReplaceAsync(Resource resource, string correlationIdentifier)
        {
            Task<Resource> result;

            if (resource == null)
            {
                throw new ArgumentNullException(nameof(resource));
            }

            if (resource.Identifier == null)
            {
                throw new ArgumentException(Scim.Schema.Properties.Resources.ExceptionInvalidResource);
            }

            if (resource.Is(SchemaIdentifiers.Core2EnterpriseUser) &&
                string.Equals(resource.Identifier, SampleProvider.IdentifierUser, StringComparison.OrdinalIgnoreCase))
            {
                result = Task.FromResult(resource);
            }
            else
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            return result;
        }

        /// <inheritdoc />
        public override Task<Resource> RetrieveAsync(IResourceRetrievalParameters parameters, string correlationIdentifier)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            if (parameters.ResourceIdentifier == null)
            {
                throw new ArgumentException(Scim.Schema.Properties.Resources.ExceptionInvalidParameters);
            }

            Resource resource = null;

            if (parameters.ResourceIdentifier.SchemaIdentifier == SchemaIdentifiers.Core2EnterpriseUser &&
                string.Equals(parameters.ResourceIdentifier.Identifier, SampleProvider.IdentifierUser, StringComparison.OrdinalIgnoreCase))
            {
                resource = this.SampleUser;
            }

            Task<Resource> result = Task.FromResult(resource);
            return result;
        }

        /// <inheritdoc />
        public override Task UpdateAsync(IPatch patch, string correlationIdentifier)
        {
            if (patch == null)
            {
                throw new ArgumentNullException(nameof(patch));
            }

            Task result = Task.WhenAll();
            return result;
        }

        /// <summary>
        /// </summary>
        /// <param name="containerIdentifier"></param>
        /// <param name="memberAttributePath"></param>
        /// <param name="memberIdentifier"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        private static bool HasMember(IResourceIdentifier containerIdentifier, string memberAttributePath, string memberIdentifier)
        {
            if (containerIdentifier == null)
            {
                throw new ArgumentNullException(nameof(containerIdentifier));
            }

            if (string.IsNullOrWhiteSpace(memberAttributePath))
            {
                throw new ArgumentNullException(nameof(memberAttributePath));
            }

            if (string.IsNullOrWhiteSpace(memberIdentifier))
            {
                throw new ArgumentNullException(nameof(memberIdentifier));
            }

            if (string.IsNullOrWhiteSpace(containerIdentifier.Identifier))
            {
                throw new ArgumentException(Scim.Schema.Properties.Resources.ExceptionInvalidIdentifier);
            }

            if (memberAttributePath != AttributeNames.Members)
            {
                string exceptionMessage = string.Format(CultureInfo.InvariantCulture, Scim.Schema.Properties.Resources.ExceptionFilterAttributePathNotSupportedTemplate, memberAttributePath);
                throw new NotSupportedException(exceptionMessage);
            }

            if (containerIdentifier.SchemaIdentifier != SchemaIdentifiers.Core2Group)
            {
                throw new NotSupportedException(Scim.Schema.Properties.Resources.ExceptionFilterNotSupported);
            }

            bool result = string.Equals(SampleProvider.IdentifierGroup, containerIdentifier.Identifier, StringComparison.OrdinalIgnoreCase)
                          && string.Equals(SampleProvider.IdentifierUser, memberIdentifier, StringComparison.OrdinalIgnoreCase);

            return result;
        }

        /// <summary>
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        private static Resource[] QueryMember(IQueryParameters parameters, IFilter filter)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            if (filter == null)
            {
                throw new ArgumentNullException(nameof(filter));
            }

            if (filter.AdditionalFilter == null)
            {
                throw new ArgumentException(Scim.Schema.Properties.Resources.ExceptionQueryNotSupported);
            }

            Resource[] results = null;

            if (parameters.ExcludedAttributePaths != null && parameters.ExcludedAttributePaths.Any())
            {
                throw new ArgumentException(Scim.Schema.Properties.Resources.ExceptionQueryNotSupported);
            }

            if (!string.Equals(parameters.SchemaIdentifier, SchemaIdentifiers.Core2Group, StringComparison.Ordinal))
            {
                throw new NotSupportedException(Scim.Schema.Properties.Resources.ExceptionQueryNotSupported);
            }

            if (null == parameters.RequestedAttributePaths || !parameters.RequestedAttributePaths.Any())
            {
                throw new NotSupportedException(Scim.Schema.Properties.Resources.ExceptionQueryNotSupported);
            }

            if (filter.AdditionalFilter.AdditionalFilter != null)
            {
                throw new NotSupportedException(Scim.Schema.Properties.Resources.ExceptionQueryNotSupported);
            }

            string selectedAttribute = parameters.RequestedAttributePaths.SingleOrDefault();

            if (string.IsNullOrWhiteSpace(selectedAttribute))
            {
                throw new NotSupportedException(Scim.Schema.Properties.Resources.ExceptionQueryNotSupported);
            }

            if (!string.Equals(selectedAttribute, AttributeNames.Identifier, StringComparison.OrdinalIgnoreCase))
            {
                throw new NotSupportedException(Scim.Schema.Properties.Resources.ExceptionQueryNotSupported);
            }

            IReadOnlyCollection<IFilter> filters = new IFilter[]
            {
                filter,
                filter.AdditionalFilter
            };

            IFilter filterIdentifier = filters.SingleOrDefault(item => item.AttributePath.Equals(AttributeNames.Identifier, StringComparison.OrdinalIgnoreCase));

            if (filterIdentifier == null)
            {
                throw new NotSupportedException(Scim.Schema.Properties.Resources.ExceptionQueryNotSupported);
            }

            IFilter filterMembers = filters.SingleOrDefault(item => item.AttributePath.Equals(AttributeNames.Members, StringComparison.OrdinalIgnoreCase));

            if (filterMembers == null)
            {
                throw new NotSupportedException(Scim.Schema.Properties.Resources.ExceptionQueryNotSupported);
            }

            IResourceIdentifier containerIdentifier = new ResourceIdentifier
            {
                SchemaIdentifier = parameters.SchemaIdentifier,
                Identifier = filterIdentifier.ComparisonValue
            };

            if (!SampleProvider.HasMember(containerIdentifier, filterMembers.AttributePath, filterMembers.ComparisonValue))
            {
                results = Array.Empty<Resource>();
            }
            else
            {
                Resource container = new Core2Group
                {
                    Identifier = containerIdentifier.Identifier
                };

                results = container.ToCollection().ToArray();
            }

            return results;
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        private PatchOperation2Combined ConstructOperation()
        {
            IPath path = Protocol.Path.Create(AttributeNames.Members);
            PatchOperation2Combined result = new PatchOperation2Combined
            {
                Name = OperationName.Add,
                Path = path
            };
            result.Value = JsonConvert.SerializeObject(this.sampleOperationValue);
            return result;
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        private PatchRequest2 ConstructPatch()
        {
            PatchRequest2 result = new PatchRequest2();
            result.AddOperation(this.sampleOperation);
            return result;
        }

        /// <summary>
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        private Resource[] Query(IQueryParameters parameters)
        {
            Resource[] result;

            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            if (parameters.AlternateFilters.Count != 1)
            {
                throw new NotSupportedException(Scim.Schema.Properties.Resources.ExceptionFilterCount);
            }

            if (parameters.PaginationParameters != null)
            {
                string exceptionMessage = string.Format(CultureInfo.InvariantCulture, Scim.Schema.Properties.Resources.ExceptionPaginationIsNotSupportedTemplate, parameters.SchemaIdentifier);
                throw new NotSupportedException(exceptionMessage);
            }

            IFilter filter = parameters.AlternateFilters.Single();

            if (filter.AdditionalFilter != null)
            {
                result = QueryMember(parameters, filter);
            }
            else if (string.Equals(parameters.SchemaIdentifier, SchemaIdentifiers.Core2EnterpriseUser, StringComparison.OrdinalIgnoreCase))
            {
                result = this.QueryUsers(parameters, filter);
            }
            else if (string.Equals(parameters.SchemaIdentifier, SchemaIdentifiers.Core2Group, StringComparison.OrdinalIgnoreCase))
            {
                result = this.QueryGroups(parameters, filter);
            }
            else
            {
                string exceptionMessage =
                        string.Format(
                            CultureInfo.InvariantCulture,
                            Scim.Schema.Properties.Resources.ExceptionFilterAttributePathNotSupportedTemplate,
                            filter.AttributePath);
                throw new NotSupportedException(exceptionMessage);
            }

            return result;
        }

        private Resource[] QueryGroups(IQueryParameters parameters, IFilter filter)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            if (filter == null)
            {
                throw new ArgumentNullException(nameof(filter));
            }

            if (parameters.ExcludedAttributePaths == null ||
                !parameters.ExcludedAttributePaths.Any() ||
                parameters.ExcludedAttributePaths.Count != 1 ||
                !parameters.ExcludedAttributePaths.Single().Equals(AttributeNames.Members, StringComparison.Ordinal))
            {
                throw new ArgumentException(Scim.Schema.Properties.Resources.ExceptionQueryNotSupported);
            }

            if (!string.Equals(filter.AttributePath, AttributeNames.ExternalIdentifier, StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(filter.AttributePath, AttributeNames.DisplayName, StringComparison.OrdinalIgnoreCase))
            {
                string exceptionMessage = string.Format(CultureInfo.InvariantCulture, Scim.Schema.Properties.Resources.ExceptionFilterAttributePathNotSupportedTemplate, filter.AttributePath);
                throw new NotSupportedException(exceptionMessage);
            }

            if (filter.FilterOperator != ComparisonOperator.Equals)
            {
                string exceptionMessage = string.Format(CultureInfo.InvariantCulture, Scim.Schema.Properties.Resources.ExceptionFilterOperatorNotSupportedTemplate, filter.FilterOperator);
                throw new NotSupportedException(exceptionMessage);
            }

            Resource[] results = !string.Equals(filter.ComparisonValue, SampleProvider.GroupName, StringComparison.OrdinalIgnoreCase) ?
                Enumerable.Empty<Resource>().ToArray() :
                this.sampleGroup.ToCollection().ToArray();

            return results;
        }

        /// <summary>
        /// </summary>
        /// <param name="parameters"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        private Resource[] QueryUsers(IQueryParameters parameters, IFilter filter)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            if (filter == null)
            {
                throw new ArgumentNullException(nameof(filter));
            }

            if (parameters.ExcludedAttributePaths != null && parameters.ExcludedAttributePaths.Any())
            {
                throw new ArgumentException(Scim.Schema.Properties.Resources.ExceptionQueryNotSupported);
            }

            if (!string.Equals(filter.AttributePath, AttributeNames.ExternalIdentifier, StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(filter.AttributePath, AttributeNames.UserName, StringComparison.OrdinalIgnoreCase))
            {
                string exceptionMessage = string.Format(CultureInfo.InvariantCulture, Scim.Schema.Properties.Resources.ExceptionFilterAttributePathNotSupportedTemplate, filter.AttributePath);
                throw new NotSupportedException(exceptionMessage);
            }

            if (filter.FilterOperator != ComparisonOperator.Equals)
            {
                string exceptionMessage = string.Format(CultureInfo.InvariantCulture, Scim.Schema.Properties.Resources.ExceptionFilterOperatorNotSupportedTemplate, filter.FilterOperator);
                throw new NotSupportedException(exceptionMessage);
            }

            Resource[] results;

            if (!string.Equals(filter.ComparisonValue, SampleProvider.IdentifierExternal, StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(filter.ComparisonValue, this.SampleUser.UserName, StringComparison.OrdinalIgnoreCase))
            {
                results = Enumerable.Empty<Resource>().ToArray();
            }
            else
            {
                results = this.SampleUser.ToCollection().ToArray();
            }

            return results;
        }
    }
}