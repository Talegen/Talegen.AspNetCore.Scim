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
    using System.Threading;
    using Schema;

    /// <summary>
    /// This class implements a base abstraction for a SCIM protocol deserialization factory.
    /// </summary>
    public abstract class ProtocolJsonDeserializingFactory : ProtocolJsonDeserializingFactory<Schematized>
    {
    }

    /// <summary>
    /// This class implements a base abstraction for a SCIM protocol deserialization factory for type <typeparamref name="T" />.
    /// </summary>
    /// <typeparam name="T">Contains the type.</typeparam>
    public abstract class ProtocolJsonDeserializingFactory<T> : JsonDeserializingFactory<T>
    {
        /// <summary>
        /// Contains the normalizer.
        /// </summary>
        private IJsonNormalizationBehavior jsonNormalizer;

        /// <summary>
        /// Gets the normalizer.
        /// </summary>
        public override IJsonNormalizationBehavior JsonNormalizer => LazyInitializer.EnsureInitialized<IJsonNormalizationBehavior>(ref this.jsonNormalizer, () => new ProtocolJsonNormalizer());
    }
}