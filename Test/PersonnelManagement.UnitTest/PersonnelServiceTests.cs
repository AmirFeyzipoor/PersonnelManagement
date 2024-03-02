using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Moq;
using PersonnelManagement.Entities.Identities;
using PersonnelManagement.UseCases.Personnel;
using PersonnelManagement.UseCases.Personnel.Contracts;
using PersonnelManagement.UseCases.Personnel.Contracts.Configs;
using PersonnelManagement.UseCases.Personnel.Contracts.Dtos;
using PersonnelManagement.UseCases.Personnel.Exceptions;

namespace PersonnelManagement.UnitTest;

public class PersonnelServiceTests
{
    private readonly Mock<UserManager<User>> _mockUserManager;
    private readonly IPersonnelService _personnelService;

    public PersonnelServiceTests()
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
        _personnelService = new PersonnelService(
            userManager: _mockUserManager.Object,
            new JwtBearerTokenSettings());
    }

    [Fact]
    public async Task RegisterUser_register_user_properly()
    {
        var dto = new RegisterPersonnelDto(
            name: "Amir",
            lastName: "feyzipoor",
            phoneNumber: "09389066817",
            password: "Amir007");
            
        _mockUserManager.Setup(_ => _.CreateAsync(
                It.Is<User>(_ => _.PhoneNumber == dto.PhoneNumber),
                It.Is<string>(_ => _ == dto.Password)))
            .ReturnsAsync(IdentityResult.Success);

        await _personnelService.RegisterUser(dto);
    }
    
    [Fact]
    public async Task RegisterUser_throw_InvalidPhoneNumberException_when_phoneNumber_is_invalid()
    {
        var dto = new RegisterPersonnelDto(
            name: "Amir",
            lastName: "feyzipoor",
            phoneNumber: "dummyPhoneNumber",
            password: "Amir007");

        var expected = async () => await _personnelService.RegisterUser(dto);
        
        await expected.Should().ThrowExactlyAsync<InvalidPhoneNumberException>();
    }

}