using FluentAssertions;
using Kerbero.Domain.NukiActions.Entities;
using Kerbero.Domain.NukiActions.Errors;
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
    private readonly NukiSmartLockEntity _data;
    private readonly ApplicationDbContext _appDbContext;

    public NukiSmartLockPersistentRepositoryTests()
    {        
        var logger = new Mock<ILogger<NukiSmartLockPersistentRepository>>();
        _data = new NukiSmartLockEntity
        {
            Favourite = true,
            Name = "kquarter",
            Type = 0,
            NukiAccountId = 0,
            AuthId = 0,
            ExternalSmartLockId = 0,
            State = new NukiSmartLockStateEntity
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
        _appDbContext = new ApplicationDbContext(options);
        _persistent = new NukiSmartLockPersistentRepository(_appDbContext, logger.Object);
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
    [Fact]
    public async Task Read_Success_Test()
    {
        // Arrange
        var ent = _appDbContext.NukiSmartLocks.Add(new NukiSmartLockEntity
        {
            NukiAccountId = 1,
            Favourite = true,
            Name = "kquarter",
            ExternalSmartLockId = 432414,
            Type = 0
        });
        await _appDbContext.SaveChangesAsync();
        
        // Act
        var response = await _persistent.GetById(ent.Entity.Id);
        
        response.IsSuccess.Should().BeTrue();
        response.Value.Name.Should().Be("kquarter");
        response.Value.ExternalSmartLockId.Should().Be(432414);

    }
    [Fact]    
    public async Task Read_NotFound_Test()
    {
        var response = await _persistent.GetById(0);
        response.IsFailed.Should().BeTrue();
        response.Errors.First().Should().BeEquivalentTo(new SmartLockNotFoundError());
    }
}