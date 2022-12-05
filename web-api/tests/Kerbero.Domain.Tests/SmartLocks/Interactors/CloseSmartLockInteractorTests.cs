using FluentAssertions;
using FluentResults;
using Kerbero.Domain.Common.Models;
using Kerbero.Domain.NukiCredentials.Interfaces;
using Kerbero.Domain.NukiCredentials.Models;
using Kerbero.Domain.SmartLocks.Interactors;
using Kerbero.Domain.SmartLocks.Repositories;
using Moq;

namespace Kerbero.Domain.Tests.SmartLocks.Interactors;

public class CloseSmartLockInteractorTests
{
  [Fact]
  public async Task CloseSmartLock_WithValidParameters()
  {
    var getNukiCredentialInteractor = new Mock<IGetNukiCredentialInteractor>();
    var nukiSmartLockRepository = new Mock<INukiSmartLockRepository>();

    var closeSmartLockInteractor = new CloseSmartLockInteractor(
      nukiSmartLockRepository.Object,
      getNukiCredentialInteractor.Object
    );

    var tUserId = new Guid();
    var token = "VALID_TOKEN";
    var tCredentials = new NukiCredentialModel()
    {
      Id = 1,
      Token = token,
      NukiEmail = "test@nuki.com"
    };

    getNukiCredentialInteractor
      .Setup(c => c.Handle(It.IsAny<int>(), It.IsAny<Guid?>()))
      .ReturnsAsync(tCredentials);
    nukiSmartLockRepository.Setup(c => c.Close(It.IsAny<NukiCredentialModel>(), It.IsAny<string>()))
      .ReturnsAsync(Result.Ok());

    var interactorResult = await closeSmartLockInteractor.Handle(tUserId, SmartLockProvider.Nuki, "VALID_ID", 1);

    getNukiCredentialInteractor.Verify(c => c.Handle(tCredentials.Id, tUserId));

    getNukiCredentialInteractor.VerifyNoOtherCalls();

    interactorResult.IsSuccess.Should().BeTrue();
  }
}