using FluentAssertions;
using FluentResults;
using Kerbero.Domain.Common.Errors;
using Kerbero.Domain.NukiActions.Entities;
using Kerbero.Domain.SmartLockKeys.Entities;
using Kerbero.Domain.SmartLockKeys.Interactors;
using Kerbero.Domain.SmartLockKeys.Managers;
using Kerbero.Domain.SmartLockKeys.Models.PresentationRequests;
using Kerbero.Domain.SmartLockKeys.Models.PresentationResponses;
using Kerbero.Domain.SmartLockKeys.Repositories;
using Moq;

namespace Kerbero.Domain.Tests.SmartLockKeys.Interactors;

public class CreateSmartLockKeyInteractorTest
{
	private readonly CreateSmartLockKeyInteractor _createSmartLockKeyInteractor;
	private readonly Mock<ISmartLockKeyPersistentRepository> _smartLockKeyPersistentRepository;
	private readonly Mock<ISmartLockKeyGeneratorManager> _smartLockKeyGeneratorManager;

	public CreateSmartLockKeyInteractorTest()
	{
		_smartLockKeyPersistentRepository = new Mock<ISmartLockKeyPersistentRepository>();
		_smartLockKeyGeneratorManager = new Mock<ISmartLockKeyGeneratorManager>();
		_createSmartLockKeyInteractor = new CreateSmartLockKeyInteractor(_smartLockKeyGeneratorManager.Object, _smartLockKeyPersistentRepository.Object);
	}

	[Fact]
	public async Task CreateKey_Success()
	{
		// Arrange
		var expectedId = Guid.NewGuid();
		var expiryDate = DateTime.Now.AddDays(7);
		var smartLockKey = new SmartLockKey
		{
			Token = "TOKEN",
			CreationDate = DateTime.Now,
			ExpiryDate = expiryDate,
			IsDisabled = false,
			UsageCounter = 0,
			NukiSmartLockId = 1,
			NukiSmartLockEntity = new NukiSmartLockEntity() { Id = 1 }
		};
		var keyPresentation = new CreateSmartLockKeyPresentationResponse()
		{
			Id = expectedId,
			CreationDate = smartLockKey.CreationDate,
			ExpiryDate = smartLockKey.ExpiryDate,
			Token = smartLockKey.Token
		};
		var presentationRequest = new CreateSmartLockKeyPresentationRequest(SmartLockId: 1, ExpiryDate: expiryDate);
		_smartLockKeyPersistentRepository.Setup(repo => repo.Create(It.IsAny<SmartLockKey>()))
			.ReturnsAsync(Result.Ok(smartLockKey));
		_smartLockKeyGeneratorManager.Setup(help => help.GenerateSmartLockKey(1, expiryDate))
			.Returns(() =>
			{
				smartLockKey.Id = expectedId;
				return smartLockKey;
			});
		
		// Act
		var result = await _createSmartLockKeyInteractor.Handle(presentationRequest);
		
		// Assert
		_smartLockKeyGeneratorManager.Verify(man => man.GenerateSmartLockKey(1, expiryDate));
		_smartLockKeyPersistentRepository.Verify(rep => rep.Create(smartLockKey));
		
		_smartLockKeyGeneratorManager.VerifyNoOtherCalls();
		_smartLockKeyPersistentRepository.VerifyNoOtherCalls();

		result.Value.Should().BeEquivalentTo(keyPresentation);
	}

	[Fact]
	public async Task CreateKey_PersistentResourceNotReachable()
	{
		// Arrange
		var expectedId = Guid.NewGuid();
		var expiryDate = DateTime.Now.AddDays(7);
		var smartLockKey = new SmartLockKey
		{
			Token = "TOKEN",
			CreationDate = DateTime.Now,
			ExpiryDate = expiryDate,
			IsDisabled = false,
			UsageCounter = 0,
			NukiSmartLockId = 1,
			NukiSmartLockEntity = new NukiSmartLockEntity() { Id = 1 }
		};
		var presentationRequest = new CreateSmartLockKeyPresentationRequest(SmartLockId: 1, ExpiryDate: expiryDate);
		_smartLockKeyPersistentRepository.Setup(repo => repo.Create(It.IsAny<SmartLockKey>()))
			.ReturnsAsync(Result.Fail(new PersistentResourceNotAvailableError()));
		_smartLockKeyGeneratorManager.Setup(help => help.GenerateSmartLockKey(1, expiryDate))
			.Returns(() =>
			{
				smartLockKey.Id = expectedId;
				return smartLockKey;
			});
		
		// Act
		var result = await _createSmartLockKeyInteractor.Handle(presentationRequest);

		result.Errors.Single().Should().BeEquivalentTo(new PersistentResourceNotAvailableError());
	}
}
