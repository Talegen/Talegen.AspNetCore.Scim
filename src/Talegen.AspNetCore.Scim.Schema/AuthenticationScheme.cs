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
    using System.Runtime.Serialization;

    /// <summary>
    /// This class defines authentication schemes. This class cannot be inherited.
    /// </summary>
    [DataContract]
    public sealed class AuthenticationScheme
    {
        /// <summary>
        /// The authentication type resource value open standard for authentication bearer token
        /// </summary>
        private const string AuthenticationTypeResourceValueOpenStandardForAuthenticationBearerToken = "oauthbearertoken";

        /// <summary>
        /// The description open standard for authentication bearer token
        /// </summary>
        private const string DescriptionOpenStandardForAuthenticationBearerToken = "Authentication Scheme using the OAuth Bearer Token Standard";

        /// <summary>
        /// The documentation resource value open standard for authentication bearer token
        /// </summary>
        private const string DocumentationResourceValueOpenStandardForAuthenticationBearerToken = "http://example.com/help/oauth.html";

        /// <summary>
        /// The name open standard for authentication bearer token
        /// </summary>
        private const string NameOpenStandardForAuthenticationBearerToken = "OAuth Bearer Token";

        /// <summary>
        /// The specification resource value open standard for authentication bearer token
        /// </summary>
        private const string SpecificationResourceValueOpenStandardForAuthenticationBearerToken = "http://tools.ietf.org/html/draft-ietf-oauth-v2-bearer-01";

        /// <summary>
        /// The documentation resource open standard for authentication bearer token
        /// </summary>
        private static readonly Lazy<Uri> DocumentationResourceOpenStandardForAuthenticationBearerToken = new(() => new Uri(DocumentationResourceValueOpenStandardForAuthenticationBearerToken));

        /// <summary>
        /// The specification resource open standard for authentication bearer token
        /// </summary>
        private static readonly Lazy<Uri> SpecificationResourceOpenStandardForAuthenticationBearerToken = new(() => new Uri(SpecificationResourceValueOpenStandardForAuthenticationBearerToken));

        /// <summary>
        /// Gets or sets the type of the authentication.
        /// </summary>
        /// <value>The type of the authentication.</value>
        [DataMember(Name = AttributeNames.Type)]
        public string AuthenticationType { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        /// <value>The description.</value>
        [DataMember(Name = AttributeNames.Description)]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the documentation resource.
        /// </summary>
        /// <value>The documentation resource.</value>
        [DataMember(Name = AttributeNames.Documentation)]
        public Uri DocumentationResource { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>The name.</value>
        [DataMember(Name = AttributeNames.Name)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="AuthenticationScheme" /> is primary.
        /// </summary>
        /// <value><c>true</c> if primary; otherwise, <c>false</c>.</value>
        [DataMember(Name = AttributeNames.Primary)]
        public bool Primary { get; set; }

        /// <summary>
        /// Gets or sets the specification resource.
        /// </summary>
        /// <value>The specification resource.</value>
        [DataMember(Name = AttributeNames.Specification)]
        public Uri SpecificationResource { get; set; }

        /// <summary>
        /// Creates the open standard for authorization bearer token scheme.
        /// </summary>
        /// <returns>AuthenticationScheme.</returns>
        public static AuthenticationScheme CreateOpenStandardForAuthorizationBearerTokenScheme()
        {
            return new AuthenticationScheme
            {
                AuthenticationType = AuthenticationTypeResourceValueOpenStandardForAuthenticationBearerToken,
                Name = NameOpenStandardForAuthenticationBearerToken,
                Description = DescriptionOpenStandardForAuthenticationBearerToken,
                DocumentationResource = DocumentationResourceOpenStandardForAuthenticationBearerToken.Value,
                SpecificationResource = SpecificationResourceOpenStandardForAuthenticationBearerToken.Value
            };
        }
    }
}