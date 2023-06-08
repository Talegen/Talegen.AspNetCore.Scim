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
    using System.Runtime.Serialization;
    using System.Linq;

    /// <summary>
    /// This class represents an attribute scheme.
    /// </summary>
    [DataContract]
    public sealed class AttributeScheme
    {
        /// <summary>
        /// The data type
        /// </summary>
        private AttributeDataType dataType;

        /// <summary>
        /// The data type value
        /// </summary>
        private string dataTypeValue;

        /// <summary>
        /// The mutability
        /// </summary>
        private Mutability mutability;

        /// <summary>
        /// The mutability value
        /// </summary>
        private string mutabilityValue;

        /// <summary>
        /// The returned
        /// </summary>
        private Returned returned;

        /// <summary>
        /// The returned value
        /// </summary>
        private string returnedValue;

        /// <summary>
        /// The uniqueness
        /// </summary>
        private Uniqueness uniqueness;

        /// <summary>
        /// The uniqueness value
        /// </summary>
        private string uniquenessValue;

        /// <summary>
        /// The sub attributes
        /// </summary>
        private List<AttributeScheme> subAttributes;

        /// <summary>
        /// The sub attributes wrapper
        /// </summary>
        private IReadOnlyCollection<AttributeScheme> subAttributesWrapper;

        /// <summary>
        /// The canonical values
        /// </summary>
        private List<string> canonicalValues;

        /// <summary>
        /// The canonical values wrapper
        /// </summary>
        private IReadOnlyCollection<string> canonicalValuesWrapper;

        /// <summary>
        /// The reference types
        /// </summary>
        private List<string> referenceTypes;

        /// <summary>
        /// The reference types wrapper
        /// </summary>
        private IReadOnlyCollection<string> referenceTypesWrapper;

        /// <summary>
        /// The this lock
        /// </summary>
        private object thisLock;

        /// <summary>
        /// Initializes a new instance of the <see cref="AttributeScheme" /> class.
        /// </summary>
        public AttributeScheme()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AttributeScheme" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="type">The type.</param>
        /// <param name="plural">if set to <c>true</c> [plural].</param>
        /// <exception cref="System.ArgumentNullException">name</exception>
        public AttributeScheme(string name, AttributeDataType type, bool plural)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            this.OnInitialization();
            this.OnInitialized();
            this.Name = name;
            this.DataType = type;
            this.Plural = plural;
            this.Mutability = Mutability.readWrite;
            this.Returned = Returned.@default;
            this.Uniqueness = Uniqueness.none;
        }

        /// <summary>
        /// Gets or sets a value indicating whether [case exact].
        /// </summary>
        /// <value><c>true</c> if [case exact]; otherwise, <c>false</c>.</value>
        [DataMember(Name = AttributeNames.CaseExact)]
        public bool CaseExact { get; set; }

        /// <summary>
        /// Gets or sets the type of the data.
        /// </summary>
        /// <value>The type of the data.</value>
        public AttributeDataType DataType
        {
            get => this.dataType;

            set
            {
                this.dataTypeValue = Enum.GetName(typeof(AttributeDataType), value);
                this.dataType = value;
            }
        }

        /// <summary>
        /// Gets or sets the data type value.
        /// </summary>
        /// <value>The data type value.</value>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Called upon serialization")]
        [DataMember(Name = AttributeNames.Type)]
        private string DataTypeValue
#pragma warning restore IDE0051 // Remove unused private members
        {
            get => this.dataTypeValue;

            set
            {
                this.dataType = (AttributeDataType)Enum.Parse(typeof(AttributeDataType), value);
                this.dataTypeValue = value;
            }
        }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        [DataMember(Name = AttributeNames.Description)]
        public string Description
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the mutability.
        /// </summary>
        /// <value>The mutability.</value>
        public Mutability Mutability
        {
            get => this.mutability;

            set
            {
                this.mutabilityValue = Enum.GetName(typeof(Mutability), value);
                this.mutability = value;
            }
        }

        /// <summary>
        /// Gets or sets the mutability value.
        /// </summary>
        /// <value>The mutability value.</value>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Called upon serialization")]
        [DataMember(Name = AttributeNames.Mutability)]
        private string MutabilityValue
