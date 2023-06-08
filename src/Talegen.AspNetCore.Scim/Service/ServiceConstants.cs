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

namespace Talegen.AspNetCore.Scim.Service
{
    using Protocol;
    using Schema;

    /// <summary>
    /// This class contains service constants.
    /// </summary>
    public static class ServiceConstants
    {
        /// <summary>
        /// Contains segment separator.
        /// </summary>
        public const string SeparatorSegments = "/";

        /// <summary>
        /// Contains resource type path segment.
        /// </summary>
        public const string PathSegmentResourceTypes = "ResourceTypes";

        /// <summary>
        /// Contains schemas path segment.
        /// </summary>
        public const string PathSegmentSchemas = "Schemas";

        /// <summary>
        /// Contains service provider config segment.
        /// </summary>
        public const string PathSegmentServiceProviderConfiguration = "ServiceProviderConfig";

        /// <summary>
        /// Contains groups route.
        /// </summary>
        public const string RouteGroups = SchemaConstants.PathInterface + SeparatorSegments + ProtocolConstants.PathGroups;

        /// <summary>
        /// Contains resource types route.
        /// </summary>
        public const string RouteResourceTypes = SchemaConstants.PathInterface + SeparatorSegments + PathSegmentResourceTypes;

        /// <summary>
        /// Contains schemas route.
        /// </summary>
        public const string RouteSchemas = SchemaConstants.PathInterface + SeparatorSegments + PathSegmentSchemas;

        /// <summary>
        /// Contains service configuration route.
        /// </summary>
        public const string RouteServiceConfiguration = SchemaConstants.PathInterface + SeparatorSegments + PathSegmentServiceProviderConfiguration;

        /// <summary>
        /// Contains users route.
        /// </summary>
        public const string RouteUsers = SchemaConstants.PathInterface + SeparatorSegments + ProtocolConstants.PathUsers;

        /// <summary>
        /// Contains bulk operation route.
        /// </summary>
        public const string RouteBulk = SchemaConstants.PathInterface + SeparatorSegments + ProtocolConstants.PathBulk;
    }
}