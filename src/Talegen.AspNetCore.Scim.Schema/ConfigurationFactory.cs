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

    /// <summary>
    /// Class ConfigurationFactory.
    /// </summary>
    /// <typeparam name="TConfiguration">The type of the t configuration.</typeparam>
    /// <typeparam name="TException">The type of the t exception.</typeparam>
    public abstract class ConfigurationFactory<TConfiguration, TException> where TException : Exception
    {
        /// <summary>
        /// Creates the specified default configuration.
        /// </summary>
        /// <param name="defaultConfiguration">The default configuration.</param>
        /// <param name="configurationError">The configuration error.</param>
        /// <returns>TConfiguration.</returns>
        public abstract TConfiguration Create(Lazy<TConfiguration> defaultConfiguration, out TException configurationError);
    }
}