using FluentAssertions;
using FluentResults;
using Kerbero.Domain.Common.Errors;
using Kerbero.Domain.Common.Models;
using Kerbero.Domain.NukiCredentials.Models;
using Kerbero.Domain.NukiCredentials.Repositories;
using Kerbero.Domain.SmartLockKeys.Errors;
using Kerbero.Domain.SmartLockKeys.Interactors;
using Kerbero.Domain.SmartLockKeys.Models;
using Kerbero.Domain.SmartLockKeys.Repositories;
using Kerbero.Domain.SmartLocks.Interfaces;
using Moq;

namespace Kerbero.Domain.Tests.SmartLockKeys.Interactors;

public class OpenSmartLockWithKeyInteractorTests
{
	[Fact]
	public async Task OpenSmartLock_WithValidParameters()
	{
		var smartLockKeyRepository = new Mock<ISmartLockKeyRepository>();
		var nukiCredentialRepository = new Mock<INukiCredentialRepository>();
		var openSmartLockInteractor = new Mock<IOpenSmartLockInteractor>();
		var openSmartLockWithKeyInteractor = new OpenSmartLockWithKeyInteractor(
			smartLockKeyRepository.Object,
			nukiCredentialRepository.Object,
			openSmartLockInteractor.Object
			);

		var tSmartLockProvider = SmartLockProvider.Nuki;

		var tSmartLockKeyModel = new SmartLockKeyModel()
		{
			Id = new Guid(),
			Password = "PASSWORD",
			CreationDate = DateTime.Now,
			ExpiryDate = DateTime.Now.AddDays(7).ToUniversalTime(),
			CredentialId = 1,
			IsDisabled = false,
			UsageCounter = 0,
			SmartLockId = "VALID_ID",
			SmartLockProvider = tSmartLockProvider.Name
		};

		var tNukiCredentialModel = new NukiCredentialModel()
		{
			Id = 1,
			Token = "VALID_TOKEN",
			UserId = new Guid()
		};

		var tSmartLockKeyId = new Guid();

		smartLockKeyRepository.Setup(c => c.GetById(It.IsAny<Guid>()))
			.ReturnsAsync(tSmartLockKeyModel);
		nukiCredentialRepository.Setup(c => c.GetById(It.IsAny<int>()))
			.ReturnsAsync(tNukiCredentialModel);
		openSmartLockInteractor.Setup(c =>
				c.Handle(It.IsAny<Guid>(), It.IsAny<SmartLockProvider>(), It.IsAny<string>(), It.IsAny<int>()))
			.ReturnsAsync(Result.Ok());
		smartLockKeyRepository.Setup(c => c.Update(It.IsAny<SmartLockKeyModel>()))
			.ReturnsAsync(tSmartLockKeyModel);

		var result = await openSmartLockWithKeyInteractor.Handle(tSmartLockKeyId, "PASSWORD");

		result.IsSuccess.Should().BeTrue();

		smartLockKeyRepository.Verify(c => c.GetById(tSmartLockKeyId));
		tSmartLockKeyModel.UsageCounter++;
		smartLockKeyRepository.Verify(c => c.Update(tSmartLockKeyModel));
		smartLockKeyRepository.VerifyNoOtherCalls();

		nukiCredentialRepository.Verify(c => c.GetById(tSmartLockKeyModel.CredentialId));
		nukiCredentialRepository.VerifyNoOtherCalls();
		
		openSmartLockInteractor.Verify(c => c.Handle(tNukiCredentialModel.UserId,
			tSmartLockProvider,
			tSmartLockKeyModel.SmartLockId,
			tSmartLockKeyModel.CredentialId
			));
		openSmartLockInteractor.VerifyNoOtherCalls();
	}
	
	[Fact]
	public async Task OpenSmartLock_PasswordMismatch()
	{
		var smartLockKeyRepository = new Mock<ISmartLockKeyRepository>();
		var nukiCredentialRepository = new Mock<INukiCredentialRepository>();
		var openSmartLockInteractor = new Mock<IOpenSmartLockInteractor>();
		var openSmartLockWithKeyInteractor = new OpenSmartLockWithKeyInteractor(
			smartLockKeyRepository.Object,
			nukiCredentialRepository.Object,
			openSmartLockInteractor.Object
			);

		var tSmartLockProvider = SmartLockProvider.Nuki;

		var tSmartLockKeyModel = new SmartLockKeyModel()
		{
			Id = new Guid(),
			Password = "PASSWORD",
			CreationDate = DateTime.Now,
			ExpiryDate = DateTime.Now.AddDays(7).ToUniversalTime(),
			CredentialId = 1,
			IsDisabled = false,
			UsageCounter = 0,
			SmartLockId = "VALID_ID",
			SmartLockProvider = tSmartLockProvider.Name
		};

		var tSmartLockKeyId = new Guid();

		smartLockKeyRepository.Setup(c => c.GetById(It.IsAny<Guid>()))
			.ReturnsAsync(tSmartLockKeyModel);

		var result = await openSmartLockWithKeyInteractor.Handle(tSmartLockKeyId, "INVALID_PASSWORD");

		result.IsFailed.Should().BeTrue();
		result.Errors.First().Should().BeEquivalentTo(new UnauthorizedAccessError());

		smartLockKeyRepository.Verify(c => c.GetById(tSmartLockKeyId));
	}
	
	[Fact]
	public async Task OpenSmartLock_SmartLockKeyIsExpired()
	{
		var smartLockKeyRepository = new Mock<ISmartLockKeyRepository>();
		var nukiCredentialRepository = new Mock<INukiCredentialRepository>();
		var openSmartLockInteractor = new Mock<IOpenSmartLockInteractor>();
		var openSmartLockWithKeyInteractor = new OpenSmartLockWithKeyInteractor(
			smartLockKeyRepository.Object,
			nukiCredentialRepository.Object,
			openSmartLockInteractor.Object
		);

		var tSmartLockProvider = SmartLockProvider.Nuki;

		var tSmartLockKeyModel = new SmartLockKeyModel()
		{
			Id = new Guid(),
			Password = "PASSWORD",
			CreationDate = DateTime.Now,
			ExpiryDate = DateTime.Now.AddDays(-1).ToUniversalTime(),
			CredentialId = 1,
			IsDisabled = false,
			UsageCounter = 0,
			SmartLockId = "VALID_ID",
			SmartLockProvider = tSmartLockProvider.Name
		};

		var tSmartLockKeyId = new Guid();

		smartLockKeyRepository.Setup(c => c.GetById(It.IsAny<Guid>()))
			.ReturnsAsync(tSmartLockKeyModel);

		var result = await openSmartLockWithKeyInteractor.Handle(tSmartLockKeyId, "PASSWORD");

		result.IsFailed.Should().BeTrue();
		result.Errors.First().Should().BeEquivalentTo(new SmartLockKeyExpiredError());

		smartLockKeyRepository.Verify(c => c.GetById(tSmartLockKeyId));
	}
}