#pragma warning restore IDE0051 // Remove unused private members
        {
            get
            {
                return this.mutabilityValue;
            }

            set
            {
                this.mutability = (Mutability)Enum.Parse(typeof(Mutability), value);
                this.mutabilityValue = value;
            }
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        [DataMember(Name = AttributeNames.Name)]
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="AttributeScheme" /> is plural.
        /// </summary>
        /// <value><c>true</c> if plural; otherwise, <c>false</c>.</value>
        [DataMember(Name = AttributeNames.Plural)]
        public bool Plural
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="AttributeScheme" /> is required.
        /// </summary>
        /// <value><c>true</c> if required; otherwise, <c>false</c>.</value>
        [DataMember(Name = AttributeNames.Required)]
        public bool Required
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the returned.
        /// </summary>
        /// <value>The returned.</value>
        public Returned Returned
        {
            get => this.returned;

            set
            {
                this.returnedValue = Enum.GetName(typeof(Returned), value);
                this.returned = value;
            }
        }

        /// <summary>
        /// Gets or sets the returned value.
        /// </summary>
        /// <value>The returned value.</value>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Called upon serialization")]
        [DataMember(Name = AttributeNames.Returned)]
        private string ReturnedValue
#pragma warning restore IDE0051 // Remove unused private members
        {
            get
            {
                return this.returnedValue;
            }

            set
            {
                this.returned = (Returned)Enum.Parse(typeof(Returned), value);
                this.returnedValue = value;
            }
        }

        /// <summary>
        /// Gets or sets the uniqueness.
        /// </summary>
        /// <value>The uniqueness.</value>
        public Uniqueness Uniqueness
        {
            get => this.uniqueness;

            set
            {
                this.uniquenessValue = Enum.GetName(typeof(Uniqueness), value);
                this.uniqueness = value;
            }
        }

        /// <summary>
        /// Gets or sets the uniqueness value.
        /// </summary>
        /// <value>The uniqueness value.</value>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Called upon serialization")]
        [DataMember(Name = AttributeNames.Uniqueness)]
        private string UniquenessValue
#pragma warning restore IDE0051 // Remove unused private members
        {
            get
            {
                return this.uniquenessValue;
            }

            set
            {
                this.uniqueness = (Uniqueness)Enum.Parse(typeof(Uniqueness), value);
                this.uniquenessValue = value;
            }
        }

        /// <summary>
        /// Gets the sub attributes.
        /// </summary>
        /// <value>The sub attributes.</value>
        [DataMember(Name = AttributeNames.SubAttributes)]
        public IReadOnlyCollection<AttributeScheme> SubAttributes => this.subAttributesWrapper.Count == 0 ? null : this.subAttributesWrapper;

        /// <summary>
        /// Gets the canonical values.
        /// </summary>
        /// <value>The canonical values.</value>
        [DataMember(Name = AttributeNames.CanonicalValues)]
        public IReadOnlyCollection<string> CanonicalValues => this.canonicalValuesWrapper.Count == 0 ? null : this.canonicalValuesWrapper;

        /// <summary>
        /// Gets the reference types.
        /// </summary>
        /// <value>The reference types.</value>
        [DataMember(Name = AttributeNames.ReferenceTypes)]
        public IReadOnlyCollection<string> ReferenceTypes => this.referenceTypesWrapper.Count == 0 ? null : this.referenceTypesWrapper;

        /// <summary>
        /// Adds the sub attribute.
        /// </summary>
        /// <param name="subAttribute">The sub attribute.</param>
        public void AddSubAttribute(AttributeScheme subAttribute)
        {
            var containsFunction = new Func<bool>(() => this.subAttributes.Any(item => string.Equals(item.Name, subAttribute.Name, StringComparison.OrdinalIgnoreCase)));
            this.AddItemFunction(subAttribute, this.subAttributes, containsFunction);
        }

        /// <summary>
        /// Adds the canonical values.
        /// </summary>
        /// <param name="canonicalValue">The canonical value.</param>
        public void AddCanonicalValues(string canonicalValue)
        {
            var containsFunction = new Func<bool>(() => this.canonicalValues.Any(item => string.Equals(item, canonicalValue, StringComparison.OrdinalIgnoreCase)));
            this.AddItemFunction(canonicalValue, this.canonicalValues, containsFunction);
        }

        /// <summary>
        /// Adds the reference types.
        /// </summary>
        /// <param name="referenceType">Type of the reference.</param>
        public void AddReferenceTypes(string referenceType)
        {
            var containsFunction = new Func<bool>(() => this.referenceTypes.Any(item => string.Equals(item, referenceType, StringComparison.OrdinalIgnoreCase)));
            this.AddItemFunction(referenceType, this.referenceTypes, containsFunction);
        }

        /// <summary>
        /// Called when [initialization].
        /// </summary>
        private void OnInitialization()
        {
            this.thisLock = new object();
            this.subAttributes = new List<AttributeScheme>();
            this.canonicalValues = new List<string>();
            this.referenceTypes = new List<string>();
        }

        /// <summary>
        /// Called when [initialized].
        /// </summary>
        private void OnInitialized()
        {
            this.subAttributesWrapper = this.subAttributes.AsReadOnly();
            this.canonicalValuesWrapper = this.canonicalValues.AsReadOnly();
            this.referenceTypesWrapper = this.referenceTypes.AsReadOnly();
        }

        /// <summary>
        /// Adds the item function.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item">The item.</param>
        /// <param name="itemCollection">The item collection.</param>
        /// <param name="containsFunction">The contains function.</param>
        /// <exception cref="System.ArgumentNullException">item</exception>
        private void AddItemFunction<T>(T item, List<T> itemCollection, Func<bool> containsFunction)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (!containsFunction())
            {
                lock (this.thisLock)
                {
                    if (!containsFunction())
                    {
                        itemCollection.Add(item);
                    }
                }
            }
        }
    }
}