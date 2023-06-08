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
    using Newtonsoft.Json;
    using Schema;

    /// <summary>
    /// This class contains extension methods for working with user and group requests.
    /// </summary>
    public static class Core2EnterpriseUserExtensions
    {
        /// <summary>
        /// This method is used to apply patch requests to a specified user model.
        /// </summary>
        /// <param name="user">Contains the user model to update.</param>
        /// <param name="patch">Contains the patch request model.</param>
        /// <exception cref="ArgumentNullException">Thrown if user model is invalid.</exception>
        public static void Apply(this Core2EnterpriseUser user, PatchRequest2Base<PatchOperation2> patch)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            foreach (PatchOperation2 operation in patch.Operations)
            {
                user.Apply(operation);
            }
        }

        /// <summary>
        /// This method is used to apply a specific patch request to a specified model.
        /// </summary>
        /// <param name="user">Contains the user model to update.</param>
        /// <param name="patch">Contains the patch request model.</param>
        /// <exception cref="ArgumentNullException">Thrown if the user model is invalid.</exception>
        public static void Apply(this Core2EnterpriseUser user, PatchRequest2 patch)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            foreach (PatchOperation2Combined operation in patch.Operations)
            {
                PatchOperation2 operationInternal = new PatchOperation2
                {
                    OperationName = operation.OperationName,
                    Path = operation.Path
                };

                var values = JsonConvert.DeserializeObject<OperationValue[]>(operation.Value, ProtocolConstants.JsonSettings.Value);

                if (values == null)
                {
                    OperationValue valueSingle = new OperationValue
                    {
                        Value = JsonConvert.DeserializeObject<string>(operation.Value, ProtocolConstants.JsonSettings.Value) ?? string.Empty
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

                user.Apply(operationInternal);
            }
        }

        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "None")]
        /// <summary>
        /// This method is used to apply a specific patch operation to a specific model.
        /// </summary>
        /// <param name="user">Contains the user model to update.</param>
        /// <param name="operation">Contains the patch operation to apply.</param>
        private static void Apply(this Core2EnterpriseUser user, PatchOperation2 operation)
        {
            if (!string.IsNullOrWhiteSpace(operation.Path.AttributePath))
            {
                if (!string.IsNullOrWhiteSpace(operation.Path.SchemaIdentifier) && (operation?.Path?.SchemaIdentifier?.Equals(SchemaIdentifiers.Core2EnterpriseUser, StringComparison.OrdinalIgnoreCase) == true))
                {
                    user.PatchEnterpriseExtension(operation);
                }
                else
                {
                    OperationValue operationValue;

                    switch (operation?.Path?.AttributePath)
                    {
                        case AttributeNames.Active:
                            if (operation.Name != OperationName.Remove)
                            {
                                operationValue = operation.Value.SingleOrDefault();

                                if (operationValue != null && !string.IsNullOrWhiteSpace(operationValue.Value) && bool.TryParse(operationValue.Value, out bool active))
                                {
                                    user.Active = active;
                                }
                            }

                            break;

                        case AttributeNames.Addresses:
                            user.PatchAddresses(operation);
                            break;

                        case AttributeNames.DisplayName:
                            operationValue = operation.Value.SingleOrDefault();

                            if (operation.Name == OperationName.Remove)
                            {
                                if ((operationValue == null) ||
                                    string.Equals(operationValue.Value, user.DisplayName, StringComparison.OrdinalIgnoreCase))
                                {
                                    operationValue = null;
                                }
                                else
                                {
                                    break;
                                }
                            }

                            user.DisplayName = operationValue?.Value ?? null;

                            break;

                        case AttributeNames.ElectronicMailAddresses:
                            user.PatchElectronicMailAddresses(operation);
                            break;

                        case AttributeNames.ExternalIdentifier:
                            operationValue = operation.Value.SingleOrDefault();

                            if (operation.Name == OperationName.Remove)
                            {
                                if (operationValue == null ||
                                    string.Equals(operationValue.Value, user.ExternalIdentifier, StringComparison.OrdinalIgnoreCase))
                                {
                                    operationValue = null;
                                }
                                else
                                {
                                    break;
                                }
                            }

                            user.ExternalIdentifier = operationValue?.Value ?? null;

                            break;

                        case AttributeNames.Name:
                            user.PatchName(operation);
                            break;

                        case AttributeNames.PhoneNumbers:
                            user.PatchPhoneNumbers(operation);
                            break;

                        case AttributeNames.PreferredLanguage:
                            operationValue = operation.Value.SingleOrDefault();

                            if (operation.Name == OperationName.Remove)
                            {
                                if (operationValue == null ||
                                    string.Equals(user.PreferredLanguage, operationValue.Value, StringComparison.OrdinalIgnoreCase))
                                {
                                    operationValue = null;
                                }
                                else
                                {
                                    break;
                                }
                            }

                            user.PreferredLanguage = operationValue?.Value ?? null;

                            break;

                        case AttributeNames.Roles:
                            user.PatchRoles(operation);
                            break;

                        case AttributeNames.Title:
                            operationValue = operation.Value.SingleOrDefault();

                            if (operation.Name == OperationName.Remove)
                            {
                                if (operationValue == null ||
                                    string.Equals(user.Title, operationValue.Value, StringComparison.OrdinalIgnoreCase))
                                {
                                    operationValue = null;
                                }
                                else
                                {
                                    break;
                                }
                            }

                            user.Title = operationValue?.Value ?? null;

                            break;

                        case AttributeNames.UserName:
                            operationValue = operation.Value.SingleOrDefault();

                            if (operation.Name == OperationName.Remove)
                            {
                                if (operationValue == null ||
                                    string.Equals(user.UserName, operationValue.Value, StringComparison.OrdinalIgnoreCase))
                                {
                                    operationValue = null;
                                }
                                else
                                {
                                    break;
                                }
                            }

                            user.UserName = operationValue?.Value ?? null;

                            break;
                    }
                }
            }
        }

        /// <summary>
        /// This method is used to apply a patch to enterprise related user model properties using a specific patch operation model.
        /// </summary>
        /// <param name="user">Contains the user model to patch.</param>
        /// <param name="operation">Contains the operation to execute.</param>
        private static void PatchEnterpriseExtension(this Core2EnterpriseUser user, PatchOperation2 operation)
        {
            if (operation is { Path: not null } &&
                !string.IsNullOrWhiteSpace(operation.Path.AttributePath))
            {
                ExtensionAttributeEnterpriseUser2 extension = user.EnterpriseExtension;

                switch (operation.Path.AttributePath)
                {
                    case AttributeNames.CostCenter:
                        Core2EnterpriseUserExtensions.PatchCostCenter(extension, operation);

                        break;

                    case AttributeNames.Department:
                        Core2EnterpriseUserExtensions.PatchDepartment(extension, operation);

                        break;

                    case AttributeNames.Division:
                        Core2EnterpriseUserExtensions.PatchDivision(extension, operation);

                        break;

                    case AttributeNames.EmployeeNumber:
                        Core2EnterpriseUserExtensions.PatchEmployeeNumber(extension, operation);

                        break;

                    case AttributeNames.Manager:
                        Core2EnterpriseUserExtensions.PatchManager(extension, operation);

                        break;

                    case AttributeNames.Organization:
                        Core2EnterpriseUserExtensions.PatchOrganization(extension, operation);

                        break;
                }
            }
        }

        /// <summary>
        /// This method is used to patch address information within a user model with the specified patch operation data.
        /// </summary>
        /// <param name="user">Contains the user model to patch.</param>
        /// <param name="operation">Contains the operation to execute.</param>
        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "None")]
        private static void PatchAddresses(this Core2EnterpriseUser user, PatchOperation2 operation)
        {
            if (operation != null)
            {
                if (string.Equals(Talegen.AspNetCore.Scim.Schema.AttributeNames.Addresses, operation.Path.AttributePath, StringComparison.OrdinalIgnoreCase) &&
                    operation.Path.ValuePath != null &&
                    !string.IsNullOrWhiteSpace(operation.Path.ValuePath.AttributePath))
                {
                    IFilter subAttribute = operation.Path.SubAttributes.SingleOrDefault();

                    if (subAttribute != null &&
                        operation.Value.Count == 1 &&
                        operation.Name == OperationName.Remove &&
                        string.Equals(Talegen.AspNetCore.Scim.Schema.AttributeNames.Type, subAttribute.AttributePath, StringComparison.OrdinalIgnoreCase))
                    {
                        Address address;
                        Address addressExisting;

                        if (user.Addresses != null)
                        {
                            addressExisting =
                                address =
                                    user.Addresses.SingleOrDefault((Address item) => item.ItemType == subAttribute.ComparisonValue);
                        }
                        else
                        {
                            addressExisting = null;
                            address =
                                new Address
                                {
                                    ItemType = subAttribute.ComparisonValue
                                };
                        }

                        if (address != null)
                        {
                            string value;
                            switch (subAttribute.ComparisonValue)
                            {
                                case AddressBase.Work:
                                    {
                                        switch (operation.Path.ValuePath.AttributePath)
                                        {
                                            case AttributeNames.Country:
                                                {
                                                    value = operation.Value.Single().Value;
                                                    if (OperationName.Remove == operation.Name &&
                                                        string.Equals(value, address.Country, StringComparison.OrdinalIgnoreCase))
                                                    {
                                                        value = null;
                                                    }

                                                    address.Country = value;
                                                    break;
                                                }
                                            case AttributeNames.Locality:
                                                {
                                                    value = operation.Value.Single().Value;

                                                    if (operation.Name == OperationName.Remove &&
                                                        string.Equals(value, address.Locality, StringComparison.OrdinalIgnoreCase))
                                                    {
                                                        value = null;
                                                    }

                                                    address.Locality = value;
                                                    break;
                                                }
                                            case AttributeNames.PostalCode:
                                                {
                                                    value = operation.Value.Single().Value;

                                                    if
                                                        (operation.Name == OperationName.Remove &&
                                                         string.Equals(value, address.PostalCode, StringComparison.OrdinalIgnoreCase))
                                                    {
                                                        value = null;
                                                    }

                                                    address.PostalCode = value;
                                                    break;
                                                }
                                            case AttributeNames.Region:
                                                {
                                                    value = operation.Value.Single().Value;

                                                    if (operation.Name == OperationName.Remove &&
                                                        string.Equals(value, address.Region, StringComparison.OrdinalIgnoreCase))
                                                    {
                                                        value = null;
                                                    }

                                                    address.Region = value;
                                                    break;
                                                }
                                            case AttributeNames.StreetAddress:
                                                {
                                                    value = operation.Value.Single().Value;

                                                    if
                                                        (operation.Name == OperationName.Remove &&
                                                         string.Equals(value, address.StreetAddress, StringComparison.OrdinalIgnoreCase))
                                                    {
                                                        value = null;
                                                    }

                                                    address.StreetAddress = value;
                                                    break;
                                                }
                                        }

                                        break;
                                    }
                                case AddressBase.Other:
                                    {
                                        if (operation.Path.ValuePath.AttributePath == AttributeNames.Formatted)
                                        {
                                            value = operation.Value.Single().Value;

                                            if
                                                (operation.Name == OperationName.Remove &&
                                                 string.Equals(value, address.Formatted, StringComparison.OrdinalIgnoreCase))
                                            {
                                                value = null;
                                            }

                                            address.Formatted = value;
                                        }

                                        break;
                                    }
                            }

                            if (string.IsNullOrWhiteSpace(address.Country) &&
                                string.IsNullOrWhiteSpace(address.Locality) &&
                                string.IsNullOrWhiteSpace(address.PostalCode) &&
                                string.IsNullOrWhiteSpace(address.Region) &&
                                string.IsNullOrWhiteSpace(address.StreetAddress) &&
                                string.IsNullOrWhiteSpace(address.Formatted))
                            {
                                if (addressExisting != null && user.Addresses != null)
                                {
                                    user.Addresses =
                                        user.Addresses.Where(item => item.ItemType != subAttribute.ComparisonValue).ToArray();
                                }
                            }
                            else if (addressExisting == null)
                            {
                                IEnumerable<Address> addresses = new Address[]
                                {
                                    address
                                };

                                user.Addresses = user.Addresses?.Union(addresses).ToArray() ?? addresses;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// This method is used to patch cost center information within a user model with the specified patch operation data.
        /// </summary>
        /// <param name="extension">Contains the user extension model to patch.</param>
        /// <param name="operation">Contains the operation to execute.</param>
        private static void PatchCostCenter(ExtensionAttributeEnterpriseUser2 extension, PatchOperation2 operation)
        {
            OperationValue value = operation.Value.SingleOrDefault();

            if (operation.Name == OperationName.Remove &&
                (value == null || string.Equals(value.Value, extension.CostCenter, StringComparison.OrdinalIgnoreCase)))
            {
                value = null;
            }

            extension.CostCenter = value?.Value ?? null;
        }

        /// <summary>
        /// This method is used to patch department information within a user model with the specified patch operation data.
        /// </summary>
        /// <param name="extension">Contains the user extension model to patch.</param>
        /// <param name="operation">Contains the operation to execute.</param>
        private static void PatchDepartment(ExtensionAttributeEnterpriseUser2 extension, PatchOperation2 operation)
        {
            OperationValue value = operation.Value.SingleOrDefault();

            if (operation.Name == OperationName.Remove &&
                (value == null || string.Equals(value.Value, extension.Department, StringComparison.OrdinalIgnoreCase)))
            {
                value = null;
            }

            extension.Department = value?.Value ?? null;
        }

        /// <summary>
        /// This method is used to patch division information within a user model with the specified patch operation data.
        /// </summary>
        /// <param name="extension">Contains the user extension model to patch.</param>
        /// <param name="operation">Contains the operation to execute.</param>
        private static void PatchDivision(ExtensionAttributeEnterpriseUser2 extension, PatchOperation2 operation)
        {
            OperationValue value = operation.Value.SingleOrDefault();

            if (operation.Name == OperationName.Remove &&
                (value == null || string.Equals(extension.Division, value.Value, StringComparison.OrdinalIgnoreCase)))
            {
                value = null;
            }

            extension.Division = value?.Value ?? null;
        }

        /// <summary>
        /// This method is used to patch email addresses within a user model with the specified patch operation data.
        /// </summary>
        /// <param name="user">Contains the user model to patch.</param>
        /// <param name="operation">Contains the patch operation model data.</param>
        private static void PatchElectronicMailAddresses(this Core2EnterpriseUser user, PatchOperation2 operation)
        {
            user.ElectronicMailAddresses = ProtocolExtensions.PatchElectronicMailAddresses(user.ElectronicMailAddresses, operation);
        }

        /// <summary>
        /// This method is used to patch employee number information within a user model with the specified patch operation data.
        /// </summary>
        /// <param name="extension">Contains the user extension model to patch.</param>
        /// <param name="operation">Contains the operation to execute.</param>
        private static void PatchEmployeeNumber(ExtensionAttributeEnterpriseUser2 extension, PatchOperation2 operation)
        {
            OperationValue value = operation.Value.SingleOrDefault();

            if (operation.Name == OperationName.Remove &&
                (value == null || string.Equals(extension.EmployeeNumber, value.Value, StringComparison.OrdinalIgnoreCase)))
            {
                value = null;
            }

            extension.EmployeeNumber = value?.Value ?? null;
        }

        /// <summary>
        /// This method is used to patch employee manager information within a user model with the specified patch operation data.
        /// </summary>
        /// <param name="extension">Contains the user extension model to patch.</param>
        /// <param name="operation">Contains the operation to execute.</param>
        private static void PatchManager(ExtensionAttributeEnterpriseUser2 extension, PatchOperation2 operation)
        {
            OperationValue value = operation.Value.SingleOrDefault();

            if (operation.Name == OperationName.Remove &&
                value == null || extension.Manager == null || string.Equals(extension.Manager.Value, value.Value, StringComparison.OrdinalIgnoreCase))
            {
                value = null;
            }

            extension.Manager = value != null ? new Manager
            {
                Value = value.Value
            } : null;
        }

        private static void PatchName(this Core2EnterpriseUser user, PatchOperation2 operation)
        {
            if (operation?.Path != null &&
                string.Equals(operation.Path.AttributePath, AttributeNames.Name, StringComparison.OrdinalIgnoreCase) &&
                !string.IsNullOrWhiteSpace(operation.Path.ValuePath.AttributePath) &&
                (operation.Value.Count == 1 || operation is { Value: not null, Name: OperationName.Remove }))
            {
                Name nameExisting;
                Name name = nameExisting = user.Name ?? new Name();
                string value;

                if (string.Equals(operation.Path.ValuePath.AttributePath, AttributeNames.GivenName, StringComparison.OrdinalIgnoreCase))
                {
                    value = operation.Value?.Single().Value;

                    if (value != null && operation.Name == OperationName.Remove && string.Equals(value, name.GivenName, StringComparison.OrdinalIgnoreCase))
                    {
                        value = null;
                    }

                    name.GivenName = value;
                }

                if (string.Equals(operation.Path.ValuePath.AttributePath, AttributeNames.FamilyName, StringComparison.OrdinalIgnoreCase))
                {
                    value = operation.Value?.Single().Value;

                    if (value != null && operation.Name == OperationName.Remove && string.Equals(value, name.FamilyName, StringComparison.OrdinalIgnoreCase))
                    {
                        value = null;
                    }

                    name.FamilyName = value;
                }

                if (string.IsNullOrWhiteSpace(name.FamilyName) && string.IsNullOrWhiteSpace(name.GivenName))
                {
                    user.Name = null;
                }
            }
        }

        /// <summary>
        /// This method is used to patch user organization information within a user model with the specified patch operation data.
        /// </summary>
        /// <param name="extension">Contains the user extension model to patch.</param>
        /// <param name="operation">Contains the operation to execute.</param>
        private static void PatchOrganization(ExtensionAttributeEnterpriseUser2 extension, PatchOperation2 operation)
        {
            OperationValue value = operation.Value.SingleOrDefault();

            if (operation.Name == OperationName.Remove)
            {
                if (value == null || string.Equals(extension.Organization, value.Value, StringComparison.OrdinalIgnoreCase))
                {
                    value = null;
                }
            }

            extension.Organization = value?.Value ?? null;
        }

        /// <summary>
        /// This method is used to patch user phone information within a user model with the specified patch operation data.
        /// </summary>
        /// <param name="user">Contains the user model to patch.</param>
        /// <param name="operation">Contains the operation to execute.</param>
        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "None")]
        private static void PatchPhoneNumbers(this Core2EnterpriseUser user, PatchOperation2 operation)
        {
            if (string.Equals(operation.Path.AttributePath, AttributeNames.PhoneNumbers, StringComparison.OrdinalIgnoreCase) &&
                !string.IsNullOrWhiteSpace(operation.Path.ValuePath.AttributePath))
            {
                IFilter subAttribute = operation.Path.SubAttributes.SingleOrDefault();

                if (subAttribute != null &&
                    (operation.Value.Count == 1 || operation.Name == OperationName.Remove) &&
                    string.Equals(subAttribute.AttributePath, AttributeNames.Type, StringComparison.OrdinalIgnoreCase))
                {
                    string phoneNumberType = subAttribute.ComparisonValue;

                    if (string.Equals(PhoneNumberBase.Fax, phoneNumberType, StringComparison.Ordinal) ||
                        string.Equals(PhoneNumberBase.Mobile, phoneNumberType, StringComparison.Ordinal) ||
                        string.Equals(PhoneNumberBase.Work, phoneNumberType, StringComparison.Ordinal))
                    {
                        PhoneNumber phoneNumber;
                        PhoneNumber phoneNumberExisting =
                            phoneNumber = user.PhoneNumbers.SingleOrDefault(item => item.ItemType == subAttribute.ComparisonValue);
                        string value = operation.Value?.Single().Value;

                        if (value != null &&
                            operation.Name == OperationName.Remove &&
                            string.Equals(value, phoneNumber.Value, StringComparison.OrdinalIgnoreCase))
                        {
                            value = null;
                        }

                        phoneNumber.Value = value;

                        if (string.IsNullOrWhiteSpace(phoneNumber.Value))
                        {
                            if (phoneNumberExisting != null)
                            {
                                user.PhoneNumbers = user.PhoneNumbers.Where(item => item.ItemType != subAttribute.ComparisonValue).ToArray();
                            }
                        }
                        else if (phoneNumberExisting == null)
                        {
                            IEnumerable<PhoneNumber> phoneNumbers =
                                new PhoneNumber[]
                                {
                                        phoneNumber
                                };

                            user.PhoneNumbers = user.PhoneNumbers != null ? user.PhoneNumbers.Union(phoneNumbers).ToArray() : phoneNumbers;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// This method is used to patch user role information within a user model with the specified patch operation data.
        /// </summary>
        /// <param name="user">Contains the user model to patch.</param>
        /// <param name="operation">Contains the operation to execute.</param>
        private static void PatchRoles(this Core2EnterpriseUser user, PatchOperation2 operation)
        {
            user.Roles = ProtocolExtensions.PatchRoles(user.Roles, operation);
        }
    }
}