using Microsoft.AspNetCore.Identity;
using Moq;
using PersonnelManagement.Entities.Identities;
using PersonnelManagement.IntegrationTest.Infrastructure;
using PersonnelManagement.UseCases.Personnel;
using PersonnelManagement.UseCases.Personnel.Contracts.Configs;
using PersonnelManagement.UseCases.Personnel.Contracts.Dtos;

namespace PersonnelManagement.IntegrationTest.Personnel.Add;

//Here, due to the use of UserManager<User> and
//not having a personal repository for users,
//spec tests and integration tests will not be much different from unit tests
//And we don't need to pour the test data into the test database and check it at the end.

[Story(
    AsA = "کاربر ادمین احراز هویت شده",
    InOrderTo = "پرسنل را مدیریت کنم",
    IWantTo = "یک پرسنل و کاربر عادی را ثبت کنم")]
[Scenario("ثبت پرسنل جدید")]
public class Success
{
    private readonly Mock<UserManager<User>> _mockUserManager;
    private RegisterPersonnelDto _dto;

    public Success()
    {
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
            
        _mockUserManager.Setup(_ => _.CreateAsync(
                It.Is<User>(_ => _.PhoneNumber == _dto.PhoneNumber &&
                                 _.Name == _dto.Name && 
                                 _.LastName == _dto.LastName && 
                                 _.Email == _dto.Email),
                It.Is<string>(_ => _ == _dto.Password)))
            .ReturnsAsync(IdentityResult.Success);

        var personnelService = new PersonnelService(
            userManager: _mockUserManager.Object,
            new JwtBearerTokenSettings());
        await personnelService.RegisterUser(_dto);
    }

    [Then("باید تنها یک پرسنل و کاربر عادی (به جز ادمین ها)" +
          "با شماره ی '09029380902'" +
          "و نام 'amir'" +
          "و نام خانوادگی 'feyzipoor'" +
          "و ایمیل 'Amir0007@gmail.com'"+
          " در فهرست پرسنل وجود داشته باشد")]
    public void Then()
    {
        _mockUserManager.Verify(_ => _.CreateAsync(
            It.Is<User>(_ => _.PhoneNumber == _dto.PhoneNumber),
            It.Is<string>(_ => _ == _dto.Password)));
    }

    [Fact]
    public void Run()
    {
        Runner.RunScenario(
            _ => Given(),
            _ => When().Wait(),
            _ => Then());
    }
}