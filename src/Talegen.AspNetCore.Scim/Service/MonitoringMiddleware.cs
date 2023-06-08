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
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Http.Extensions;
    using Monitor;
    using Talegen.AspNetCore.Scim.Schema;

    /// <summary>
    /// This class implements a class for executing monitoring within the ASP.net middleware.
    /// </summary>
    public sealed class MonitoringMiddleware
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MonitoringMiddleware" /> class.
        /// </summary>
        /// <param name="next">Contains the next request delegate.</param>
        /// <param name="monitor">Contains an instance of the monitor.</param>
        /// <exception cref="ArgumentNullException">Exception is thrown if a monitor is not specified.</exception>
        public MonitoringMiddleware(RequestDelegate next, IMonitor monitor)
        {
            this.Monitor = monitor ?? throw new ArgumentNullException(nameof(monitor));
            this.Next = next;
        }

        /// <summary>
        /// Gets the monitor.
        /// </summary>
        private IMonitor Monitor { get; }

        /// <summary>
        /// Contains the next request delegate.
        /// </summary>
        private RequestDelegate Next { get; }

        /// <summary>
        /// This method is used to compose a new request.
        /// </summary>
        /// <param name="context">Contains the current HTTP context.</param>
        /// <returns>Returns a new request.</returns>
        /// <exception cref="ArgumentNullException">Exception is thrown when context is not specified.</exception>
        private static string ComposeRequest(HttpContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            string method = null;
            Uri resource = null;
            string headers = null;

            if (context.Request != null)
            {
                method = context.Request.Method;
                resource = new Uri(context.Request.GetDisplayUrl());

                if (context.Request.Headers != null)
                {
                    headers = context.Request.Headers.ToDictionary(item => item.Key, item => string.Join(Schema.Properties.Resources.SeparatorHeaderValues, item.Value))
                        .Select((item) => string.Format(CultureInfo.InvariantCulture, Schema.Properties.Resources.HeaderTemplate, item.Key, item.Value))
                        .Aggregate(new StringBuilder(), (aggregate, item) => aggregate.AppendLine(item))
                        .ToString();
                }
            }

            string result = string.Format(CultureInfo.InvariantCulture, Schema.Properties.Resources.MessageTemplate, method, resource, headers);
            return result;
        }

        /// <summary>
        /// This method is invoked to execute the middleware.
        /// </summary>
        /// <param name="context">Contains the HTTP context.</param>
        /// <exception cref="ArgumentNullException">Exception is thrown when the context is not specified.</exception>
        /// <exception cref="ArgumentException">Exception is thrown when the request context is invalid.</exception>
        public async Task InvokeAsync(HttpContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (context.Request == null)
            {
                throw new ArgumentException(Schema.Properties.Resources.ExceptionInvalidContext);
            }

            // get the request identifier
            Uri resource = new Uri(context.Request.GetDisplayUrl());
            var resourceIdentifier = new SystemForCrossDomainIdentityManagementResourceIdentifier(resource);
            string requestIdentifier = resourceIdentifier.RelativePath;

            string message = ComposeRequest(context);

            IInformationNotification receptionNotification = InformationNotificationFactory.Instance.FormatNotification(
                Schema.Properties.Resources.InformationRequestReceivedTemplate, requestIdentifier,
                ServiceNotificationIdentifiers.MonitoringMiddlewareReception, message);

            this.Monitor.Inform(receptionNotification);

            try
            {
                await this.Next(context);
            }
            catch (Exception exception)
            {
                IExceptionNotification exceptionNotification = ExceptionNotificationFactory.Instance.CreateNotification(exception, requestIdentifier,
            ServiceNotificationIdentifiers.MonitoringMiddlewareInvocationException); this.Monitor.Report(exceptionNotification);

                throw;
            }

            string responseStatusCode = context.Response != null ? context.Response.StatusCode.ToString(CultureInfo.InvariantCulture) : null;

            IInformationNotification processedNotification =
                InformationNotificationFactory.Instance.FormatNotification(
                    Schema.Properties.Resources.InformationRequestProcessedTemplate,
                    requestIdentifier,
                    ServiceNotificationIdentifiers.MonitoringMiddlewareRequestProcessed,
                    message,
                    responseStatusCode);
            this.Monitor.Inform(processedNotification);
        }
    }
}