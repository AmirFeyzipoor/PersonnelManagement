using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Moq;
using PersonnelManagement.Entities.AuditLogs;
using PersonnelManagement.Entities.Identities;
using PersonnelManagement.UseCases.Infrastructure;
using PersonnelManagement.UseCases.Infrastructure.AuditLogs;
using PersonnelManagement.UseCases.Infrastructure.TokenManager.Contracts;
using PersonnelManagement.UseCases.Infrastructure.UserTokens.Contrects;
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
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IAuditLogRepository> _mockAuditLogRepository;

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
        var mockPersonnelRepository = new Mock<IPersonnelRepository>();
        var mockTokenManagerService = new Mock<ITokenManagerService>();
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockAuditLogRepository = new Mock<IAuditLogRepository>();
        
        _personnelService = new PersonnelService(
            _mockUserManager.Object,
            mockPersonnelRepository.Object,
            mockTokenManagerService.Object,
            _mockAuditLogRepository.Object,
            _mockUnitOfWork.Object);
    }

    [Fact]
    public async Task RegisterUser_register_user_properly()
    {
        var dto = new RegisterPersonnelDto(
            name: "Amir",
            lastName: "feyzipoor",
            phoneNumber: "09389066817",
            password: "Amir007");
        var fakeRegistrantIdId = "b4134711-a12e-491a-9aa4-766d538846fb";
            
        _mockUserManager.Setup(_ => _.CreateAsync(
                It.Is<User>(_ => _.PhoneNumber == dto.PhoneNumber),
                It.Is<string>(_ => _ == dto.Password)))
            .ReturnsAsync(IdentityResult.Success);

        await _personnelService.RegisterUser(fakeRegistrantIdId, dto);
        
        _mockAuditLogRepository.Verify(_ => _.AddLog(
            It.IsAny<AuditLog>()));
        _mockUnitOfWork.Verify(_ => _.Complete());
    }
    
    [Fact]
    public async Task RegisterUser_throw_InvalidPhoneNumberException_when_phoneNumber_is_invalid()
    {
        var dto = new RegisterPersonnelDto(
            name: "Amir",
            lastName: "feyzipoor",
            phoneNumber: "dummyPhoneNumber",
            password: "Amir007");
        var fakeRegistrantIdId = "b4134711-a12e-491a-9aa4-766d538846fb";

        var expected = async () => await _personnelService.RegisterUser(fakeRegistrantIdId, dto);
        
        await expected.Should().ThrowExactlyAsync<InvalidPhoneNumberException>();
    }

}