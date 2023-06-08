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
    using System.Collections.ObjectModel;
    using System.Runtime.Serialization;

    /// <summary>
    /// Class Core2UserBase. Implements the <see cref="Talegen.AspNetCore.Scim.Schema.UserBase" />
    /// </summary>
    /// <seealso cref="Talegen.AspNetCore.Scim.Schema.UserBase" />
    [DataContract]
    public abstract class Core2UserBase : UserBase
    {
        /// <summary>
        /// The custom extension
        /// </summary>
        private IDictionary<string, IDictionary<string, object>> customExtension;

        /// <summary>
        /// Initializes a new instance of the <see cref="Core2UserBase" /> class.
        /// </summary>
        protected Core2UserBase()
        {
            this.AddSchema(SchemaIdentifiers.Core2User);
            this.Metadata = new Core2Metadata
            {
                ResourceType = Types.User
            };
            this.OnInitialization();
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Core2UserBase" /> is active.
        /// </summary>
        /// <value><c>true</c> if active; otherwise, <c>false</c>.</value>
        [DataMember(Name = AttributeNames.Active)]
        public virtual bool Active
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the addresses.
        /// </summary>
        /// <value>The addresses.</value>
        [DataMember(Name = AttributeNames.Addresses, IsRequired = false, EmitDefaultValue = false)]
        public virtual IEnumerable<Address> Addresses
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the custom extension.
        /// </summary>
        /// <value>The custom extension.</value>
        public virtual IReadOnlyDictionary<string, IDictionary<string, object>> CustomExtension => new ReadOnlyDictionary<string, IDictionary<string, object>>(this.customExtension);

        /// <summary>
        /// Gets or sets the display name.
        /// </summary>
        /// <value>The display name.</value>
        [DataMember(Name = AttributeNames.DisplayName, IsRequired = false, EmitDefaultValue = false)]
        public virtual string DisplayName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the electronic mail addresses.
        /// </summary>
        /// <value>The electronic mail addresses.</value>
        [DataMember(Name = AttributeNames.ElectronicMailAddresses, IsRequired = false, EmitDefaultValue = false)]
        public virtual IEnumerable<ElectronicMailAddress> ElectronicMailAddresses
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the instant messagings.
        /// </summary>
        /// <value>The instant messagings.</value>
        [DataMember(Name = AttributeNames.Ims, IsRequired = false, EmitDefaultValue = false)]
        public virtual IEnumerable<InstantMessaging> InstantMessagings
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the locale.
        /// </summary>
        /// <value>The locale.</value>
        [DataMember(Name = AttributeNames.Locale, IsRequired = false, EmitDefaultValue = false)]
        public virtual string Locale
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the metadata.
        /// </summary>
        /// <value>The metadata.</value>
        [DataMember(Name = AttributeNames.Metadata)]
        public virtual Core2Metadata Metadata
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        [DataMember(Name = AttributeNames.Name, IsRequired = false, EmitDefaultValue = false)]
        public virtual Name Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the nickname.
        /// </summary>
        /// <value>The nickname.</value>
        [DataMember(Name = AttributeNames.Nickname, IsRequired = false, EmitDefaultValue = false)]
        public virtual string Nickname
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the phone numbers.
        /// </summary>
        /// <value>The phone numbers.</value>
        [DataMember(Name = AttributeNames.PhoneNumbers, IsRequired = false, EmitDefaultValue = false)]
        public virtual IEnumerable<PhoneNumber> PhoneNumbers
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the preferred language.
        /// </summary>
        /// <value>The preferred language.</value>
        [DataMember(Name = AttributeNames.PreferredLanguage, IsRequired = false, EmitDefaultValue = false)]
        public virtual string PreferredLanguage
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the roles.
        /// </summary>
        /// <value>The roles.</value>
        [DataMember(Name = AttributeNames.Roles, IsRequired = false, EmitDefaultValue = false)]
        public virtual IEnumerable<Role> Roles
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the time zone.
        /// </summary>
        /// <value>The time zone.</value>
        [DataMember(Name = AttributeNames.TimeZone, IsRequired = false, EmitDefaultValue = false)]
        public virtual string TimeZone
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        /// <value>The title.</value>
        [DataMember(Name = AttributeNames.Title, IsRequired = false, EmitDefaultValue = false)]
        public virtual string Title
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the type of the user.
        /// </summary>
        /// <value>The type of the user.</value>
        [DataMember(Name = AttributeNames.UserType, IsRequired = false, EmitDefaultValue = false)]
        public virtual string UserType
        {
            get;
            set;
        }

        /// <summary>
        /// Adds the custom attribute.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public virtual void AddCustomAttribute(string key, object value)
        {
            if (key.StartsWith(SchemaIdentifiers.PrefixExtension, StringComparison.OrdinalIgnoreCase) &&
                !key.StartsWith(SchemaIdentifiers.Core2EnterpriseUser, StringComparison.OrdinalIgnoreCase) &&
                value is Dictionary<string, object> nestedObject)
            {
                this.customExtension.Add(key, nestedObject);
            }
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
            this.customExtension = new Dictionary<string, IDictionary<string, object>>();
        }

        /// <summary>
        /// Converts to json.
        /// </summary>
        /// <returns>Dictionary&lt;System.String, System.Object&gt;.</returns>
        public override Dictionary<string, object> ToJson()
        {
            Dictionary<string, object> result = base.ToJson();

            foreach (KeyValuePair<string, IDictionary<string, object>> entry in this.CustomExtension)
            {
                result.Add(entry.Key, entry.Value);
            }

            return result;
        }
    }
}