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

namespace Talegen.AspNetCore.Scim.ApiSample.Controllers
{
    using System.IdentityModel.Tokens.Jwt;
    using System.Text;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.IdentityModel.Tokens;

    [Route("scim/token")]
    [ApiController]
    public class TokenController : Controller
    {
        private readonly IConfiguration configuration;
        private const int defaultTokenExpirationTimeInMins = 120;

        public TokenController(IConfiguration Configuration)
        {
            this.configuration = Configuration;
        }

        private string GenerateJSONWebToken()
        {
            SymmetricSecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.configuration["Token:IssuerSigningKey"]));
            SigningCredentials credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            DateTime startTime = DateTime.UtcNow;
            DateTime expiryTime;

            if (double.TryParse(this.configuration["Token:TokenLifetimeInMins"], out double tokenExpiration))
                expiryTime = startTime.AddMinutes(tokenExpiration);
            else
                expiryTime = startTime.AddMinutes(defaultTokenExpirationTimeInMins);

            JwtSecurityToken token =
                new JwtSecurityToken(
                    this.configuration["Token:TokenIssuer"],
                    this.configuration["Token:TokenAudience"],
                    null,
                    notBefore: startTime,
                    expires: expiryTime,
                    signingCredentials: credentials);

            string result = new JwtSecurityTokenHandler().WriteToken(token);
            return result;
        }

        [HttpGet]
        public ActionResult Get()
        {
            string tokenString = this.GenerateJSONWebToken();
            return this.Ok(new { token = tokenString });
        }
    }
}