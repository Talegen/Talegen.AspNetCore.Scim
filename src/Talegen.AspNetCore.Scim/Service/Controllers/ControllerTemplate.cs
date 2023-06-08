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
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Controllers;
    using Http;
    using Microsoft.AspNetCore.Mvc;
    using Monitor;
    using Protocol;
    using Schema;
    using Talegen.AspNetCore.Scim.Provider;

    /// <summary>
    /// This class implements the core logic for SCIM related controllers.
    /// </summary>
    public abstract class ControllerTemplate : ControllerBase
    {
        /// <summary>
        /// Contains an attribute value identifier.
        /// </summary>
        internal const string AttributeValueIdentifier = "{identifier}";

        /// <summary>
        /// Contains the Header content type name.
        /// </summary>
        private const string HeaderKeyContentType = "Content-Type";

        /// <summary>
        /// Contains the Header location name.
        /// </summary>
        private const string HeaderKeyLocation = "Location";

        /// <summary>
        /// Contains an instance of the SCIM monitor plugin.
        /// </summary>
        internal readonly IMonitor monitor;

        /// <summary>
        /// Contains an instance of the SCIM provider repository.
        /// </summary>
        internal readonly IProvider provider;

        /// <summary>
        /// Initializes a new instance of the <see cref="ControllerTemplate" /> class.
        /// </summary>
        /// <param name="provider">Contains an instance of the SCIM provider repository.</param>
        /// <param name="monitor">Contains an instance of the SCIM monitor plugin.</param>
        /// <exception cref="HttpResponseException">Exception is thrown if a base provider is not specified.</exception>
        internal ControllerTemplate(IProvider provider, IMonitor monitor)
        {
            this.monitor = monitor;
            this.provider = provider ?? throw new HttpResponseException(HttpStatusCode.InternalServerError);
        }

        /// <summary>
        /// This method is used to set the controller response information based on the resource.
        /// </summary>
        /// <param name="resource"></param>
        protected virtual void ConfigureResponse(Resource resource)
        {
            this.Response.ContentType = ProtocolConstants.ContentType;
            this.Response.StatusCode = (int)HttpStatusCode.Created;

            if (this.Response.Headers != null)
            {
                if (!this.Response.Headers.ContainsKey(HeaderKeyContentType))
                {
                    this.Response.Headers.Add(HeaderKeyContentType, ProtocolConstants.ContentType);
                }

                Uri baseResourceIdentifier = this.ConvertRequest().GetBaseResourceIdentifier();
                Uri resourceIdentifier = resource.GetResourceIdentifier(baseResourceIdentifier);
                string resourceLocation = resourceIdentifier.AbsoluteUri;
                if (!this.Response.Headers.ContainsKey(HeaderKeyLocation))
                {
                    this.Response.Headers.Add(HeaderKeyLocation, resourceLocation);
                }
            }
        }

        /// <summary>
        /// This method is used to convert the current context to an <see cref="HttpRequestMessage" /> object.
        /// </summary>
        /// <returns></returns>
        protected HttpRequestMessage ConvertRequest()
        {
            HttpRequestMessageFeature requestMessageFeature = new HttpRequestMessageFeature(this.HttpContext);
            HttpRequestMessage result = requestMessageFeature.HttpRequestMessage;
            return result;
        }

        /// <summary>
        /// This method is used to return an SCIM error result object.
        /// </summary>
        /// <param name="httpStatusCode">Contains the HTTP status code.</param>
        /// <param name="message">Contains a message to return.</param>
        /// <returns>Returns a new status code <see cref="ObjectResult" /> object.</returns>
        protected ObjectResult ScimError(HttpStatusCode httpStatusCode, string message)
        {
            return StatusCode((int)httpStatusCode, new Core2Error(message, (int)httpStatusCode));
        }

        /// <summary>
        /// This method is used to try and get the current monitor plugin.
        /// </summary>
        /// <param name="monitor">Contains the monitor found.</param>
        /// <returns>Returns a value indicating whether the monitor was found.</returns>
        protected virtual bool TryGetMonitor(out IMonitor monitor)
        {
            monitor = this.monitor;
            return monitor != null;
        }
    }

    /// <summary>
    /// This class implements a controller template with provider adapter for a given type <typeparamref name="T" />.
    /// </summary>
    /// <typeparam name="T">Contains the model type for the controller provider adapter.</typeparam>
    public abstract class ControllerTemplate<T> : ControllerTemplate where T : Resource
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ControllerTemplate" /> class.
        /// </summary>
        /// <param name="provider">Contains an instance of the SCIM provider repository.</param>
        /// <param name="monitor">Contains an instance of the SCIM monitor plugin.</param>
        internal ControllerTemplate(IProvider provider, IMonitor monitor)
            : base(provider, monitor)
        {
        }

        /// <summary>
        /// This method is used to implement an provider adapter for the given provider.
        /// </summary>
        /// <param name="provider">Contains the provider to implement a provider adapter for.</param>
        /// <returns>Returns the provider adapter for the given <typeparamref name="T" /> type.</returns>
        protected abstract IProviderAdapter<T> AdaptProvider(IProvider provider);

        /// <summary>
        /// This method is used to implement an provider adapter for the given provider.
        /// </summary>
        /// <returns>Returns the provider adapter for the given <typeparamref name="T" /> type.</returns>
        protected virtual IProviderAdapter<T> AdaptProvider()
        {
            IProviderAdapter<T> result = this.AdaptProvider(this.provider);
            return result;
        }

        /// <summary>
        /// This method is used to delete a reference by identifier.
        /// </summary>
        /// <param name="identifier">Contains the identifier of the reference to delete.</param>
        /// <returns>Returns an action result of the operation.</returns>
        /// <exception cref="HttpResponseException">Exception is thrown if the identifier is not specified.</exception>
        [HttpDelete(AttributeValueIdentifier)]
        public virtual async Task<IActionResult> Delete(string identifier)
        {
            string correlationIdentifier = null;
            IActionResult actionResult = this.BadRequest();

            try
            {
                if (!string.IsNullOrWhiteSpace(identifier))
                {
                    identifier = Uri.UnescapeDataString(identifier);
                    HttpRequestMessage request = this.ConvertRequest();
                    if (!request.TryGetRequestIdentifier(out correlationIdentifier))
                    {
                        throw new HttpResponseException(HttpStatusCode.InternalServerError);
                    }

                    IProviderAdapter<T> provider = this.AdaptProvider();
                    await provider.Delete(request, identifier, correlationIdentifier).ConfigureAwait(false);
                    actionResult = this.NoContent();
                }
            }
            catch (ArgumentException argumentException)
            {
                if (this.TryGetMonitor(out IMonitor monitor))
                {
                    IExceptionNotification notification =
                        ExceptionNotificationFactory.Instance.CreateNotification(
                            argumentException,
                            correlationIdentifier,
                            ServiceNotificationIdentifiers.ControllerTemplateDeleteArgumentException);
                    monitor.Report(notification);
                }
            }
            catch (HttpResponseException responseException)
            {
                if (responseException.Response?.StatusCode == HttpStatusCode.NotFound)
                {
                    actionResult = this.NotFound();
                }
                else
                {
                    throw;
                }
            }
            catch (NotImplementedException notImplementedException)
            {
                if (this.TryGetMonitor(out IMonitor monitor))
                {
                    IExceptionNotification notification =
                        ExceptionNotificationFactory.Instance.CreateNotification(
                            notImplementedException,
                            correlationIdentifier,
                            ServiceNotificationIdentifiers.ControllerTemplateDeleteNotImplementedException);
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
                            ServiceNotificationIdentifiers.ControllerTemplateDeleteNotSupportedException);
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
                            ServiceNotificationIdentifiers.ControllerTemplateDeleteException);
                    monitor.Report(notification);
                }

                throw;
            }

            return actionResult;
        }

        /// <summary>
        /// This method is used to get a reference object.
        /// </summary>
        /// <returns>Returns an action result of the operation.</returns>
        [HttpGet]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Get", Justification = "The names of the methods of a controller must correspond to the names of hypertext markup verbs")]
        public virtual async Task<ActionResult<QueryResponseBase>> Get()
        {
            string correlationIdentifier = null;
            ActionResult<QueryResponseBase> actionResult = this.BadRequest();

            try
            {
                HttpRequestMessage request = this.ConvertRequest();

                if (!request.TryGetRequestIdentifier(out correlationIdentifier))
                {
                    throw new HttpResponseException(HttpStatusCode.InternalServerError);
                }

                IResourceQuery resourceQuery = new ResourceQuery(request.RequestUri);
                IProviderAdapter<T> provider = this.AdaptProvider();
                QueryResponseBase result =
                    await provider
                            .Query(
                                request,
                                resourceQuery.Filters,
                                resourceQuery.Attributes,
                                resourceQuery.ExcludedAttributes,
                                resourceQuery.PaginationParameters,
                                correlationIdentifier)
                            .ConfigureAwait(false);
                actionResult = this.Ok(result);
            }
            catch (ArgumentException argumentException)
            {
                if (this.TryGetMonitor(out IMonitor monitor))
                {
                    IExceptionNotification notification =
                        ExceptionNotificationFactory.Instance.CreateNotification(
                            argumentException,
                            correlationIdentifier,
                            ServiceNotificationIdentifiers.ControllerTemplateQueryArgumentException);
                    monitor.Report(notification);
                }

                actionResult = this.ScimError(HttpStatusCode.BadRequest, argumentException.Message);
            }
            catch (NotImplementedException notImplementedException)
            {
                if (this.TryGetMonitor(out IMonitor monitor))
                {
                    IExceptionNotification notification =
                        ExceptionNotificationFactory.Instance.CreateNotification(
                            notImplementedException,
                            correlationIdentifier,
                            ServiceNotificationIdentifiers.ControllerTemplateQueryNotImplementedException);
                    monitor.Report(notification);
                }

                actionResult = this.ScimError(HttpStatusCode.NotImplemented, notImplementedException.Message);
            }
            catch (NotSupportedException notSupportedException)
            {
                if (this.TryGetMonitor(out IMonitor monitor))
                {
                    IExceptionNotification notification =
                        ExceptionNotificationFactory.Instance.CreateNotification(
                            notSupportedException,
                            correlationIdentifier,
                            ServiceNotificationIdentifiers.ControllerTemplateQueryNotSupportedException);
                    monitor.Report(notification);
                }

                actionResult = this.ScimError(HttpStatusCode.BadRequest, notSupportedException.Message);
            }
            catch (HttpResponseException responseException)
            {
                if (responseException.Response?.StatusCode != HttpStatusCode.NotFound)
                {
                    if (this.TryGetMonitor(out IMonitor monitor))
                    {
                        IExceptionNotification notification =
                            ExceptionNotificationFactory.Instance.CreateNotification(
                                responseException.InnerException ?? responseException,
                                correlationIdentifier,
                                ServiceNotificationIdentifiers.ControllerTemplateGetException);
                        monitor.Report(notification);
                    }
                }

                actionResult = this.ScimError(HttpStatusCode.InternalServerError, responseException.Message);
            }
            catch (Exception exception)
            {
                if (this.TryGetMonitor(out IMonitor monitor))
                {
                    IExceptionNotification notification =
                        ExceptionNotificationFactory.Instance.CreateNotification(
                            exception,
                            correlationIdentifier,
                            ServiceNotificationIdentifiers.ControllerTemplateQueryException);
                    monitor.Report(notification);
                }

                actionResult = this.ScimError(HttpStatusCode.InternalServerError, exception.Message);
            }

            return actionResult;
        }

        /// <summary>
        /// This method is used to get a reference object by identifier.
        /// </summary>
        /// <param name="identifier">Contains the identifier of the reference object to get.</param>
        /// <returns>Returns an action result of the operation.</returns>
        [HttpGet(ControllerTemplate.AttributeValueIdentifier)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Get", Justification = "The names of the methods of a controller must correspond to the names of hypertext markup verbs")]
        public virtual async Task<IActionResult> Get([FromUri] string identifier)
        {
            string correlationIdentifier = null;
            IActionResult actionResult;

            try
            {
                if (!string.IsNullOrWhiteSpace(identifier))
                {
                    HttpRequestMessage request = this.ConvertRequest();

                    if (!request.TryGetRequestIdentifier(out correlationIdentifier))
                    {
                        throw new HttpResponseException(HttpStatusCode.InternalServerError);
                    }

                    IResourceQuery resourceQuery = new ResourceQuery(request.RequestUri);

                    if (resourceQuery.Filters.Any())
                    {
                        if (resourceQuery.Filters.Count == 1)
                        {
                            IFilter filter = new Filter(AttributeNames.Identifier, ComparisonOperator.Equals, identifier);
                            filter.AdditionalFilter = resourceQuery.Filters.Single();
                            IReadOnlyCollection<IFilter> filters = new IFilter[] { filter };
                            IResourceQuery effectiveQuery = new ResourceQuery(filters, resourceQuery.Attributes, resourceQuery.ExcludedAttributes);
                            IProviderAdapter<T> provider = this.AdaptProvider();
                            QueryResponseBase queryResponse = await provider.Query(request, effectiveQuery.Filters,
                                    effectiveQuery.Attributes, effectiveQuery.ExcludedAttributes,
                                    effectiveQuery.PaginationParameters, correlationIdentifier);

                            if (queryResponse.Resources.Any())
                            {
                                Resource result = queryResponse.Resources.Single();
                                actionResult = this.Ok(result);
                            }
                            else
                            {
                                actionResult = this.ScimError(HttpStatusCode.NotFound, string.Format(Schema.Properties.Resources.ResourceNotFoundTemplate, identifier));
                            }
                        }
                        else
                        {
                            actionResult = this.ScimError(HttpStatusCode.BadRequest, Schema.Properties.Resources.ExceptionFilterCount);
                        }
                    }
                    else
                    {
                        IProviderAdapter<T> provider = this.AdaptProvider();
                        Resource result = await provider.Retrieve(request, identifier, resourceQuery.Attributes,
                                resourceQuery.ExcludedAttributes, correlationIdentifier);

                        if (result != null)
                        {
                            actionResult = this.Ok(result);
                        }
                        else
                        {
                            actionResult = this.ScimError(HttpStatusCode.NotFound, string.Format(Schema.Properties.Resources.ResourceNotFoundTemplate, identifier));
                        }
                    }
                }
                else
                {
                    actionResult = this.ScimError(HttpStatusCode.BadRequest, Schema.Properties.Resources.ExceptionInvalidIdentifier);
                }
            }
            catch (ArgumentException argumentException)
            {
                if (this.TryGetMonitor(out IMonitor monitor))
                {
                    IExceptionNotification notification =
                        ExceptionNotificationFactory.Instance.CreateNotification(
                            argumentException,
                            correlationIdentifier,
                            ServiceNotificationIdentifiers.ControllerTemplateGetArgumentException);
                    monitor.Report(notification);
                }

                actionResult = this.ScimError(HttpStatusCode.BadRequest, argumentException.Message);
            }
            catch (NotImplementedException notImplementedException)
            {
                if (this.TryGetMonitor(out IMonitor monitor))
                {
                    IExceptionNotification notification =
                        ExceptionNotificationFactory.Instance.CreateNotification(
                            notImplementedException,
                            correlationIdentifier,
                            ServiceNotificationIdentifiers.ControllerTemplateGetNotImplementedException);
                    monitor.Report(notification);
                }

                actionResult = this.ScimError(HttpStatusCode.NotImplemented, notImplementedException.Message);
            }
            catch (NotSupportedException notSupportedException)
            {
                if (this.TryGetMonitor(out IMonitor monitor))
                {
                    IExceptionNotification notification =
                        ExceptionNotificationFactory.Instance.CreateNotification(
                            notSupportedException,
                            correlationIdentifier,
                            ServiceNotificationIdentifiers.ControllerTemplateGetNotSupportedException);
                    monitor.Report(notification);
                }

                actionResult = this.ScimError(HttpStatusCode.BadRequest, notSupportedException.Message);
            }
            catch (HttpResponseException responseException)
            {
                if (responseException.Response?.StatusCode != HttpStatusCode.NotFound)
                {
                    if (this.TryGetMonitor(out IMonitor monitor))
                    {
                        IExceptionNotification notification =
                            ExceptionNotificationFactory.Instance.CreateNotification(
                                responseException.InnerException ?? responseException,
                                correlationIdentifier,
                                ServiceNotificationIdentifiers.ControllerTemplateGetException);
                        monitor.Report(notification);
                    }
                }

                if (responseException.Response?.StatusCode == HttpStatusCode.NotFound)
                {
                    actionResult = this.ScimError(HttpStatusCode.NotFound, string.Format(Schema.Properties.Resources.ResourceNotFoundTemplate, identifier));
                }
                else
                {
                    actionResult = this.ScimError(HttpStatusCode.InternalServerError, responseException.Message);
                }
            }
            catch (Exception exception)
            {
                if (this.TryGetMonitor(out IMonitor monitor))
                {
                    IExceptionNotification notification =
                        ExceptionNotificationFactory.Instance.CreateNotification(
                            exception,
                            correlationIdentifier,
                            ServiceNotificationIdentifiers.ControllerTemplateGetException);
                    monitor.Report(notification);
                }

                actionResult = this.ScimError(HttpStatusCode.InternalServerError, exception.Message);
            }

            return actionResult;
        }

        /// <summary>
        /// This method is used to patch a reference object by identifier.
        /// </summary>
        /// <param name="identifier">Contains the identifier of the reference object to patch.</param>
        /// <param name="patchRequest">Contains the patch operation model.</param>
        /// <returns>Returns an action result of the operation.</returns>
        /// <exception cref="HttpResponseException">Exception is thrown if the identifiers are not specified.</exception>
        [HttpPatch(AttributeValueIdentifier)]
        public virtual async Task<IActionResult> Patch(string identifier, [FromBody] PatchRequest2 patchRequest)
        {
            string correlationIdentifier = null;
            IActionResult actionResult = this.BadRequest();

            try
            {
                if (!string.IsNullOrWhiteSpace(identifier))
                {
                    identifier = Uri.UnescapeDataString(identifier);

                    if (patchRequest != null)
                    {
                        HttpRequestMessage request = this.ConvertRequest();

                        if (!request.TryGetRequestIdentifier(out correlationIdentifier))
                        {
                            throw new HttpResponseException(HttpStatusCode.InternalServerError);
                        }

                        IProviderAdapter<T> adaptProvider = this.AdaptProvider();
                        await adaptProvider.Update(request, identifier, patchRequest, correlationIdentifier).ConfigureAwait(false);

                        // If EnterpriseUser, return HTTP code 200 and user object, otherwise HTTP code 204
                        if (adaptProvider.SchemaIdentifier == SchemaIdentifiers.Core2EnterpriseUser)
                        {
                            actionResult = await this.Get(identifier);
                        }
                        else
                        {
                            actionResult = this.NoContent();
                        }
                    }
                }
            }
            catch (ArgumentException argumentException)
            {
                if (this.TryGetMonitor(out IMonitor monitor))
                {
                    IExceptionNotification notification =
                        ExceptionNotificationFactory.Instance.CreateNotification(
                            argumentException,
                            correlationIdentifier,
                            ServiceNotificationIdentifiers.ControllerTemplatePatchArgumentException);
                    monitor.Report(notification);
                }
            }
            catch (NotImplementedException notImplementedException)
            {
                if (this.TryGetMonitor(out IMonitor monitor))
                {
                    IExceptionNotification notification =
                        ExceptionNotificationFactory.Instance.CreateNotification(
                            notImplementedException,
                            correlationIdentifier,
                            ServiceNotificationIdentifiers.ControllerTemplatePatchNotImplementedException);
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
                            ServiceNotificationIdentifiers.ControllerTemplatePatchNotSupportedException);
                    monitor.Report(notification);
                }

                throw new HttpResponseException(HttpStatusCode.NotImplemented);
            }
            catch (HttpResponseException responseException)
            {
                if (responseException.Response?.StatusCode == HttpStatusCode.NotFound)
                {
                    actionResult = this.NotFound();
                }
                else
                {
                    if (this.TryGetMonitor(out IMonitor monitor))
                    {
                        IExceptionNotification notification =
                            ExceptionNotificationFactory.Instance.CreateNotification(
                                responseException.InnerException ?? responseException,
                                correlationIdentifier,
                                ServiceNotificationIdentifiers.ControllerTemplateGetException);
                        monitor.Report(notification);
                    }

                    throw;
                }
            }
            catch (Exception exception)
            {
                if (this.TryGetMonitor(out IMonitor monitor))
                {
                    IExceptionNotification notification =
                        ExceptionNotificationFactory.Instance.CreateNotification(
                            exception,
                            correlationIdentifier,
                            ServiceNotificationIdentifiers.ControllerTemplatePatchException);
                    monitor.Report(notification);
                }

                throw;
            }

            return actionResult;
        }

        /// <summary>
        /// This method is used to post a new reference object.
        /// </summary>
        /// <param name="resource">Contains the resource object to create.</param>
        /// <returns>Returns an action result for the operation.</returns>
        /// <exception cref="HttpResponseException">Exception is thrown if identifier is not specified.</exception>
        [HttpPost]
        public virtual async Task<ActionResult<Resource>> Post([FromBody] T resource)
        {
            string correlationIdentifier = null;
            ActionResult<Resource> actionResult = this.BadRequest();

            try
            {
                if (resource != null)
                {
                    HttpRequestMessage request = this.ConvertRequest();

                    if (!request.TryGetRequestIdentifier(out correlationIdentifier))
                    {
                        throw new HttpResponseException(HttpStatusCode.InternalServerError);
                    }

                    IProviderAdapter<T> provider = this.AdaptProvider();
                    Resource result = await provider.Create(request, resource, correlationIdentifier).ConfigureAwait(false);
                    this.ConfigureResponse(result);

                    actionResult = this.CreatedAtAction(nameof(this.Post), result);
                }
            }
            catch (ArgumentException argumentException)
            {
                if (this.TryGetMonitor(out IMonitor monitor))
                {
                    IExceptionNotification notification =
                        ExceptionNotificationFactory.Instance.CreateNotification(
                            argumentException,
                            correlationIdentifier,
                            ServiceNotificationIdentifiers.ControllerTemplatePostArgumentException);
                    monitor.Report(notification);
                }
            }
            catch (NotImplementedException notImplementedException)
            {
                if (this.TryGetMonitor(out IMonitor monitor))
                {
                    IExceptionNotification notification =
                        ExceptionNotificationFactory.Instance.CreateNotification(
                            notImplementedException,
                            correlationIdentifier,
                            ServiceNotificationIdentifiers.ControllerTemplatePostNotImplementedException);
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
                            ServiceNotificationIdentifiers.ControllerTemplatePostNotSupportedException);
                    monitor.Report(notification);
                }

                throw new HttpResponseException(HttpStatusCode.NotImplemented);
            }
            catch (HttpResponseException httpResponseException)
            {
                if (this.TryGetMonitor(out IMonitor monitor))
                {
                    IExceptionNotification notification =
                        ExceptionNotificationFactory.Instance.CreateNotification(
                            httpResponseException,
                            correlationIdentifier,
                            ServiceNotificationIdentifiers.ControllerTemplatePostNotSupportedException);
                    monitor.Report(notification);
                }

                if (httpResponseException.Response.StatusCode == HttpStatusCode.Conflict)
                {
                    actionResult = this.Conflict();
                }
            }
            catch (Exception exception)
            {
                if (this.TryGetMonitor(out IMonitor monitor))
                {
                    IExceptionNotification notification =
                        ExceptionNotificationFactory.Instance.CreateNotification(
                            exception,
                            correlationIdentifier,
                            ServiceNotificationIdentifiers.ControllerTemplatePostException);
                    monitor.Report(notification);
                }

                throw;
            }

            return actionResult;
        }

        /// <summary>
        /// This method is used to put an update to an existing reference object by identifier.
        /// </summary>
        /// <param name="resource">Contains the resource model information to update.</param>
        /// <param name="identifier">Contains the identifier of the resource object to update.</param>
        /// <returns></returns>
        [HttpPut(AttributeValueIdentifier)]
        public virtual async Task<ActionResult<Resource>> Put([FromBody] T resource, string identifier)
        {
            string correlationIdentifier = null;
            ActionResult<Resource> actionResult = null;

            try
            {
                if (resource != null)
                {
                    if (!string.IsNullOrEmpty(identifier))
                    {
                        HttpRequestMessage request = this.ConvertRequest();

                        if (!request.TryGetRequestIdentifier(out correlationIdentifier))
                        {
                            throw new HttpResponseException(HttpStatusCode.InternalServerError);
                        }

                        IProviderAdapter<T> provider = this.AdaptProvider();
                        Resource result = await provider.Replace(request, resource, correlationIdentifier).ConfigureAwait(false);
                        this.ConfigureResponse(result);
                        actionResult = this.Ok(result);
                    }
                    else
                    {
                        actionResult = this.ScimError(HttpStatusCode.BadRequest, Schema.Properties.Resources.ExceptionInvalidIdentifier);
                    }
                }
                else
                {
                    actionResult = this.ScimError(HttpStatusCode.BadRequest, Schema.Properties.Resources.ExceptionInvalidResource);
                }
            }
            catch (ArgumentException argumentException)
            {
                if (this.TryGetMonitor(out IMonitor monitor))
                {
                    IExceptionNotification notification =
                        ExceptionNotificationFactory.Instance.CreateNotification(
                            argumentException,
                            correlationIdentifier,
                            ServiceNotificationIdentifiers.ControllerTemplatePutArgumentException);
                    monitor.Report(notification);
                }

                actionResult = this.ScimError(HttpStatusCode.BadRequest, argumentException.Message);
            }
            catch (NotImplementedException notImplementedException)
            {
                if (this.TryGetMonitor(out IMonitor monitor))
                {
                    IExceptionNotification notification =
                        ExceptionNotificationFactory.Instance.CreateNotification(
                            notImplementedException,
                            correlationIdentifier,
                            ServiceNotificationIdentifiers.ControllerTemplatePutNotImplementedException);
                    monitor.Report(notification);
                }

                actionResult = this.ScimError(HttpStatusCode.NotImplemented, notImplementedException.Message);
            }
            catch (NotSupportedException notSupportedException)
            {
                if (this.TryGetMonitor(out IMonitor monitor))
                {
                    IExceptionNotification notification =
                        ExceptionNotificationFactory.Instance.CreateNotification(
                            notSupportedException,
                            correlationIdentifier,
                            ServiceNotificationIdentifiers.ControllerTemplatePutNotSupportedException);
                    monitor.Report(notification);
                }

                actionResult = this.ScimError(HttpStatusCode.BadRequest, notSupportedException.Message);
            }
            catch (HttpResponseException httpResponseException)
            {
                if (this.TryGetMonitor(out IMonitor monitor))
                {
                    IExceptionNotification notification =
                        ExceptionNotificationFactory.Instance.CreateNotification(
                            httpResponseException,
                            correlationIdentifier,
                            ServiceNotificationIdentifiers.ControllerTemplatePostNotSupportedException);
                    monitor.Report(notification);
                }

                actionResult = httpResponseException.Response.StatusCode switch
                {
                    HttpStatusCode.NotFound => this.ScimError(HttpStatusCode.NotFound, string.Format(Schema.Properties.Resources.ResourceNotFoundTemplate, identifier)),
                    HttpStatusCode.Conflict => this.ScimError(HttpStatusCode.Conflict, Schema.Properties.Resources.ExceptionInvalidRequest),
                    _ => this.ScimError(HttpStatusCode.BadRequest, httpResponseException.Message)
                };
            }
            catch (Exception exception)
            {
                if (this.TryGetMonitor(out IMonitor monitor))
                {
                    IExceptionNotification notification =
                        ExceptionNotificationFactory.Instance.CreateNotification(
                            exception,
                            correlationIdentifier,
                            ServiceNotificationIdentifiers.ControllerTemplatePutException);
                    monitor.Report(notification);
                }

                actionResult = this.ScimError(HttpStatusCode.InternalServerError, exception.Message);
            }

            return actionResult;
        }
    }
}