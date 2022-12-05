using FluentAssertions;
using FluentResults;
using Kerbero.Domain.Common.Models;
using Kerbero.Domain.NukiCredentials.Interfaces;
using Kerbero.Domain.NukiCredentials.Models;
using Kerbero.Domain.SmartLockKeys.Interactors;
using Kerbero.Domain.SmartLockKeys.Models;
using Kerbero.Domain.SmartLockKeys.Repositories;
using Moq;

namespace Kerbero.Domain.Tests.SmartLockKeys.Interactors;

public class GetSmartLockKeysInteractorTests
{
  [Fact]
  public async Task GetSmartLockKeys_ValidParameters()
  {
    var smartLockKeysRepository = new Mock<ISmartLockKeyRepository>();
    var getNukiCredentialsByUserInteractor = new Mock<IGetNukiCredentialsByUserInteractor>();
    var getSmartLockKeysInteractor =
      new GetSmartLockKeysInteractor(smartLockKeysRepository.Object, getNukiCredentialsByUserInteractor.Object);

    var credentialsModel = new UserNukiCredentialsModel(
      NukiCredentials: new List<NukiCredentialModel>()
      {
        new()
        {
          Id = 1, Token = "VALID_TOKEN",
          NukiEmail = "test@nuki.com"
        }
      },
      OutdatedCredentials: new List<(NukiCredentialModel, List<IError>)>()
    );

    getNukiCredentialsByUserInteractor.Setup(c => c.Handle(It.IsAny<Guid>()))
      .ReturnsAsync(credentialsModel);

    var tSmartLockKeyModel = new SmartLockKeyModel
    {
      Password = "VALID_TOKEN",
      SmartLockId = "VALID_ID",
      SmartLockProvider = SmartLockProvider.Nuki.Name,
      ValidFrom = DateTime.Now,
      ValidUntil = DateTime.Now,
      CreatedAt = DateTime.Now
    };

    var tExpectedList = new List<SmartLockKeyModel>()
    {
      tSmartLockKeyModel
    };

    var tExpected = new UserSmartLockKeysModel(
      SmartLockKeys: tExpectedList,
      OutdatedCredentials: new List<(NukiCredentialModel, List<IError>)>()
    );

    smartLockKeysRepository.Setup(c => c.GetAllByCredentials(It.IsAny<List<NukiCredentialModel>>()))
      .ReturnsAsync(tExpectedList);
    var result = await getSmartLockKeysInteractor.Handle(new Guid());
    result.Value.Should().BeEquivalentTo(tExpected);
  }
}