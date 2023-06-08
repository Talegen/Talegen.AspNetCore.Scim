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

namespace Talegen.AspNetCore.Scim.ApiSample
{
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Text;
    using Microsoft.AspNetCore.Mvc.Formatters;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.IdentityModel.Tokens;

    using Talegen.AspNetCore.Scim.Provider;
    using Talegen.AspNetCore.Scim.Service.Monitor;
    using Microsoft.IdentityModel.JsonWebTokens;
    using Microsoft.OpenApi.Models;

    public static class Program
    {
        /// <summary>
        /// Gets or sets an instance of the application configuration.
        /// </summary>
        public static IConfiguration Configuration { get; set; }

        public static IMonitor Monitor { get; set; }

        public static IProvider Provider { get; set; }

        public static void Main(string[] args)
        {
            // load configuration get the connection string information early
            var configurationBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
                .AddEnvironmentVariables();

            // set the configuration instance.
            Configuration = configurationBuilder.Build();

            // load our SCIM related singletons
            Monitor = new ConsoleMonitor(); // monitor rendering to console
            Provider = new SampleProvider(); // use in-memory sample provider

            var builder = WebApplication.CreateBuilder(args);

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                // using System.Reflection;
                var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));

                // add JWT Authentication
                var securityScheme = new OpenApiSecurityScheme
                {
                    Name = "JWT Authentication",
                    Description = "Enter JWT Bearer token **_only_**",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer", // must be lower case
                    BearerFormat = "JWT",
                    Reference = new OpenApiReference
                    {
                        Id = JwtBearerDefaults.AuthenticationScheme,
                        Type = ReferenceType.SecurityScheme
                    }
                };
                options.AddSecurityDefinition(securityScheme.Reference.Id, securityScheme);
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {securityScheme, new string[] { }}
                });
            });

            builder.Services.AddSingleton(typeof(IProvider), Provider);
            builder.Services.AddSingleton(typeof(IMonitor), Monitor);

            builder.Services.AddAuthentication(config =>
                {
                    config.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    config.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    if (builder.Environment.IsDevelopment())
                    {
                        // load our test token validator using developer self-signing certificate
                        options.TokenValidationParameters =
                            new TokenValidationParameters
                            {
                                ValidateIssuer = false,
                                ValidateAudience = false,
                                ValidateLifetime = false,
                                ValidateIssuerSigningKey = false,
                                ValidIssuer = Configuration["Token:TokenIssuer"],
                                ValidAudience = Configuration["Token:TokenAudience"],
                                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Token:IssuerSigningKey"]))
                            };
                    }
                    else
                    {
                        options.Authority = Configuration["Token:TokenIssuer"];
                        options.Audience = Configuration["Token:TokenAudience"];
                    }

                    options.Events = new JwtBearerEvents
                    {
                        OnTokenValidated = context =>
                        {
                            context.Success();
                            if (context.Principal?.Identity?.IsAuthenticated ?? false)
                            {
                                Console.Write("Authenticated");
                            }

                            return Task.CompletedTask;
                        },
                        OnAuthenticationFailed = AuthenticationFailed
                    };
                });

            // add authorization
            builder.Services.AddAuthorization(config =>
            {
            });

            builder.Services.AddControllers(options =>
                {
                    if (!builder.Environment.IsDevelopment())
                    {
                        options.Filters.Add(new RequireHttpsAttribute());
                    }
                })
                .AddNewtonsoftJson(setup =>
                {
                    setup.SerializerSettings.Formatting = builder.Environment.IsDevelopment() ? Newtonsoft.Json.Formatting.Indented : Newtonsoft.Json.Formatting.None;
                    setup.SerializerSettings.DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Utc;
                    setup.SerializerSettings.StringEscapeHandling = Newtonsoft.Json.StringEscapeHandling.EscapeNonAscii;
                    setup.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver();
                    setup.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
                    setup.SerializerSettings.DefaultValueHandling = Newtonsoft.Json.DefaultValueHandling.Include & Newtonsoft.Json.DefaultValueHandling.Populate;
                    setup.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger(options =>
                {
                });
                app.UseSwaggerUI(options =>
                {
                    options.ShowExtensions();
                    options.EnableTryItOutByDefault();
                    options.EnablePersistAuthorization();
                    options.ShowCommonExtensions();
                });
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(
                (IEndpointRouteBuilder endpoints) =>
                {
                    endpoints.MapDefaultControllerRoute();
                });

            app.Run();
        }

        private static Task AuthenticationFailed(AuthenticationFailedContext arg)
        {
            // For debugging purposes only!
            string authenticationExceptionMessage = $"{{AuthenticationFailed: '{arg.Exception.Message}'}}";

            arg.Response.ContentLength = authenticationExceptionMessage.Length;
            arg.Response.Body.WriteAsync(
                Encoding.UTF8.GetBytes(authenticationExceptionMessage),
                0,
                authenticationExceptionMessage.Length);

            return Task.FromException(arg.Exception);
        }
    }
}