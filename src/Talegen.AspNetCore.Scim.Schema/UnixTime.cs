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
    using System.Globalization;

    // Refer to https://en.wikipedia.org/wiki/Unix_time
    /// <summary>
    /// Class UnixTime. Implements the <see cref="Talegen.AspNetCore.Scim.Schema.IUnixTime" />
    /// </summary>
    /// <seealso cref="Talegen.AspNetCore.Scim.Schema.IUnixTime" />
    public class UnixTime : IUnixTime
    {
        /// <summary>
        /// The epoch
        /// </summary>
        public static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// Initializes a new instance of the <see cref="UnixTime" /> class.
        /// </summary>
        /// <param name="epochTimestamp">The epoch timestamp.</param>
        public UnixTime(long epochTimestamp)
        {
            this.EpochTimestamp = epochTimestamp;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnixTime" /> class.
        /// </summary>
        /// <param name="epochTimestamp">The epoch timestamp.</param>
        public UnixTime(int epochTimestamp)
            : this(Convert.ToInt64(epochTimestamp))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnixTime" /> class.
        /// </summary>
        /// <param name="epochTimestamp">The epoch timestamp.</param>
        public UnixTime(double epochTimestamp)
            : this(Convert.ToInt64(epochTimestamp))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnixTime" /> class.
        /// </summary>
        /// <param name="sinceEpoch">The since epoch.</param>
        public UnixTime(TimeSpan sinceEpoch)
            : this(sinceEpoch.TotalSeconds)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnixTime" /> class.
        /// </summary>
        /// <param name="dateTime">The date time.</param>
        public UnixTime(DateTime dateTime)
            : this(dateTime.ToUniversalTime().Subtract(Epoch))
        {
        }

        /// <summary>
        /// Converts to universal time.
        /// </summary>
        /// <returns>DateTime.</returns>
        public DateTime ToUniversalTime()
        {
            DateTime result = Epoch.AddSeconds(this.EpochTimestamp);
            return result;
        }

        /// <summary>
        /// Gets the epoch timestamp.
        /// </summary>
        /// <value>The epoch timestamp.</value>
        public long EpochTimestamp
        {
            get;
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            string result = this.EpochTimestamp.ToString(CultureInfo.InvariantCulture);
            return result;
        }
    }
}