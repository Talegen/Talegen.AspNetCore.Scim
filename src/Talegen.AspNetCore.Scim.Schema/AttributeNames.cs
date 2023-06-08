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
    /// <summary>
    /// This class contains SCIM attribute names.
    /// </summary>
    public static class AttributeNames
    {
        /// <summary>
        /// The account enabled
        /// </summary>
        public const string AccountEnabled = "accountEnabled";

        /// <summary>
        /// The active
        /// </summary>
        public const string Active = "active";

        /// <summary>
        /// The addresses
        /// </summary>
        public const string Addresses = "addresses";

        /// <summary>
        /// The attributes
        /// </summary>
        public const string Attributes = "attributes";

        /// <summary>
        /// The authentication schemes
        /// </summary>
        public const string AuthenticationSchemes = "authenticationSchemes";

        /// <summary>
        /// The bulk
        /// </summary>
        public const string Bulk = "bulk";

        /// <summary>
        /// The canonical values
        /// </summary>
        public const string CanonicalValues = "canonicalValues";

        /// <summary>
        /// The case exact
        /// </summary>
        public const string CaseExact = "caseExact";

        /// <summary>
        /// The change password
        /// </summary>
        public const string ChangePassword = "changePassword";

        /// <summary>
        /// The change polling interval
        /// </summary>
        public const string ChangePollingInterval = NotBefore;

        /// <summary>
        /// The change watermark
        /// </summary>
        public const string ChangeWatermark = Watermark;

        /// <summary>
        /// The change watermark lifetime
        /// </summary>
        public const string ChangeWatermarkLifetime = Expiry;

        /// <summary>
        /// The country
        /// </summary>
        public const string Country = "country";

        /// <summary>
        /// The cost center
        /// </summary>
        public const string CostCenter = "costCenter";

        /// <summary>
        /// The created
        /// </summary>
        public const string Created = "created";

        /// <summary>
        /// The department
        /// </summary>
        public const string Department = "department";

        /// <summary>
        /// The description
        /// </summary>
        public const string Description = "description";

        /// <summary>
        /// The display
        /// </summary>
        public const string Display = "display";

        /// <summary>
        /// The display name
        /// </summary>
        public const string DisplayName = "displayName";

        /// <summary>
        /// The division
        /// </summary>
        public const string Division = "division";

        /// <summary>
        /// The documentation
        /// </summary>
        public const string Documentation = "documentationUrl";

        /// <summary>
        /// The electronic mail addresses
        /// </summary>
        public const string ElectronicMailAddresses = "emails";

        /// <summary>
        /// The employee number
        /// </summary>
        public const string EmployeeNumber = "employeeNumber";

        /// <summary>
        /// The endpoint
        /// </summary>
        public const string Endpoint = "endpoint";

        /// <summary>
        /// The entity tag
        /// </summary>
        public const string EntityTag = "eTag";

        /// <summary>
        /// The expiry
        /// </summary>
        public const string Expiry = "exp";

        /// <summary>
        /// The extension enterprise user2
        /// </summary>
        public const string ExtensionEnterpriseUser2 = SchemaIdentifiers.Core2EnterpriseUser;

        /// <summary>
        /// The external identifier
        /// </summary>
        public const string ExternalIdentifier = "externalId";

        /// <summary>
        /// The family name
        /// </summary>
        public const string FamilyName = "familyName";

        /// <summary>
        /// The filter
        /// </summary>
        public const string Filter = "filter";

        /// <summary>
        /// The given name
        /// </summary>
        public const string GivenName = "givenName";

        /// <summary>
        /// The formatted
        /// </summary>
        public const string Formatted = "formatted";

        /// <summary>
        /// The honorific prefix
        /// </summary>
        public const string HonorificPrefix = "honorificPrefix";

        /// <summary>
        /// The honorific suffix
        /// </summary>
        public const string HonorificSuffix = "honorificSuffix";

        /// <summary>
        /// The identifier
        /// </summary>
        public const string Identifier = "id";

        /// <summary>
        /// The ims
        /// </summary>
        public const string Ims = "ims";

        /// <summary>
        /// The is deleted
        /// </summary>
        public const string IsDeleted = "isDeleted";

        /// <summary>
        /// The last modified
        /// </summary>
        public const string LastModified = "lastModified";

        /// <summary>
        /// The locale
        /// </summary>
        public const string Locale = "locale";

        /// <summary>
        /// The locality
        /// </summary>
        public const string Locality = "locality";

        /// <summary>
        /// The location
        /// </summary>
        public const string Location = "location";

        /// <summary>
        /// The mail enabled
        /// </summary>
        public const string MailEnabled = "mailEnabled";

        /// <summary>
        /// The mail nickname
        /// </summary>
        public const string MailNickname = "mailNickname";

        /// <summary>
        /// The manager
        /// </summary>
        public const string Manager = "manager";

        /// <summary>
        /// The manager identifier
        /// </summary>
        public const string ManagerId = "managerId";

        /// <summary>
        /// The maximum operations
        /// </summary>
        public const string MaximumOperations = "maxOperations";

        /// <summary>
        /// The maximum payload size
        /// </summary>
        public const string MaximumPayloadSize = "maxPayloadSize";

        /// <summary>
        /// The members
        /// </summary>
        public const string Members = "members";

        /// <summary>
        /// The metadata
        /// </summary>
        public const string Metadata = "meta";

        /// <summary>
        /// The mutability
        /// </summary>
        public const string Mutability = "mutability";

        /// <summary>
        /// The name
        /// </summary>
        public const string Name = "name";

        /// <summary>
        /// The nickname
        /// </summary>
        public const string Nickname = "nickName";

        /// <summary>
        /// The not before
        /// </summary>
        public const string NotBefore = "nbf";

        /// <summary>
        /// The on premises security identifier
        /// </summary>
        public const string OnPremisesSecurityIdentifier = "onPremisesSecurityIdentifier";

        /// <summary>
        /// The organization
        /// </summary>
        public const string Organization = "organization";

        /// <summary>
        /// The other
        /// </summary>
        public const string Other = "other";

        /// <summary>
        /// The password
        /// </summary>
        public const string Password = "password";

        /// <summary>
        /// The patch
        /// </summary>
        public const string Patch = "patch";

        /// <summary>
        /// The path
        /// </summary>
        public const string Path = "path";

        /// <summary>
        /// The phone numbers
        /// </summary>
        public const string PhoneNumbers = "phoneNumbers";

        /// <summary>
        /// The photos
        /// </summary>
        public const string Photos = "photos";

        /// <summary>
        /// The plural
        /// </summary>
        public const string Plural = "multiValued";

        /// <summary>
        /// The postal code
        /// </summary>
        public const string PostalCode = "postalCode";

        /// <summary>
        /// The preferred language
        /// </summary>
        public const string PreferredLanguage = "preferredLanguage";

        /// <summary>
        /// The primary
        /// </summary>
        public const string Primary = "primary";

        /// <summary>
        /// The profile URL
        /// </summary>
        public const string ProfileUrl = "profileUrl";

        /// <summary>
        /// The proxy addresses
        /// </summary>
        public const string ProxyAddresses = "proxyAddresses";

        /// <summary>
        /// The reference types
        /// </summary>
        public const string ReferenceTypes = "referenceTypes";

        /// <summary>
        /// The region
        /// </summary>
        public const string Region = "region";

        /// <summary>
        /// The roles
        /// </summary>
        public const string Roles = "roles";

        /// <summary>
        /// The required
        /// </summary>
        public const string Required = "required";

        /// <summary>
        /// The resource type
        /// </summary>
        public const string ResourceType = "resourceType";

        /// <summary>
        /// The returned
        /// </summary>
        public const string Returned = "returned";

        /// <summary>
        /// The schema
        /// </summary>
        public const string Schema = "schema";

        /// <summary>
        /// The schemas
        /// </summary>
        public const string Schemas = "schemas";

        /// <summary>
        /// The security enabled
        /// </summary>
        public const string SecurityEnabled = "securityEnabled";

        /// <summary>
        /// The sort
        /// </summary>
        public const string Sort = "sort";

        /// <summary>
        /// The specification
        /// </summary>
        public const string Specification = "specUrl";

        /// <summary>
        /// The street address
        /// </summary>
        public const string StreetAddress = "streetAddress";

        /// <summary>
        /// The sub attributes
        /// </summary>
        public const string SubAttributes = "subAttributes";

        /// <summary>
        /// The supported
        /// </summary>
        public const string Supported = "supported";

        /// <summary>
        /// The time zone
        /// </summary>
        public const string TimeZone = "timezone";

        /// <summary>
        /// The title
        /// </summary>
        public const string Title = "title";

        /// <summary>
        /// The type
        /// </summary>
        public const string Type = "type";

        /// <summary>
        /// The uniqueness
        /// </summary>
        public const string Uniqueness = "uniqueness";

        /// <summary>
        /// The user name
        /// </summary>
        public const string UserName = "userName";

        /// <summary>
        /// The user type
        /// </summary>
        public const string UserType = "userType";

        /// <summary>
        /// The value
        /// </summary>
        public const string Value = "value";

        /// <summary>
        /// The version
        /// </summary>
        public const string Version = "version";

        /// <summary>
        /// The watermark
        /// </summary>
        public const string Watermark = "watermark";
    }
}