using BullMarket.Application;
using BullMarket.Domain.Entities;
using BullMarket.Infrastructure;
using BullMarket.Infrastructure.Persistence;
using BullMarket.Infrastructure.Services;
using BullMarket.WebUI.Helpers;
using BullMarket.WebUI.Hubs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BullMarket.WebUI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            ConfigureIdentityServerAuthentication(services);
            ConfigureKeycloakAuthentication(services);

            services.AddAuthorization(options => {
                options.DefaultPolicy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .AddAuthenticationSchemes("BullMarket")
                    .Build();

                options.AddPolicy("CanAccessDetailedView", policyBuilder =>
                {
                    policyBuilder.RequireAuthenticatedUser()
                        .AddAuthenticationSchemes("BullMarket")
                        .RequireAssertion(context =>
                        {
                            if (AuthorizationHelpers.RetrieveClientRoles(context.User.Claims).TryGetValue("BullMarketApi", out var role))
                            {
                                return role.Contains("single-stock-view");
                            }

                            return false;
                        })
                       .Build();
                });

                options.AddPolicy("CanInitiateOpenIdConfigurationRefresh", policyBuilder =>
                {
                    policyBuilder.RequireAuthenticatedUser()
                        .AddAuthenticationSchemes("BullMarket")
                        .RequireAssertion(context =>
                        {
                            if (AuthorizationHelpers.RetrieveClientRoles(context.User.Claims).TryGetValue("BullMarketApi", out var role))
                            {
                                return role.Contains("jwks-administrator");
                            }

                            return false;
                        })
                       .Build();
                });
            });

            services.AddCors(options =>
            {
                options.AddPolicy("DevPolicy", builder =>
                {
                    builder.WithOrigins("http://localhost:4200");
                    builder.AllowAnyMethod();
                    builder.AllowAnyHeader();
                    builder.AllowCredentials();
                });
            });

            services
                .AddIdentity<ApplicationUser, IdentityRole<int>>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddCors();
            services.AddApplication();
            services.AddInfrastructure(Configuration);
            services.AddHostedService<MigratorService>();
            services.AddHostedService<StreamingService<StockHub>>();
            services.AddControllers();
            services.AddSignalR();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors("DevPolicy");

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<StockHub>("/hubs/stocks")
                    .RequireCors("DevPolicy");
            });
        }

        private void ConfigureIdentityServerAuthentication(IServiceCollection services)
        {
            services
                .AddAuthentication(opt =>
                {
                    opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    opt.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = false,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("0123456789ImportantSecret"))
                    };

                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var accessToken = context.Request.Query["access_token"];

                            // If the request is for our hub...
                            var path = context.HttpContext.Request.Path;
                            if (!string.IsNullOrEmpty(accessToken) &&
                                (path.StartsWithSegments("/hubs/stocks")))
                            {
                                // Read the token out of the query string
                                context.Token = accessToken;
                            }
                            return Task.CompletedTask;
                        }
                    };
                });
        }

        private void ConfigureKeycloakAuthentication(IServiceCollection services)
        {
            var authenticationBuilder = services
                .AddAuthentication();

            var realms = Configuration
                .GetSection(nameof(JwtBearerOptions))?
                .GetChildren()
                .ToArray();

            foreach (var realm in realms)
            {
                authenticationBuilder.AddJwtBearer(realm.Key, c =>
                {
                    realm.Bind(c);

                    c.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var user = context.HttpContext.User;
                            var headers = context.HttpContext.Request.Headers;
                            var accessToken = context.Request.Query["access_token"];

                            // If the request is for our hub...
                            var path = context.HttpContext.Request.Path;
                            if (!string.IsNullOrEmpty(accessToken) &&
                                (path.StartsWithSegments("/hubs/stocks")))
                            {
                                // Read the token out of the query string
                                context.Token = accessToken;
                            }
                            return Task.CompletedTask;
                        },
                        OnTokenValidated = context =>
                        {
                            var user = context.HttpContext.User;
                            var headers = context.HttpContext.Request.Headers;
                            var secToken = context.SecurityToken;
                            return Task.CompletedTask;
                        },
                        OnForbidden = context =>
                        {
                            var user = context.HttpContext.User;
                            var headers = context.HttpContext.Request.Headers;
                            return Task.CompletedTask;
                        },
                        OnAuthenticationFailed = context =>
                        {
                            var token = context.HttpContext.User;
                            var headers = context.HttpContext.Request.Headers;
                            var reason = context.Exception.Message;
                            return Task.CompletedTask;
                        }
                    };
                });
            }
        }
    }
}
