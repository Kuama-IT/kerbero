using FluentAssertions;
using Kerbero.Domain.NukiActions.Entities;
using Kerbero.Domain.NukiActions.Repositories;
using Kerbero.Infrastructure.Common.Context;
using Kerbero.Infrastructure.NukiActions.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace Kerbero.Infrastructure.Tests.NukiActions.Repositories;

public class NukiSmartLockPersistentRepositoryTests
{
    private readonly NukiSmartLockPersistentRepository _persistent;
    private readonly Mock<DbSet<NukiSmartLock>> _dbSetNukiAccount;
    private readonly NukiSmartLock _data;

    public NukiSmartLockPersistentRepositoryTests()
    {        
        var logger = new Mock<ILogger<NukiSmartLockPersistentRepository>>();
        _data = new NukiSmartLock
        {
            Favourite = true,
            Name = "kquarter",
            Type = 0,
            NukiAccountId = 0,
            AuthId = 0,
            ExternalSmartLockId = 0,
            State = new NukiSmartLockState
            {
                Mode = 4,
                State = 255,
                LastAction = 5,
                BatteryCritical = true,
                BatteryCharging = true,
                BatteryCharge = 100,
                DoorState = 255,
                OperationId = "string"
            }
        };
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "AppDbContext")
            .Options;
        var appDbContext = new ApplicationDbContext(options);
        _dbSetNukiAccount = new Mock<DbSet<NukiSmartLock>>();
        _persistent = new NukiSmartLockPersistentRepository(appDbContext, logger.Object);
    }

    [Fact]
    public async Task Create_Success_Test()
    {
        // Arrange
        _persistent.Should().BeAssignableTo<INukiSmartLockPersistentRepository>();

        // Act
        var response = await _persistent.Create(_data);
        
        // Assert
        response.IsSuccess.Should().BeTrue();
        _data.Id = 1;
        response.Value.Should().BeEquivalentTo(_data);
    }
}