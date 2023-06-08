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
    using System.Linq;

    /// <summary>
    /// Class SingularEventToken. Implements the <see cref="Talegen.AspNetCore.Scim.Schema.EventTokenDecorator" />
    /// </summary>
    /// <seealso cref="Talegen.AspNetCore.Scim.Schema.EventTokenDecorator" />
    public abstract class SingularEventToken : EventTokenDecorator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SingularEventToken" /> class.
        /// </summary>
        /// <param name="innerToken">The inner token.</param>
        /// <exception cref="System.ArgumentException"></exception>
        protected SingularEventToken(IEventToken innerToken)
            : base(innerToken)
        {
            if (this.InnerToken.Events.Count != 1)
            {
                throw new ArgumentException(Properties.Resources.ExceptionSingleEventExpected);
            }

            KeyValuePair<string, object> singleEvent = this.InnerToken.Events.Single();
            this.SchemaIdentifier = singleEvent.Key;
            this.Event = new ReadOnlyDictionary<string, object>((IDictionary<string, object>)singleEvent.Value);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SingularEventToken" /> class.
        /// </summary>
        /// <param name="serialized">The serialized.</param>
        protected SingularEventToken(string serialized)
            : this(new EventToken(serialized))
        {
        }

        /// <summary>
        /// Gets the event.
        /// </summary>
        /// <value>The event.</value>
        public IReadOnlyDictionary<string, object> Event
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the schema identifier.
        /// </summary>
        /// <value>The schema identifier.</value>
        public string SchemaIdentifier
        {
            get;
            private set;
        }
    }
}