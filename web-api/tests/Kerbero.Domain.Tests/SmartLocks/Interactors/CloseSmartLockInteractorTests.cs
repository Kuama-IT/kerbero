using FluentAssertions;
using FluentResults;
using Kerbero.Domain.Common.Models;
using Kerbero.Domain.NukiCredentials.Dtos;
using Kerbero.Domain.NukiCredentials.Interfaces;
using Kerbero.Domain.NukiCredentials.Mappers;
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
		var getNukiCredentialsByUserInteractor = new Mock<IGetNukiCredentialsByUserInteractor>();
		var nukiSmartLockRepository = new Mock<INukiSmartLockRepository>();

		var closeSmartLockInteractor = new CloseSmartLockInteractor(
			getNukiCredentialsByUserInteractor.Object,
			nukiSmartLockRepository.Object);

		var tUserId = new Guid();
		var token = "VALID_TOKEN";
		var tCredentials = new NukiCredentialDto()
		{
			Id = 1,
			Token = token
		};

		getNukiCredentialsByUserInteractor.Setup(c => c.Handle(It.IsAny<Guid>()))
			.ReturnsAsync(new List<NukiCredentialDto>()
			{
				tCredentials
			});
		nukiSmartLockRepository.Setup(c => c.Close(It.IsAny<NukiCredentialModel>(), It.IsAny<string>()))
			.ReturnsAsync(Result.Ok());
		
		var interactorResult = await closeSmartLockInteractor.Handle(tUserId, SmartLockProvider.Nuki, "VALID_ID", 1);

		getNukiCredentialsByUserInteractor.Verify(c => c.Handle(tUserId));
		
		getNukiCredentialsByUserInteractor.VerifyNoOtherCalls();
		
		interactorResult.IsSuccess.Should().BeTrue();
	}
}
