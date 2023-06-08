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
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;

    /// <summary>
    /// Class ServiceConfigurationBase. Implements the <see cref="Talegen.AspNetCore.Scim.Schema.Schematized" />
    /// </summary>
    /// <seealso cref="Talegen.AspNetCore.Scim.Schema.Schematized" />
    [DataContract]
    public abstract class ServiceConfigurationBase : Schematized
    {
        /// <summary>
        /// The authentication schemes
        /// </summary>
        [DataMember(Name = AttributeNames.AuthenticationSchemes)]
        private List<AuthenticationScheme> authenticationSchemes;

        /// <summary>
        /// The authentication schemes wrapper
        /// </summary>
        private IReadOnlyCollection<AuthenticationScheme> authenticationSchemesWrapper;

        /// <summary>
        /// The this lock
        /// </summary>
        private object thisLock;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceConfigurationBase" /> class.
        /// </summary>
        protected ServiceConfigurationBase()
        {
            this.OnInitialization();
            this.OnInitialized();
        }

        /// <summary>
        /// Gets the authentication schemes.
        /// </summary>
        /// <value>The authentication schemes.</value>
        public IReadOnlyCollection<AuthenticationScheme> AuthenticationSchemes
        {
            get
            {
                return this.authenticationSchemesWrapper;
            }
        }

        /// <summary>
        /// Gets or sets the bulk requests.
        /// </summary>
        /// <value>The bulk requests.</value>
        [DataMember(Name = AttributeNames.Bulk)]
        public BulkRequestsFeature BulkRequests
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the documentation resource.
        /// </summary>
        /// <value>The documentation resource.</value>
        [DataMember(Name = AttributeNames.Documentation)]
        public Uri DocumentationResource
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the entity tags.
        /// </summary>
        /// <value>The entity tags.</value>
        [DataMember(Name = AttributeNames.EntityTag)]
        public Feature EntityTags
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the filtering.
        /// </summary>
        /// <value>The filtering.</value>
        [DataMember(Name = AttributeNames.Filter)]
        public Feature Filtering
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the password change.
        /// </summary>
        /// <value>The password change.</value>
        [DataMember(Name = AttributeNames.ChangePassword)]
        public Feature PasswordChange
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the patching.
        /// </summary>
        /// <value>The patching.</value>
        [DataMember(Name = AttributeNames.Patch)]
        public Feature Patching
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the sorting.
        /// </summary>
        /// <value>The sorting.</value>
        [DataMember(Name = AttributeNames.Sort)]
        public Feature Sorting
        {
            get;
            set;
        }

        /// <summary>
        /// Adds the authentication scheme.
        /// </summary>
        /// <param name="authenticationScheme">The authentication scheme.</param>
        /// <exception cref="System.ArgumentNullException">authenticationScheme</exception>
        /// <exception cref="System.ArgumentException"></exception>
        public void AddAuthenticationScheme(AuthenticationScheme authenticationScheme)
        {
            if (null == authenticationScheme)
            {
                throw new ArgumentNullException(nameof(authenticationScheme));
            }

            if (string.IsNullOrWhiteSpace(authenticationScheme.Name))
            {
                throw new ArgumentException(
                    Properties.Resources.ExceptionUnnamedAuthenticationScheme);
            }

            Func<bool> containsFunction =
                new Func<bool>(
                        () =>
                            this
                            .authenticationSchemes
                            .Any(
                                (AuthenticationScheme item) =>
                                    string.Equals(item.Name, authenticationScheme.Name, StringComparison.OrdinalIgnoreCase)));

            if (!containsFunction())
            {
                lock (this.thisLock)
                {
                    if (!containsFunction())
                    {
                        this.authenticationSchemes.Add(authenticationScheme);
                    }
                }
            }
        }

        /// <summary>
        /// This event method is fired when deserialization.
        /// </summary>
        /// <param name="context">Contains the stream context.</param>
        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            this.OnInitialized();
        }

        /// <summary>
        /// This event method is fired on deserializing.
        /// </summary>
        /// <param name="context">Contains the stream context.</param>
        [OnDeserializing]
        private void OnDeserializing(StreamingContext context)
        {
            this.OnInitialization();
        }

        /// <summary>
        /// This event method is called on initialization.
        /// </summary>
        private void OnInitialization()
        {
            this.thisLock = new object();
            this.authenticationSchemes = new List<AuthenticationScheme>();
        }

        /// <summary>
        /// This event method is called after initialization.
        /// </summary>
        private void OnInitialized()
        {
            this.authenticationSchemesWrapper = this.authenticationSchemes.AsReadOnly();
        }
    }
}