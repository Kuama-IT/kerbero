using FluentAssertions;
using FluentResults;
using Kerbero.Domain.Common.Errors;
using Kerbero.Domain.NukiActions.Entities;
using Kerbero.Domain.NukiActions.Errors;
using Kerbero.Domain.NukiActions.Interactors;
using Kerbero.Domain.NukiActions.Interfaces;
using Kerbero.Domain.NukiActions.Models.ExternalRequests;
using Kerbero.Domain.NukiActions.Models.PresentationRequest;
using Kerbero.Domain.NukiActions.Repositories;
using Moq;

namespace Kerbero.Domain.Tests.NukiActions.Interactors;

public class CloseNukiSmartLockInteractorTest
{
    private readonly CloseNukiSmartLockInteractor _interactor;
    private readonly Mock<INukiSmartLockExternalRepository> _external;
    private readonly Mock<INukiSmartLockPersistentRepository> _persistent;

    public CloseNukiSmartLockInteractorTest()
    {
        _persistent = new Mock<INukiSmartLockPersistentRepository>();
        _external = new Mock<INukiSmartLockExternalRepository>();
        _interactor = new CloseNukiSmartLockInteractor(_persistent.Object, _external.Object);
        _interactor.Should().BeAssignableTo<ICloseNukiSmartLockInteractor>();
    }

    [Fact]
    public async Task Handle_Success()
    {
        // Arrange
        _persistent.Setup(c => c.GetById(It.IsAny<int>()))
            .Returns(Task.FromResult(Result.Ok(new NukiSmartLock
            {
                ExternalSmartLockId = 1,
                // other things
            })));
        _external.Setup(c => c.CloseNukiSmartLock(It.IsAny<NukiSmartLockExternalRequest>()))
            .Returns(() => Task.FromResult(Result.Ok()));
            
        // Act
        var result = await _interactor.Handle(new CloseNukiSmartLockPresentationRequest("ACCESS_TOKEN", 0));
        
        // Assert
        result.IsSuccess.Should().BeTrue();
        _persistent.Verify(c => c.GetById(It.Is<int>(x => x == 0)));
        _external.Setup(c =>
            c.CloseNukiSmartLock(It.Is<NukiSmartLockExternalRequest>(x => x.ExternalId == 1)));
    }

    [Fact]
    public async Task Handle_NoSmartLockFound()
    {
        _persistent.Setup(c => c.GetById(It.IsAny<int>()))
            .Returns(async () => await Task.FromResult(Result.Fail(new SmartLockNotFoundError())));
        
        // Act
        var result = await _interactor.Handle(new CloseNukiSmartLockPresentationRequest("ACCESS_TOKEN", 0));

        result.IsFailed.Should().BeTrue();
        result.Errors.First().Should().BeEquivalentTo(new SmartLockNotFoundError());
    }
    
    [Fact]
    public async Task Handle_ErrorOnOpening()
    {
        _persistent.Setup(c => c.GetById(It.IsAny<int>()))
            .Returns(Task.FromResult(Result.Ok(new NukiSmartLock
            {
                ExternalSmartLockId = 1,
                // other things
            })));
        _external.Setup(c => c.CloseNukiSmartLock(It.IsAny<NukiSmartLockExternalRequest>()))
            .Returns(() => Task.FromResult(Result.Fail(new ExternalServiceUnreachableError())));
        
        // Act
        var result = await _interactor.Handle(new CloseNukiSmartLockPresentationRequest("ACCESS_TOKEN", 0));

        result.IsFailed.Should().BeTrue();
        result.Errors.First().Should().BeEquivalentTo(new ExternalServiceUnreachableError());
    }
}