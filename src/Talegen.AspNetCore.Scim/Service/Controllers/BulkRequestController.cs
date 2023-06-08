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

namespace Talegen.AspNetCore.Scim.Service.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Monitor;
    using Protocol;
    using Talegen.AspNetCore.Scim.Provider;

    /// <summary>
    /// This controller is used to handle SCIM bulk requests.
    /// </summary>
    [Route(ServiceConstants.RouteBulk)]
    [Authorize]
    [ApiController]
    public sealed class BulkRequestController : ControllerTemplate
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BulkRequestController" /> class.
        /// </summary>
        /// <param name="provider">Contains an instance of the SCIM provider repository.</param>
        /// <param name="monitor">Contains an instance of the SCIM monitor plugin.</param>
        public BulkRequestController(IProvider provider, IMonitor monitor)
            : base(provider, monitor)
        {
        }

        /// <summary>
        /// This method handles Bulk requests posted to the controller.
        /// </summary>
        /// <param name="bulkRequest">Contains the bulk request data contract model.</param>
        /// <returns>Returns the response as a <see cref="BulkResponse2" /> model.</returns>
        /// <exception cref="HttpResponseException">Exception is thrown if the bulk request is not specified.</exception>
        [HttpPost]
        public async Task<BulkResponse2> Post([FromBody] BulkRequest2 bulkRequest)
        {
            string correlationIdentifier = null;
            BulkResponse2 response = null;

            try
            {
                HttpRequestMessage request = this.ConvertRequest();

                if (bulkRequest == null)
                {
                    throw new HttpResponseException(HttpStatusCode.BadRequest);
                }

                if (!request.TryGetRequestIdentifier(out correlationIdentifier))
                {
                    throw new HttpResponseException(HttpStatusCode.InternalServerError);
                }

                IReadOnlyCollection<IExtension> extensions = provider.ReadExtensions();
                IRequest<BulkRequest2> request2 = new BulkRequest(request, bulkRequest, correlationIdentifier, extensions);
                BulkResponse2 result = await provider.ProcessAsync(request2).ConfigureAwait(false);
                return result;
            }
            catch (ArgumentException argumentException)
            {
                if (this.TryGetMonitor(out IMonitor monitor))
                {
                    IExceptionNotification notification =
                        ExceptionNotificationFactory.Instance.CreateNotification(
                            argumentException,
                            correlationIdentifier,
                            ServiceNotificationIdentifiers.BulkRequest2ControllerPostArgumentException);
                    monitor.Report(notification);
                }

                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }
            catch (NotImplementedException notImplementedException)
            {
                if (this.TryGetMonitor(out IMonitor monitor))
                {
                    IExceptionNotification notification =
                        ExceptionNotificationFactory.Instance.CreateNotification(
                            notImplementedException,
                            correlationIdentifier,
                            ServiceNotificationIdentifiers.BulkRequest2ControllerPostNotImplementedException);
                    monitor.Report(notification);
                }

                throw new HttpResponseException(HttpStatusCode.NotImplemented);
            }
            catch (NotSupportedException notSupportedException)
            {
                if (this.TryGetMonitor(out IMonitor monitor))
                {
                    IExceptionNotification notification =
                        ExceptionNotificationFactory.Instance.CreateNotification(
                            notSupportedException,
                            correlationIdentifier,
                            ServiceNotificationIdentifiers.BulkRequest2ControllerPostNotSupportedException);
                    monitor.Report(notification);
                }

                throw new HttpResponseException(HttpStatusCode.NotImplemented);
            }
            catch (Exception exception)
            {
                if (this.TryGetMonitor(out IMonitor monitor))
                {
                    IExceptionNotification notification =
                        ExceptionNotificationFactory.Instance.CreateNotification(
                            exception,
                            correlationIdentifier,
                            ServiceNotificationIdentifiers.BulkRequest2ControllerPostException);
                    monitor.Report(notification);
                }

                throw;
            }
        }
    }
}