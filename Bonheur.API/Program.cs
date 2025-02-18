        
using Bonheur.API.Configurations;
using Bonheur.BusinessObjects.Entities;
using Bonheur.DAOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using OpenIddict.Validation.AspNetCore;
using Quartz;
using static OpenIddict.Abstractions.OpenIddictConstants;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using Bonheur.API.Authorization;
using Microsoft.IdentityModel.Logging;
using Bonheur.API.Middlewares;
using Bonheur.Repositories.Interfaces;
using Bonheur.Repositories;
using Bonheur.Services.Interfaces;
using Bonheur.Services;
using System.Text.Json.Serialization;
using Bonheur.Services.Mappers;
using Microsoft.AspNetCore.Mvc.Versioning;
using Bonheur.Utils;
using Bonheur.API.Authorization.Requirements;
using Bonheur.BusinessObjects.Models;
using Microsoft.AspNetCore.Authorization;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Bonheur.Services.Email;
using Microsoft.Extensions.Azure;
using Net.payOS;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Bonheur.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
           
            var builder = WebApplication.CreateBuilder(args);

            // Load environment variables from .env file
            Env.Load();

            // Load SMTP configuration from environment variables
            var smtpConfig = SmtpConfig.LoadFromEnvironment();

            // Add services to the container.

            var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING") ??
                   throw new InvalidOperationException("Connection string 'DB_CONNECTION_STRING' not found.");
            var azureConnectionString = Environment.GetEnvironmentVariable("AZURE_BLOB_CONNECTION_STRING") ??
                   throw new InvalidOperationException("Connection string 'AZURE_BLOB_CONNECTION_STRING' not found.");

            var migrationsAssembly = typeof(Program).GetTypeInfo().Assembly.GetName().Name;

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseNpgsql(connectionString, b => b.MigrationsAssembly(migrationsAssembly));
                options.UseOpenIddict();
            });

            // Add Azure Blob Storage
            builder.Services.AddAzureClients(azureBuilder =>
            {
                azureBuilder.AddBlobServiceClient(azureConnectionString);
            });

            // Add Identity
            builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            // Configure Identity options and password complexity here
            builder.Services.Configure<IdentityOptions>(options =>
            {
                // User settings
                options.User.RequireUniqueEmail = true;

                // Password settings
                /*
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = false;

                // Lockout settings
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
                options.Lockout.MaxFailedAccessAttempts = 10;
                */

                // Configure Identity to use the same JWT claims as OpenIddict
                options.ClaimsIdentity.UserNameClaimType = Claims.Name;
                options.ClaimsIdentity.UserIdClaimType = Claims.Subject;
                options.ClaimsIdentity.RoleClaimType = Claims.Role;
                options.ClaimsIdentity.EmailClaimType = Claims.Email;
            });

            // Configure OpenIddict periodic pruning of orphaned authorizations/tokens from the database.
            builder.Services.AddQuartz(options =>
            {
                options.UseSimpleTypeLoader();
                options.UseInMemoryStore();
            });

            // Register the Quartz.NET service and configure it to block shutdown until jobs are complete.
            builder.Services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);

            builder.Services.AddOpenIddict()
                .AddCore(options =>
                {
                    options.UseEntityFrameworkCore()
                           .UseDbContext<ApplicationDbContext>();

                    options.UseQuartz();
                })
                .AddServer(options =>
                {
                    options.SetTokenEndpointUris("connect/token");

                    options.AllowPasswordFlow()
                           .AllowRefreshTokenFlow()
                           .AllowCustomFlow(Constants.GrantTypes.ASSERTION);

                    options.RegisterScopes(
                        Scopes.Profile,
                        Scopes.Email,
                        Scopes.Address,
                        Scopes.Phone,
                        Scopes.Roles);

                    if (builder.Environment.IsDevelopment())
                    {
                        options.AddDevelopmentEncryptionCertificate()
                               .AddDevelopmentSigningCertificate();

                        options.UseAspNetCore().DisableTransportSecurityRequirement();

                    }
                    else
                    {
                        var oidcCertFileName = builder.Configuration["OIDC:Certificates:Path"];
                        var oidcCertFilePassword = builder.Configuration["OIDC:Certificates:Password"];

                        if (string.IsNullOrWhiteSpace(oidcCertFileName))
                        {
                            // You must configure persisted keys for Encryption and Signing.
                            // See https://documentation.openiddict.com/configuration/encryption-and-signing-credentials.html
                            options.AddEphemeralEncryptionKey()
                                   .AddEphemeralSigningKey();

                            options.UseAspNetCore().DisableTransportSecurityRequirement();
                        }
                        else
                        {
                            var oidcCertificate = new X509Certificate2(oidcCertFileName, oidcCertFilePassword);

                            options.AddEncryptionCertificate(oidcCertificate)
                                   .AddSigningCertificate(oidcCertificate);
                        }
                    }

                    options.UseAspNetCore()
                           .EnableTokenEndpointPassthrough();

                })
                .AddValidation(options =>
                {
                    options.UseLocalServer();
                    options.UseAspNetCore();
                });


            builder.Services.AddAuthentication(o =>
            {
                o.DefaultScheme = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
                o.DefaultAuthenticateScheme = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
                o.DefaultChallengeScheme = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
            })
            //.AddJwtBearer(options =>
            //{
            //    // Configure the Authority to the expected value for
            //    // the authentication provider. This ensures the token
            //    // is appropriately validated.
            //    options.Authority = "Authority URL"; // TODO: Update URL
            //    options.RequireHttpsMetadata = false;

            //    // We have to hook the OnMessageReceived event in order to
            //    // allow the JWT authentication handler to read the access
            //    // token from the query string when a WebSocket or 
            //    // Server-Sent Events request comes in.

            //    // Sending the access token in the query string is required when using WebSockets or ServerSentEvents
            //    // due to a limitation in Browser APIs. We restrict it to only calls to the
            //    // SignalR hub in this code.
            //    // See https://docs.microsoft.com/aspnet/core/signalr/security#access-token-logging
            //    // for more information about security considerations when using
            //    // the query string to transmit the access token.
            //    options.Events = new JwtBearerEvents
            //    {
            //        OnMessageReceived = context =>
            //        {
            //            var accessToken = context.Request.Query["access_token"];

            //            // If the request is for our hub...
            //            var path = context.HttpContext.Request.Path;
            //            if (!string.IsNullOrEmpty(accessToken) &&
            //                (path.StartsWithSegments("/hubs/chat")))
            //            {
            //                // Read the token out of the query string
            //                context.Token = accessToken;
            //            }
            //            return Task.CompletedTask;
            //        }
            //    };
            //});
            .AddCookie()
            .AddOpenIdConnect(options =>
            {
                options.Authority = "https://localhost:7175";
                options.ClientId = OidcServerConfig.BonheurAppClientID;
                options.SaveTokens = true;
                options.ResponseType = "code";
                options.Scope.Add("openid");
                options.Scope.Add("profile");
                options.Scope.Add("email");

                // Lấy access_token từ query string khi cần
                options.Events.OnMessageReceived = context =>
                {
                    var accessToken = context.Request.Query["access_token"].ToString();
                    if (!string.IsNullOrEmpty(accessToken))
                    {
                        // Thêm token vào header Authorization
                        Console.WriteLine("Access Token: " + accessToken);
                        context.HttpContext.Request.Headers["Authorization"] = "Bearer " + accessToken;
                        context.Token = accessToken;
                    }
                    return Task.CompletedTask;
                };

                options.Events.OnTokenValidated = context =>
                {
                    var token = context.SecurityToken as JwtSecurityToken;
                    var userId = token?.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;

                    Console.WriteLine("Token validated: " + userId);

                    if (userId != null)
                    {
                        var claimsIdentity = (ClaimsIdentity)context.Principal.Identity;
                        claimsIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, userId));
                    }

                    return Task.CompletedTask;
                };
            });

            // Add Authorization
            builder.Services.AddAuthorizationBuilder()
                .AddPolicy(AuthPolicies.ViewAllUsersPolicy,
                    policy => policy.RequireClaim(CustomClaims.Permission, ApplicationPermissions.ViewUsers))
                .AddPolicy(AuthPolicies.ManageAllUsersPolicy,
                    policy => policy.RequireClaim(CustomClaims.Permission, ApplicationPermissions.ManageUsers))
                .AddPolicy(AuthPolicies.ViewAllRolesPolicy,
                    policy => policy.RequireClaim(CustomClaims.Permission, ApplicationPermissions.ViewRoles))
                .AddPolicy(AuthPolicies.ViewRoleByRoleNamePolicy,
                    policy => policy.Requirements.Add(new ViewRoleAuthorizationRequirement()))
                .AddPolicy(AuthPolicies.ManageAllRolesPolicy,
                    policy => policy.RequireClaim(CustomClaims.Permission, ApplicationPermissions.ManageRoles))
                .AddPolicy(AuthPolicies.AssignAllowedRolesPolicy,
                    policy => policy.Requirements.Add(new AssignRolesAuthorizationRequirement()));

            // Add cors
            builder.Services.AddCors();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigin",
                    builder => builder
                        .WithOrigins("http://localhost:4200")  
                        .AllowCredentials()                  
                        .AllowAnyHeader()                      
                        .AllowAnyMethod());                  
            });

            // Add controllers
            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });

            // Add API versioning
            builder.Services.AddApiVersioning(options =>
            {
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
                options.ReportApiVersions = true;
                options.ApiVersionReader =  ApiVersionReader.Combine(
                    new QueryStringApiVersionReader("api-version"),
                    new HeaderApiVersionReader("x-version"),
                    new MediaTypeApiVersionReader("version")
                    );
            });

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();

            // Add Swagger
            builder.Services.AddVersionedApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV"; 
                options.SubstituteApiVersionInUrl = true; 
            });

            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = OidcServerConfig.ServerName, Version = "v1" });
                c.OperationFilter<SwaggerAuthorizeOperationFilter>();
                c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.OAuth2,
                    Flows = new OpenApiOAuthFlows
                    {
                        Password = new OpenApiOAuthFlow
                        {
                            TokenUrl = new Uri("/connect/token", UriKind.Relative)
                        }
                    }
                });
            });

            // Mapper
            builder.Services.AddAutoMapper(typeof(MappingProfiles));

            // Configurations
            //builder.Services.Configure<AppSettings>(builder.Configuration);

            // Exception Handler
            builder.Services.AddExceptionHandler<ExceptionHandler>();
            builder.Services.AddProblemDetails();

            //PayOs
            var payOsClientId = Environment.GetEnvironmentVariable("PAYOS_CLIENT_ID") ??
                 throw new InvalidOperationException("Environement string 'PAYOS_CLIENT_ID' not found.");

            var payOsApiKey = Environment.GetEnvironmentVariable("PAYOS_API_KEY") ??
                throw new InvalidOperationException("Environement string 'PAYOS_API_KEY' not found.");

            var payOsCheckSumKey = Environment.GetEnvironmentVariable("PAYOS_CHECKSUM_KEY") ??
                throw new InvalidOperationException("Environement string 'PAYOS_CHECKSUM_KEY' not found.");

            PayOS payOS = new PayOS(payOsClientId, payOsApiKey, payOsCheckSumKey);

            builder.Services.AddSingleton(payOS);

            // DAOs
            builder.Services.AddScoped<UserAccountDAO>();
            builder.Services.AddScoped<UserRoleDAO>();
            builder.Services.AddScoped<SupplierCategoriesDAO>();
            builder.Services.AddScoped<RequestPricingDAO>();
            builder.Services.AddScoped<SupplierDAO>();
            builder.Services.AddScoped<SupplierImageDAO>();
            builder.Services.AddScoped<SubscriptionPackageDAO>();
            builder.Services.AddScoped<ReviewDAO>();
            builder.Services.AddScoped<AdPackageDAO>();
            builder.Services.AddScoped<AdvertisementDAO>();
            builder.Services.AddScoped<FavoriteSupplierDAO>();
            builder.Services.AddScoped<OrderDAO>();
            builder.Services.AddScoped<InvoiceDAO>();
            builder.Services.AddScoped<OrderDetailDAO>();
            builder.Services.AddScoped<SupplierSocialNetworkDAO>();
            builder.Services.AddScoped<SocialNetworkDAO>();
            builder.Services.AddScoped<SupplierFAQDAO>();

            //Repositories
            builder.Services.AddScoped<IUserAccountRepository, UserAccountRepository>();
            builder.Services.AddScoped<IUserRoleRepository, UserRoleRepository>();
            builder.Services.AddScoped<ISupplierRepository, SupplierRepository>();
            builder.Services.AddScoped<ISupplierImageRepository, SupplierImageRepository>();
            builder.Services.AddScoped<IRequestPricingsRepository, RequestPricingsRepository>();
            builder.Services.AddScoped<ISupplierCategoryRepository, SupplierCategoryRepository>();
            builder.Services.AddScoped<ISubscriptionPackageRepository, SubscriptionPackageRepository>();
            builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
            builder.Services.AddScoped<IAdPackageRepository, AdPackageRepository>();
            builder.Services.AddScoped<IAdvertisementRepository, AdvertisementRepository>();
            builder.Services.AddScoped<IFavoriteSupplierRepository, FavoriteSupplierRepository>();
            builder.Services.AddScoped<IOrderRepository, OrderRepository>();
            builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>();
            builder.Services.AddScoped<IOrderDetailRepository, OrderDetailRepository>();
            builder.Services.AddScoped<ISupplierSocialNetworkRepository, SupplierSocialNetworkRepository>();
            builder.Services.AddScoped<ISocialNetworkRepository, SocialNetworkRepository>();
            builder.Services.AddScoped<ISupplierFAQRepository, SupplierFAQRepository>();

            // Services
            builder.Services.AddScoped<IUserAccountService, UserAccountService>();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IUserRoleService, UserRoleService>();
            builder.Services.AddScoped<IEmailSender, EmailSender>();
            builder.Services.AddScoped<ISupplierService, SupplierService>();
            builder.Services.AddScoped<IStorageService, StorageService>();
            builder.Services.AddScoped<IRequestPricingsService, RequestPricingsService>();
            builder.Services.AddScoped<ISupplierCategoryService, SupplierCategoryService>();
            builder.Services.AddScoped<ISubscriptionPackageService, SubscriptionPackageService>();
            builder.Services.AddScoped<IReviewService, ReviewService>();
            builder.Services.AddScoped<IAdPackageService,AdPackageService>();   
            builder.Services.AddScoped<IAdvertisementService, AdvertisementService>();
            //builder.Services.AddScoped<IChatHubService, ChatHubService>();
            builder.Services.AddScoped<IFavoriteSupplierService, FavoriteSupplierService>();
            builder.Services.AddScoped<IPaymentService, PaymentService>();
            builder.Services.AddScoped<IInvoiceService, InvoiceService>();
            builder.Services.AddScoped<ISupplierSocialNetworkService, SupplierSocialNetworkService>();
            builder.Services.AddScoped<ISocialNetworkService, SocialNetworkService>();
            builder.Services.AddScoped<ISupplierFAQService, SupplierFAQService>();
            builder.Services.AddScoped<IOrderService, OrderService>();
            builder.Services.AddScoped<IPlaceService, PlaceService>();

            // Auth Handlers
            builder.Services.AddSingleton<IAuthorizationHandler, ViewUserAuthorizationHandler>();
            builder.Services.AddSingleton<IAuthorizationHandler, ManageUserAuthorizationHandler>();
            builder.Services.AddSingleton<IAuthorizationHandler, ViewRoleAuthorizationHandler>();
            builder.Services.AddSingleton<IAuthorizationHandler, AssignRolesAuthorizationHandler>();

            // HttpClient
            builder.Services.AddHttpClient();

            //File Logger
            builder.Logging.AddFile(builder.Configuration.GetSection("Logging"));

            //Email Templates
            EmailTemplates.Initialize(builder.Environment);

            //SignalR
            builder.Services.AddSignalR();


            //Rate Limiter
            builder.Services.AddRateLimiter(options =>
            {
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

                //global
                options.AddPolicy("global", httpContext =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: httpContext.Connection.RemoteIpAddress?.ToString(),
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                        PermitLimit = 100,
                        Window = TimeSpan.FromMinutes(10),
                        QueueLimit = 10
                        }));
                // /connect/token
                options.AddPolicy("5_5", httpContext =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: httpContext.Connection.RemoteIpAddress?.ToString(),
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 5,
                            Window = TimeSpan.FromMinutes(5)
                        }));
            });

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();


            var app = builder.Build();
 
            // Configure the HTTP request pipeline.
            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseExceptionHandler(opt => { });

            if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.DocumentTitle = "Swagger UI - Bonheur";
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", $"{OidcServerConfig.ServerName} V1");
                    c.OAuthClientId(OidcServerConfig.SwaggerClientID);
                });

                IdentityModelEventSource.ShowPII = true;
            }
            else
            {
                // The default HSTS value is 30 days.
                // You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            //app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors("AllowSpecificOrigin");

            app.UseCors(builder => builder
               .AllowAnyOrigin()
               .AllowAnyHeader()
               .AllowAnyMethod());

            app.UseRateLimiter();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();


            app.MapHub<ChatHubService>("/hubs/chat");

            app.MapFallbackToFile("/index.html");

            // Configure HttpContextAccessor for Utilities
            var httpContextAccessor = app.Services.GetRequiredService<IHttpContextAccessor>();
            Utilities.Configure(httpContextAccessor);

            /************* SEED DATABASE *************/

            using var scope = app.Services.CreateScope();
            try
            {
                await OidcServerConfig.RegisterClientApplicationsAsync(scope.ServiceProvider);
            }
            catch (Exception ex)
            {
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
                logger.LogCritical(ex, "An error occured whilst creating/seeding database");

                throw;
            }

            /************* RUN APP *************/
            app.Run();
        }
    }
}
