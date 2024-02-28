using PersonnelManagement.UseCases.Infrastructure;

namespace PersonnelManagement.UseCases.AdminServices.SeedData.Contracts;

public interface ISeedDataService : Service
{
    public Task Execute();
}