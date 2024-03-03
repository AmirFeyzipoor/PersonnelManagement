using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using PersonnelManagement.Entities.AuditLogs;
using PersonnelManagement.Entities.Identities;
using PersonnelManagement.Infrastructure.Data;
using PersonnelManagement.Infrastructure.Data.AuditLogs;
using PersonnelManagement.Infrastructure.Data.Identities;
using PersonnelManagement.IntegrationTest.Infrastructure;
using PersonnelManagement.RestApi.Configs.ServiceConfigs;
using PersonnelManagement.RestApi.Configs.ServiceConfigs.ServicesPrerequisites;
using PersonnelManagement.UseCases.Infrastructure.TokenManager;
using PersonnelManagement.UseCases.Personnel;
using PersonnelManagement.UseCases.Personnel.Contracts;
using PersonnelManagement.UseCases.Personnel.Contracts.Configs;
using PersonnelManagement.UseCases.Personnel.Contracts.Dtos;

namespace PersonnelManagement.IntegrationTest.Personnel.Add;

[Story(
    AsA = "کاربر ادمین احراز هویت شده",
    InOrderTo = "پرسنل را مدیریت کنم",
    IWantTo = "یک پرسنل و کاربر عادی را ثبت کنم")]
[Scenario("ثبت پرسنل جدید")]
public class Success : EFDataContextDatabaseFixture
{
    private readonly DataContext _readDataContext;
    private readonly Mock<UserManager<User>> _mockUserManager;
    private readonly IPersonnelService _personnelService;
    private RegisterPersonnelDto? _dto;

    public Success(ConfigurationFixture configuration) : base(configuration)
    {
        var dataContext = CreateDataContext();
        _readDataContext = CreateDataContext();
        var dapperDataContext = CreateDapperContext();
        var userStore = new Mock<IUserStore<User>>();
        _mockUserManager = new Mock<UserManager<User>>(
            userStore.Object,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null);
        var tokenManagerService = new TokenManagerService(new JwtBearerTokenSettings());
        var personnelRepository = new DapperPersonnelRepository(dapperDataContext);
        var unitOfWork = new UnitOfWork(dataContext);
        var auditLogRepository = new EfAuditLogRepository(dataContext); 
        
        _personnelService = new PersonnelService(
            userManager: _mockUserManager.Object,
            personnelRepository,
            tokenManagerService,
            auditLogRepository,
            unitOfWork);

    }
    
    [Given("هیچ پرسنل و کاربر عادی ای (به جز ادمین ها) در فهرست پرسنل وجود ندارد")]
    public void Given() { }

    [When("یک پرسنل با شماره ی '09029380902'" +
          "و نام 'amir'" +
          "و نام خانوادگی 'feyzipoor'" +
          "و ایمیل 'Amir0007@gmail.com'"+
          "در سامانه ثبت نام میکنم")]
    public async Task When()
    {
        _dto = new RegisterPersonnelDto(
            name: "Amir",
            lastName: "feyzipoor",
            phoneNumber: "09029380902",
            password: "Amir007");
        var fakeRegistrantId = "b4134711-a12e-491a-9aa4-766d538846fb"; 
            
        _mockUserManager.Setup(_ => _.CreateAsync(
                It.Is<User>(_ => _.PhoneNumber == _dto.PhoneNumber &&
                                 _.Name == _dto.Name && 
                                 _.LastName == _dto.LastName && 
                                 _.Email == _dto.Email),
                It.Is<string>(_ => _ == _dto.Password)))
            .ReturnsAsync(IdentityResult.Success);
        
        await _personnelService.RegisterUser(fakeRegistrantId, _dto);
    }

    [Then("باید تنها یک پرسنل و کاربر عادی (به جز ادمین ها)" +
          "با شماره ی '09029380902'" +
          " در فهرست پرسنل وجود داشته باشد")]
    [And("باید تنها یک لاگ برای پرسنل ثبت شده در فهرست لاگ ها وجود داشته باشد")]
    public async Task Then()
    {
        _mockUserManager.Verify(_ => _.CreateAsync(
            It.Is<User>(_ => _.PhoneNumber == _dto!.PhoneNumber),
            It.Is<string>(_ => _ == _dto!.Password)));

        var actual = await _readDataContext.Set<AuditLog>().ToListAsync();
        actual.Should().HaveCount(1);
        actual.First().Action.Should().Be(EntityState.Added.ToString());
    }

    [Fact]
    public void Run()
    {
        Runner.RunScenario(
            _ => Given(),
            _ => When().Wait(),
            _ => Then().Wait());
    }
}