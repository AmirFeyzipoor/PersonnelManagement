using Microsoft.AspNetCore.Identity;
using PersonnelManagement.Entities.Identities;
using PersonnelManagement.UseCases.AdminServices.SeedData.Configs;
using PersonnelManagement.UseCases.AdminServices.SeedData.Contracts;
using PersonnelManagement.UseCases.Infrastructure;

namespace PersonnelManagement.UseCases.AdminServices.SeedData;

public class SeedDataService : ISeedDataService
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<Role> _roleManager;
    private readonly IUnitOfWork _unitOfWork;
    private readonly SeedDataConfigs _seedDataConfigs;

    public SeedDataService(
        UserManager<User> userManager,
        RoleManager<Role> roleManager,
        IUnitOfWork unitOfWork,
        SeedDataConfigs seedDataConfigs)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _unitOfWork = unitOfWork;
        _seedDataConfigs = seedDataConfigs;
    }

    public async Task Execute()
    {
        await _unitOfWork.Begin();

        try
        {
            if (!_roleManager.Roles.Any())
            {
                foreach (var role in _seedDataConfigs.ApplicationRoles)
                {
                    var applicationRole = new Role()
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = role
                    };
                    await _roleManager.CreateAsync(applicationRole);
                }
            }

            if (!_userManager.Users.Any())
            {
                foreach (var admin in _seedDataConfigs.Admins)
                {
                    var user = new User()
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = admin.AdminName,
                        UserName = admin.AdminName,
                        LastName = admin.AdminLastName,
                        PhoneNumber = admin.AdminPhoneNumber,
                        Email = admin.AdminEmail,
                        CreationDate = DateTime.Now.ToUniversalTime()
                    };
                    
                    await _userManager.CreateAsync(user, admin.AdminPassword);
                    await _userManager.AddToRoleAsync(user, "Admin");
                }
            }

            await _unitOfWork.Commit();
        }
        catch
        {
            await _unitOfWork.Rollback();
        }
    }
}