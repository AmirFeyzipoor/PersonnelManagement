using System.Text;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PersonnelManagement.Entities.Identities;
using PersonnelManagement.Infrastructure.Data;
using PersonnelManagement.Infrastructure.Data.Identities;
using PersonnelManagement.RestApi.Configs.ServiceConfigs.ServicesPrerequisites;
using PersonnelManagement.UseCases.AdminServices.IpBlocking;
using PersonnelManagement.UseCases.AdminServices.IpBlocking.Configs;
using PersonnelManagement.UseCases.AdminServices.IpBlocking.Contracts;
using PersonnelManagement.UseCases.AdminServices.SeedData;
using PersonnelManagement.UseCases.AdminServices.SeedData.Configs;
using PersonnelManagement.UseCases.AdminServices.SeedData.Contracts;
using PersonnelManagement.UseCases.Infrastructure;
using PersonnelManagement.UseCases.Infrastructure.SortUtilities;
using PersonnelManagement.UseCases.Infrastructure.TokenManager;
using PersonnelManagement.UseCases.Infrastructure.TokenManager.Contracts;
using PersonnelManagement.UseCases.Notifications;
using PersonnelManagement.UseCases.Notifications.EmailServices;
using PersonnelManagement.UseCases.Notifications.EmailServices.Configs;
using PersonnelManagement.UseCases.Notifications.EmailServices.Contracts;
using PersonnelManagement.UseCases.Notifications.SmsServices;
using PersonnelManagement.UseCases.Notifications.SmsServices.Configs;
using PersonnelManagement.UseCases.Notifications.SmsServices.Contracts;
using PersonnelManagement.UseCases.Personnel;
using PersonnelManagement.UseCases.Personnel.Contracts;
using PersonnelManagement.UseCases.Personnel.Contracts.Configs;

namespace PersonnelManagement.RestApi.Configs.ServiceConfigs;

public static class ServicesConfig
{
    private static readonly ConnectionStrings _dbConnectionString = new();
    private static readonly SeedDataConfigs _seedDataConfigs = new();
    private static readonly JwtBearerTokenSettings _jwtBearerTokenSettings = new();
    private static readonly BlockedIpOptions _blockedIpOptions = new (); 
    private static readonly SmsSettings _smsSettings = new (); 
    private static readonly SmtpSettings _smtpSettings = new();

    private static void Initialized(WebApplicationBuilder builder)
    {
        builder.Configuration.Bind("ConnectionStrings", _dbConnectionString);
        builder.Configuration.Bind("SeedDataConfigs", _seedDataConfigs);
        builder.Configuration.Bind("JwtBearerTokenSettings", _jwtBearerTokenSettings);
        builder.Configuration.Bind("BlockedIpOptions", _blockedIpOptions);
        builder.Configuration.Bind("SmsSettings", _smsSettings);
        builder.Configuration.Bind("SmtpSettings", _smtpSettings);
    }

    public static void ConfigureServices(this WebApplicationBuilder builder)
    {
        Initialized(builder);

        builder.Services.AddIdentity<User, Role>(options =>
            {
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 4;
                options.Password.RequireDigit = false;
                options.Lockout.AllowedForNewUsers = false;
            })
            .AddEntityFrameworkStores<DataContext>()
            .AddDefaultTokenProviders();
        
        builder.Services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Example: \"Bearer token\""
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] { }
                }
            });
        });
        
        builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidAudience = _jwtBearerTokenSettings.Audience,
                    ValidIssuer = _jwtBearerTokenSettings.Issuer,
                    IssuerSigningKey =
                        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtBearerTokenSettings.SecretKey))
                };
            });

        builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());
        builder.Host.ConfigureContainer<ContainerBuilder>(b => b
            .RegisterModule(new AutoFacModule()));
    }

    public class AutoFacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<DataContext>()
                .WithParameter("connectionString", _dbConnectionString.SqlDb)
                .AsSelf()
                .InstancePerLifetimeScope();
            
            builder.RegisterType<DapperDataContext>()
                .WithParameter("connectionString", _dbConnectionString.SqlDb)
                .AsSelf()
                .SingleInstance();
            
            builder.RegisterAssemblyTypes(
                    typeof(NotificationService).Assembly)
                .AssignableTo<Service>()
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();
            
            builder.RegisterType<UnitOfWork>()
                .As<IUnitOfWork>()
                .InstancePerLifetimeScope();

            builder.RegisterType<SeedDataService>()
                .WithParameter("seedDataConfigs", _seedDataConfigs)
                .As<ISeedDataService>()
                .SingleInstance();
            
            builder.RegisterType<TokenManagerService>()
                .WithParameter("jwtBearerTokenSettings",
                    _jwtBearerTokenSettings)
                .As<ITokenManagerService>()
                .InstancePerLifetimeScope();
            
            builder.RegisterType<UriSortParser>()
                .AsSelf()
                .InstancePerLifetimeScope();
            
            builder.RegisterType<IpBlockingService>()
                .WithParameter("blockedIps", _blockedIpOptions.BlockedIps)
                .As<IIpBlockingService>()
                .InstancePerDependency();

            builder.RegisterType<KavenegarSmsService>()
                .WithParameter("smsSettings", _smsSettings)
                .As<ISmsService>()
                .InstancePerLifetimeScope();
            
            builder.RegisterType<EmailService>()
                .WithParameter("smtpSettings", _smtpSettings)
                .As<IEmailService>()
                .InstancePerLifetimeScope();
            
            builder.RegisterAssemblyTypes(
                    typeof(DapperPersonnelRepository).Assembly)
                .AssignableTo<Repository>()
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();
        }
    }
}