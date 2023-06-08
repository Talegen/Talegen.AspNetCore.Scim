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
    using System.Configuration;

    /// <summary>
    /// Class SystemForCrossDomainIdentityManagementConfigurationSection. Implements the <see cref="ConfigurationSection" /> Implements the <see cref="Talegen.AspNetCore.Scim.Schema.ISystemForCrossDomainIdentityManagementConfiguration" />
    /// </summary>
    /// <seealso cref="ConfigurationSection" />
    /// <seealso cref="Talegen.AspNetCore.Scim.Schema.ISystemForCrossDomainIdentityManagementConfiguration" />
    internal class SystemForCrossDomainIdentityManagementConfigurationSection : ConfigurationSection, ISystemForCrossDomainIdentityManagementConfiguration
    {
        /// <summary>
        /// The default value accept large objects
        /// </summary>
        private const string DefaultValueAcceptLargeObjects = "false";

        /// <summary>
        /// The name section
        /// </summary>
        private const string NameSection = "scimProtocol";

        /// <summary>
        /// The property name accept large objects
        /// </summary>
        private const string PropertyNameAcceptLargeObjects = "acceptLargeObjects";

        /// <summary>
        /// The default accept large objects
        /// </summary>
        private static readonly bool DefaultAcceptLargeObjects =
            bool.Parse(DefaultValueAcceptLargeObjects);

        /// <summary>
        /// The default configuration
        /// </summary>
        private static readonly Lazy<SystemForCrossDomainIdentityManagementConfigurationSection> DefaultConfiguration =
            new Lazy<SystemForCrossDomainIdentityManagementConfigurationSection>(
                () =>
                    new SystemForCrossDomainIdentityManagementConfigurationSection());

        /// <summary>
        /// The factory
        /// </summary>
        private static readonly Lazy<ConfigurationFactory<SystemForCrossDomainIdentityManagementConfigurationSection, ConfigurationErrorsException>> Factory =
            new Lazy<ConfigurationFactory<SystemForCrossDomainIdentityManagementConfigurationSection, ConfigurationErrorsException>>(
                () =>
                    new ConfigurationSectionFactory<SystemForCrossDomainIdentityManagementConfigurationSection>(
                        NameSection));

        /// <summary>
        /// The singleton
        /// </summary>
        private static readonly Lazy<ISystemForCrossDomainIdentityManagementConfiguration> Singleton =
            new Lazy<ISystemForCrossDomainIdentityManagementConfiguration>(
                () =>
                    Initialize());

        /// <summary>
        /// Prevents a default instance of the <see cref="SystemForCrossDomainIdentityManagementConfigurationSection" /> class from being created.
        /// </summary>
        private SystemForCrossDomainIdentityManagementConfigurationSection()
        {
        }

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>The instance.</value>
        public static ISystemForCrossDomainIdentityManagementConfiguration Instance
        {
            get
            {
                return Singleton.Value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether [accept large objects].
        /// </summary>
        /// <value><c>true</c> if [accept large objects]; otherwise, <c>false</c>.</value>
        public bool AcceptLargeObjects
        {
            get
            {
                if (!bool.TryParse(this.AcceptLargeObjectsValue, out bool result))
                {
                    result = DefaultAcceptLargeObjects;
                }

                return result;
            }
        }

        /// <summary>
        /// Gets the accept large objects value.
        /// </summary>
        /// <value>The accept large objects value.</value>
        [ConfigurationProperty(
            PropertyNameAcceptLargeObjects,
            DefaultValue = DefaultValueAcceptLargeObjects,
            IsRequired = false)]
        private string AcceptLargeObjectsValue
        {
            get
            {
                return (string)base[PropertyNameAcceptLargeObjects];
            }
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        /// <returns>ISystemForCrossDomainIdentityManagementConfiguration.</returns>
        private static ISystemForCrossDomainIdentityManagementConfiguration Initialize()
        {
            ISystemForCrossDomainIdentityManagementConfiguration result =
                Factory.Value.Create(
                    DefaultConfiguration,
                    out ConfigurationErrorsException errors);
            return result;
        }
    }
}