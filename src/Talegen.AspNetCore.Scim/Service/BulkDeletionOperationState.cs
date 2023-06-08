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
    using System.Net;
    using Protocol;

    /// <summary>
    /// This class implements a bulk deletion operation state.
    /// </summary>
    internal class BulkDeletionOperationState : BulkOperationStateBase<IResourceIdentifier>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BulkDeletionOperationState" /> class.
        /// </summary>
        /// <param name="request">Contains a request.</param>
        /// <param name="operation">Contains an operation.</param>
        /// <param name="context">Contains a context.</param>
        public BulkDeletionOperationState(IRequest<BulkRequest2> request, BulkRequestOperation operation, IBulkOperationContext<IResourceIdentifier> context)
            : base(request, operation, context)
        {
        }

        /// <summary>
        /// This method is used to try and prepare the request.
        /// </summary>
        /// <param name="request">Contains the request.</param>
        /// <returns>Returns a value indicating whether the request was prepared.</returns>
        public override bool TryPrepareRequest(out IRequest<IResourceIdentifier> request)
        {
            request = null;
            bool result = true;

            Uri absoluteResourceIdentifier = new Uri(this.BulkRequest.BaseResourceIdentifier, this.Operation.Path);

            if (!UniformResourceIdentifier.TryParse(absoluteResourceIdentifier, this.BulkRequest.Extensions, out IUniformResourceIdentifier resourceIdentifier))
            {
                this.Context.State = this;

                ErrorResponse error = new ErrorResponse
                {
                    Status = HttpStatusCode.BadRequest,
                    ErrorType = ErrorType.invalidPath
                };

                BulkResponseOperation response = new BulkResponseOperation(this.Operation.Identifier)
                {
                    Response = error,
                    Method = this.Operation.Method,
                    Status = HttpStatusCode.BadRequest
                };

                response.Method = this.Operation.Method;

                this.Complete(response);
                result = false;
            }
            else
            {
                request = new DeletionRequest(this.BulkRequest.Request, resourceIdentifier.Identifier, this.BulkRequest.CorrelationIdentifier, this.BulkRequest.Extensions);
            }

            return result;
        }
    }
}