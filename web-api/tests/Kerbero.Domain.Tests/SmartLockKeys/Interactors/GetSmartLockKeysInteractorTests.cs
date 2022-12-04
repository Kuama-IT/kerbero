using FluentAssertions;
using Kerbero.Domain.Common.Models;
using Kerbero.Domain.NukiCredentials.Models;
using Kerbero.Domain.NukiCredentials.Repositories;
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
		var nukiCredentialRepository = new Mock<INukiCredentialRepository>();
		var getSmartLockKeysInteractor =
			new GetSmartLockKeysInteractor(smartLockKeysRepository.Object, nukiCredentialRepository.Object);

		nukiCredentialRepository.Setup(c => c.GetAllByUserId(It.IsAny<Guid>()))
			.ReturnsAsync(new List<NukiCredentialModel>() { new() { Id = 1, Token = "VALID_TOKEN" } });
		var tSmartLockKeyModel = new SmartLockKeyModel
		{
			Password = "VALID_TOKEN",
			SmartLockId = "VALID_ID",
			SmartLockProvider = SmartLockProvider.Nuki.Name,
		};

		var tExpected = new List<SmartLockKeyModel>()
		{
			tSmartLockKeyModel
		};
		smartLockKeysRepository.Setup(c => c.GetAllByCredentials(It.IsAny<List<NukiCredentialModel>>()))
			.ReturnsAsync(tExpected);
		var result = await getSmartLockKeysInteractor.Handle(new Guid());
		result.Value.Should().BeEquivalentTo(tExpected);
	}
}
