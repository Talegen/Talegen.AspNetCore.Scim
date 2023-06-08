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
    using Schema;

    /// <summary>
    /// This class implements a schema identifier object.
    /// </summary>
    public class SchemaIdentifier : ISchemaIdentifier
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SchemaIdentifier" /> class.
        /// </summary>
        /// <param name="value">Contains the initial value.</param>
        /// <exception cref="ArgumentNullException">Exception is thrown if the value is not specified.</exception>
        public SchemaIdentifier(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentNullException(nameof(value));
            }

            this.Value = value;
        }

        /// <inheritdoc />
        public string Value { get; }

        /// <inheritdoc />
        public string FindPath()
        {
            if (!this.TryFindPath(out string result))
            {
                throw new NotSupportedException(this.Value);
            }

            return result;
        }

        /// <inheritdoc />
        public bool TryFindPath(out string path)
        {
            bool result = true;
            path = null;

            switch (this.Value)
            {
                case SchemaIdentifiers.Core2EnterpriseUser:
                case SchemaIdentifiers.Core2User:
                    path = ProtocolConstants.PathUsers;
                    break;

                case SchemaIdentifiers.Core2Group:
                    path = ProtocolConstants.PathGroups;
                    break;

                case SchemaIdentifiers.None:
                    path = SchemaConstants.PathInterface;
                    break;

                default:
                    result = false;
                    break;
            }

            return result;
        }
    }
}