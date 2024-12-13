
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

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
                   throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
            var azureConnectionString = Environment.GetEnvironmentVariable("AZURE_BLOB_CONNECTION_STRING") ??
                   throw new InvalidOperationException("Connection string 'AZURE_BLOB_CONNECTION_STRING' not found.");

            var migrationsAssembly = typeof(Program).GetTypeInfo().Assembly.GetName().Name;

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(connectionString, b => b.MigrationsAssembly(migrationsAssembly));
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

            // DAOs
            builder.Services.AddScoped<UserAccountDAO>();
            builder.Services.AddScoped<UserRoleDAO>();
            builder.Services.AddScoped<SupplierCategoriesDAO>();
            builder.Services.AddScoped<RequestPricingDAO>();
            builder.Services.AddScoped<SupplierDAO>();
            builder.Services.AddScoped<SupplierImageDAO>();
            builder.Services.AddScoped<SubscriptionPackageDAO>();

        
            //Repositories
            builder.Services.AddScoped<IUserAccountRepository, UserAccountRepository>();
            builder.Services.AddScoped<IUserRoleRepository, UserRoleRepository>();
            builder.Services.AddScoped<ISupplierRepository, SupplierRepository>();
            builder.Services.AddScoped<ISupplierImageRepository, SupplierImageRepository>();
            builder.Services.AddScoped<IRequestPricingsRepository, RequestPricingsRepository>();
            builder.Services.AddScoped<ISupplierCategoryRepository, SupplierCategoryRepository>();
            builder.Services.AddScoped<ISubscriptionPackageRepository, SubscriptionPackageRepository>();


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


            // Auth Handlers
            builder.Services.AddSingleton<IAuthorizationHandler, ViewUserAuthorizationHandler>();
            builder.Services.AddSingleton<IAuthorizationHandler, ManageUserAuthorizationHandler>();
            builder.Services.AddSingleton<IAuthorizationHandler, ViewRoleAuthorizationHandler>();
            builder.Services.AddSingleton<IAuthorizationHandler, AssignRolesAuthorizationHandler>();

            //File Logger
            builder.Logging.AddFile(builder.Configuration.GetSection("Logging"));

            //Email Templates
            EmailTemplates.Initialize(builder.Environment);


            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();

            var app = builder.Build();
 
            // Configure the HTTP request pipeline.
            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseExceptionHandler(opt => { });

            if (app.Environment.IsDevelopment())
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

            app.UseHttpsRedirection();

            app.UseCors(builder => builder
               .AllowAnyOrigin()
               .AllowAnyHeader()
               .AllowAnyMethod());

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

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
