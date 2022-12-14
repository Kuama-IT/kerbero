using FluentAssertions;
using FluentResults;
using Kerbero.Domain.Common.Errors;
using Kerbero.Domain.Common.Models;
using Kerbero.Domain.NukiCredentials.Interfaces;
using Kerbero.Domain.NukiCredentials.Models;
using Kerbero.Domain.NukiCredentials.Repositories;
using Kerbero.Domain.SmartLockKeys.Errors;
using Kerbero.Domain.SmartLockKeys.Interactors;
using Kerbero.Domain.SmartLockKeys.Models;
using Kerbero.Domain.SmartLockKeys.Repositories;
using Kerbero.Domain.SmartLocks.Interfaces;
using Moq;

namespace Kerbero.Domain.Tests.SmartLockKeys.Interactors;

public class CloseSmartLockWithKeyInteractorTests
{
  [Fact]
  public async Task CloseSmartLock_WithValidParameters()
  {
    var smartLockKeyRepository = new Mock<ISmartLockKeyRepository>();
    var getNukiCredentialInteractor = new Mock<IGetNukiCredentialInteractor>();
    var closeSmartLockInteractor = new Mock<ICloseSmartLockInteractor>();
    var closeSmartLockWithKeyInteractor = new CloseSmartLockWithKeyInteractor(
      smartLockKeyRepository: smartLockKeyRepository.Object,
      getNukiCredentialInteractor: getNukiCredentialInteractor.Object,
      closeSmartLockInteractor: closeSmartLockInteractor.Object
    );

    var tSmartLockProvider = SmartLockProvider.Nuki;

    var tSmartLockKeyModel = new SmartLockKeyModel()
    {
      Id = new Guid(),
      Password = "PASSWORD",
      CreatedAt = DateTime.Now,
      ValidFrom = DateTime.Now,
      ValidUntil = DateTime.Now.AddDays(7).ToUniversalTime(),
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
      UserId = new Guid(),
      NukiEmail = "test@nuki.com"
    };

    var tSmartLockKeyId = new Guid();

    smartLockKeyRepository.Setup(c => c.GetById(It.IsAny<Guid>()))
      .ReturnsAsync(tSmartLockKeyModel);
    getNukiCredentialInteractor.Setup(c => c.Handle(It.IsAny<int>(), It.IsAny<Guid?>()))
      .ReturnsAsync(tNukiCredentialModel);
    closeSmartLockInteractor.Setup(c =>
        c.Handle(It.IsAny<Guid>(), It.IsAny<SmartLockProvider>(), It.IsAny<string>(), It.IsAny<int>()))
      .ReturnsAsync(Result.Ok());
    smartLockKeyRepository.Setup(c => c.Update(It.IsAny<SmartLockKeyModel>()))
      .ReturnsAsync(tSmartLockKeyModel);

    var result = await closeSmartLockWithKeyInteractor.Handle(tSmartLockKeyId, "PASSWORD");

    result.IsSuccess.Should().BeTrue();

    smartLockKeyRepository.Verify(c => c.GetById(tSmartLockKeyId));
    tSmartLockKeyModel.UsageCounter++;
    smartLockKeyRepository.Verify(c => c.Update(tSmartLockKeyModel));
    smartLockKeyRepository.VerifyNoOtherCalls();

    getNukiCredentialInteractor.Verify();

    closeSmartLockInteractor.Verify(c => c.Handle(tNukiCredentialModel.UserId,
      tSmartLockProvider,
      tSmartLockKeyModel.SmartLockId,
      tSmartLockKeyModel.CredentialId
    ));
    closeSmartLockInteractor.VerifyNoOtherCalls();
  }

  [Fact]
  public async Task CloseSmartLock_PasswordMismatch()
  {
    var smartLockKeyRepository = new Mock<ISmartLockKeyRepository>();
    var getNukiCredentialInteractor = new Mock<IGetNukiCredentialInteractor>();
    var closeSmartLockInteractor = new Mock<ICloseSmartLockInteractor>();
    var closeSmartLockWithKeyInteractor = new CloseSmartLockWithKeyInteractor(
      smartLockKeyRepository: smartLockKeyRepository.Object,
      getNukiCredentialInteractor: getNukiCredentialInteractor.Object,
      closeSmartLockInteractor: closeSmartLockInteractor.Object
    );

    var tSmartLockProvider = SmartLockProvider.Nuki;

    var tSmartLockKeyModel = new SmartLockKeyModel()
    {
      Id = new Guid(),
      Password = "PASSWORD",
      CreatedAt = DateTime.Now,
      ValidFrom = DateTime.Now,
      ValidUntil = DateTime.Now.AddDays(7).ToUniversalTime(),
      CredentialId = 1,
      IsDisabled = false,
      UsageCounter = 0,
      SmartLockId = "VALID_ID",
      SmartLockProvider = tSmartLockProvider.Name
    };

    var tSmartLockKeyId = new Guid();

    smartLockKeyRepository.Setup(c => c.GetById(It.IsAny<Guid>()))
      .ReturnsAsync(tSmartLockKeyModel);

    var result = await closeSmartLockWithKeyInteractor.Handle(tSmartLockKeyId, "INVALID_PASSWORD");

    result.IsFailed.Should().BeTrue();
    result.Errors.First().Should().BeEquivalentTo(new UnauthorizedAccessError());

    smartLockKeyRepository.Verify(c => c.GetById(tSmartLockKeyId));
  }

  [Fact]
  public async Task OpenSmartLock_SmartLockKeyIsExpired()
  {
    var smartLockKeyRepository = new Mock<ISmartLockKeyRepository>();
    var getNukiCredentialInteractor = new Mock<IGetNukiCredentialInteractor>();
    var closeSmartLockInteractor = new Mock<ICloseSmartLockInteractor>();
    var closeSmartLockWithKeyInteractor = new CloseSmartLockWithKeyInteractor(
      smartLockKeyRepository: smartLockKeyRepository.Object,
      getNukiCredentialInteractor: getNukiCredentialInteractor.Object,
      closeSmartLockInteractor: closeSmartLockInteractor.Object
    );

    var tSmartLockProvider = SmartLockProvider.Nuki;

    var tSmartLockKeyModel = new SmartLockKeyModel()
    {
      Id = new Guid(),
      Password = "PASSWORD",
      CreatedAt = DateTime.Now,
      ValidFrom = DateTime.Now,
      ValidUntil = DateTime.Now.AddDays(-1).ToUniversalTime(),
      CredentialId = 1,
      IsDisabled = false,
      UsageCounter = 0,
      SmartLockId = "VALID_ID",
      SmartLockProvider = tSmartLockProvider.Name
    };

    var tSmartLockKeyId = new Guid();

    smartLockKeyRepository.Setup(c => c.GetById(It.IsAny<Guid>()))
      .ReturnsAsync(tSmartLockKeyModel);

    var result = await closeSmartLockWithKeyInteractor.Handle(tSmartLockKeyId, "PASSWORD");

    result.IsFailed.Should().BeTrue();
    result.Errors.First().Should().BeEquivalentTo(new SmartLockKeyExpiredError());

    smartLockKeyRepository.Verify(c => c.GetById(tSmartLockKeyId));
  }
}