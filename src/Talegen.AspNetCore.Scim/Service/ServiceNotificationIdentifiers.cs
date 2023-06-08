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
    // Members are numerically ordered.
    internal static class ServiceNotificationIdentifiers
    {
        public const long BulkRequest2ControllerPostArgumentException = 1;
        public const long BulkRequest2ControllerPostException = 2;
        public const long BulkRequest2ControllerPostNotImplementedException = 3;
        public const long BulkRequest2ControllerPostNotSupportedException = 4;

        public const long ControllerTemplateDeleteArgumentException = 5;
        public const long ControllerTemplateDeleteException = 6;
        public const long ControllerTemplateDeleteNotImplementedException = 7;
        public const long ControllerTemplateDeleteNotSupportedException = 8;

        public const long ControllerTemplateGetArgumentException = 9;
        public const long ControllerTemplateGetException = 10;
        public const long ControllerTemplateGetNotImplementedException = 11;
        public const long ControllerTemplateGetNotSupportedException = 12;

        public const long ControllerTemplatePatchArgumentException = 13;
        public const long ControllerTemplatePatchException = 14;
        public const long ControllerTemplatePatchNotImplementedException = 15;
        public const long ControllerTemplatePatchNotSupportedException = 16;

        public const long ControllerTemplatePostArgumentException = 17;
        public const long ControllerTemplatePostException = 18;
        public const long ControllerTemplatePostNotImplementedException = 19;
        public const long ControllerTemplatePostNotSupportedException = 20;

        public const long ControllerTemplatePutArgumentException = 21;
        public const long ControllerTemplatePutException = 22;
        public const long ControllerTemplatePutNotImplementedException = 23;
        public const long ControllerTemplatePutNotSupportedException = 24;

        public const long ControllerTemplateQueryArgumentException = 25;
        public const long ControllerTemplateQueryNotImplementedException = 26;
        public const long ControllerTemplateQueryNotSupportedException = 27;
        public const long ControllerTemplateQueryException = 28;

        public const long MonitoringMiddlewareInvocationException = 29;
        public const long MonitoringMiddlewareReception = 30;
        public const long MonitoringMiddlewareRequestProcessed = 31;

        public const long ResourceTypesControllerGetArgumentException = 32;
        public const long ResourceTypesControllerGetException = 33;
        public const long ResourceTypesControllerGetNotImplementedException = 34;
        public const long ResourceTypesControllerGetNotSupportedException = 35;

        public const long SchematizedMediaTypeFormatterReadFromStream = 36;
        public const long SchematizedMediaTypeFormatterWroteToStream = 37;

        public const long SchemasControllerGetArgumentException = 38;
        public const long SchemasControllerGetException = 39;
        public const long SchemasControllerGetNotImplementedException = 40;
        public const long SchemasControllerGetNotSupportedException = 41;

        public const long ServiceProviderConfigurationControllerGetArgumentException = 42;
        public const long ServiceProviderConfigurationControllerGetException = 43;
        public const long ServiceProviderConfigurationControllerGetNotImplementedException = 44;
        public const long ServiceProviderConfigurationControllerGetNotSupportedException = 45;

        public const long EventsControllerPostArgumentException = 46;
        public const long EventsControllerPostException = 47;
        public const long EventsControllerPostNotImplementedException = 48;
        public const long EventsControllerPostNotSupportedException = 49;
    }
}