using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using PersonnelManagement.Entities.Identities;
using PersonnelManagement.Infrastructure.Data;
using PersonnelManagement.RestApi.Configs.ServiceConfigs.ServicesPrerequisites;
using PersonnelManagement.UseCases.AdminServices.SeedData;
using PersonnelManagement.UseCases.AdminServices.SeedData.Contracts;
using PersonnelManagement.UseCases.Configurations;
using PersonnelManagement.UseCases.Infrastructure;

namespace PersonnelManagement.RestApi.Configs.ServiceConfigs;

public static class ServicesConfig
{
    private static readonly ConnectionStrings _dbConnectionString = new();
    private static readonly SeedDataConfigs _seedDataConfigs = new();

    private static void Initialized(WebApplicationBuilder builder)
    {
        builder.Configuration.Bind("ConnectionStrings", _dbConnectionString);
        builder.Configuration.Bind("SeedDataConfigs", _seedDataConfigs);
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
            
            builder.RegisterType<UnitOfWork>()
                .As<IUnitOfWork>()
                .InstancePerLifetimeScope();

            builder.RegisterType<SeedDataService>()
                .WithParameter("seedDataConfigs", _seedDataConfigs)
                .As<ISeedDataService>()
                .SingleInstance();
        }
    }
}