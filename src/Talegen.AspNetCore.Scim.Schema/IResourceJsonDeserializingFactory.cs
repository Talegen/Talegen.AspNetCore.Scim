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
    /// <summary>
    /// Interface IResourceJsonDeserializingFactory Extends the <see cref="Talegen.AspNetCore.Scim.Schema.ISchematizedJsonDeserializingFactory{TOutput}" />
    /// </summary>
    /// <typeparam name="TOutput">The type of the t output.</typeparam>
    /// <seealso cref="Talegen.AspNetCore.Scim.Schema.ISchematizedJsonDeserializingFactory{TOutput}" />
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1040:AvoidEmptyInterfaces", Justification = "This interface is a public interface that serves to refine the type of the generic member relative to the base interface")]
    public interface IResourceJsonDeserializingFactory<TOutput> : ISchematizedJsonDeserializingFactory<TOutput> where TOutput : Resource
    {
    }
}