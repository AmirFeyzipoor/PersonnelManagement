using Microsoft.AspNetCore.HttpOverrides;
using PersonnelManagement.RestApi.Configs.MigrationConfigs;
using PersonnelManagement.RestApi.Configs.ServiceConfigs;
using PersonnelManagement.RestApi.Middelwares;
using PersonnelManagement.RestApi.Middelwares.CustomExceptionMiddleware;
using PersonnelManagement.UseCases.AdminServices.SeedData.Contracts;

var builder = WebApplication.CreateBuilder(args);

builder.ConfigureServices();

builder.UpdateDataBases();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});
app.UseMiddleware<IpBlockMiddelware>();

app.UseMiddleware<ExceptionMiddleware>(); 

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

SeedDatabase();

app.UseAuthorization();

app.MapControllers();

void SeedDatabase()
{
    using var scope = app!.Services.CreateScope();
    var dbInitializer = scope.ServiceProvider.GetRequiredService<ISeedDataService>();
    dbInitializer.Execute();
}

app.Run();
