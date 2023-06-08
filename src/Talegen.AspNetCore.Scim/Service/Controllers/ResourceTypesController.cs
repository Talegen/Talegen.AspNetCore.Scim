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
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Monitor;
    using Protocol;
    using Schema;
    using Talegen.AspNetCore.Scim.Provider;

    /// <summary>
    /// This controller is used to handle SCIM Resource types requests.
    /// </summary>
    [Route(ServiceConstants.RouteResourceTypes)]
    [Authorize]
    [ApiController]
    public sealed class ResourceTypesController : ControllerTemplate
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceTypesController" /> class.
        /// </summary>
        /// <param name="provider">Contains an instance of the SCIM provider repository.</param>
        /// <param name="monitor">Contains an instance of the SCIM monitor plugin.</param>
        public ResourceTypesController(IProvider provider, IMonitor monitor)
            : base(provider, monitor)
        {
        }

        /// <summary>
        /// This method is called to get the supported resource types.
        /// </summary>
        /// <returns>Returns the query response for the request.</returns>
        /// <exception cref="HttpResponseException">Exception is thrown if the request forces an internal exception.</exception>
        [HttpGet]
        public QueryResponseBase Get()
        {
            string correlationIdentifier = null;
            QueryResponseBase result;

            try
            {
                HttpRequestMessage request = this.ConvertRequest();

                if (!request.TryGetRequestIdentifier(out correlationIdentifier))
                {
                    throw new HttpResponseException(HttpStatusCode.InternalServerError);
                }

                IProvider foundProvider = this.provider;

                if (foundProvider == null)
                {
                    throw new HttpResponseException(HttpStatusCode.InternalServerError);
                }

                IReadOnlyCollection<Resource> resources = foundProvider.ResourceTypes;
                result = new QueryResponse(resources);

                result.TotalResults = result.ItemsPerPage = resources.Count;
                result.StartIndex = 1;
            }
            catch (ArgumentException argumentException)
            {
                if (this.TryGetMonitor(out IMonitor monitor))
                {
                    IExceptionNotification notification =
                        ExceptionNotificationFactory.Instance.CreateNotification(
                            argumentException,
                            correlationIdentifier,
                            ServiceNotificationIdentifiers.ResourceTypesControllerGetArgumentException);
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
                            ServiceNotificationIdentifiers.ResourceTypesControllerGetNotImplementedException);
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
                           ServiceNotificationIdentifiers.ResourceTypesControllerGetNotSupportedException);
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
                           ServiceNotificationIdentifiers.ResourceTypesControllerGetException);
                    monitor.Report(notification);
                }

                throw;
            }

            return result;
        }
    }
}