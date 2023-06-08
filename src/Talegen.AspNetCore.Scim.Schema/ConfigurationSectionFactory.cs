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
    /// Class ConfigurationSectionFactory. Implements the <see cref="Talegen.AspNetCore.Scim.Schema.ConfigurationFactory{TConfiguration, System.Configuration.ConfigurationErrorsException}" />
    /// </summary>
    /// <typeparam name="TConfiguration">The type of the t configuration.</typeparam>
    /// <seealso cref="Talegen.AspNetCore.Scim.Schema.ConfigurationFactory{TConfiguration, System.Configuration.ConfigurationErrorsException}" />
    public class ConfigurationSectionFactory<TConfiguration> : ConfigurationFactory<TConfiguration, ConfigurationErrorsException> where TConfiguration : ConfigurationSection
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationSectionFactory{TConfiguration}" /> class.
        /// </summary>
        /// <param name="sectionName">Name of the section.</param>
        /// <exception cref="System.ArgumentNullException">sectionName</exception>
        public ConfigurationSectionFactory(string sectionName)
        {
            if (string.IsNullOrEmpty(sectionName))
            {
                throw new ArgumentNullException(nameof(sectionName));
            }

            this.SectionName = sectionName;
        }

        /// <summary>
        /// Gets or sets the name of the section.
        /// </summary>
        /// <value>The name of the section.</value>
        private string SectionName { get; }

        /// <summary>
        /// Creates the specified default configuration.
        /// </summary>
        /// <param name="defaultConfiguration">The default configuration.</param>
        /// <param name="errors">The errors.</param>
        /// <returns>TConfiguration.</returns>
        /// <exception cref="System.ArgumentNullException">defaultConfiguration</exception>
        public override TConfiguration Create(Lazy<TConfiguration> defaultConfiguration, out ConfigurationErrorsException errors)
        {
            errors = null;

            if (defaultConfiguration == null)
            {
                throw new ArgumentNullException(nameof(defaultConfiguration));
            }

            TConfiguration result = null;

            try
            {
                result = (TConfiguration)ConfigurationManager.GetSection(this.SectionName);
            }
            catch (ConfigurationErrorsException exception)
            {
                errors = exception;
            }

            return result ??= defaultConfiguration.Value;
        }
    }
}