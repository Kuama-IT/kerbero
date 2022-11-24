using FluentAssertions;
using FluentResults;
using Kerbero.Domain.Common.Errors;
using Kerbero.Domain.Common.Interfaces;
using Kerbero.Domain.NukiAuthentication.Entities;
using Kerbero.Domain.NukiAuthentication.Interactors;
using Kerbero.Domain.NukiAuthentication.Models.ExternalRequests;
using Kerbero.Domain.NukiAuthentication.Models.ExternalResponses;
using Kerbero.Domain.NukiAuthentication.Models.PresentationRequests;
using Kerbero.Domain.NukiAuthentication.Models.PresentationResponses;
using Kerbero.Domain.NukiAuthentication.Repositories;
using Moq;

namespace Kerbero.Domain.Tests.NukiAuthentication.Interactors;

public class CreateNukiAccountAndRedirectToNukiTest
{
	private readonly CreateNukiAccountAndRedirectToNukiInteractor _createNukiAccountAndRedirectToNukiInteractor;
	private readonly Mock<INukiAccountExternalRepository> _nukiExternalRepository;
	private readonly Mock<INukiAccountPersistentRepository> _nukiPersistentRepository;

	public CreateNukiAccountAndRedirectToNukiTest()
	{
		_nukiPersistentRepository = new Mock<INukiAccountPersistentRepository>();
		_nukiExternalRepository = new Mock<INukiAccountExternalRepository>();
		_createNukiAccountAndRedirectToNukiInteractor =
			new CreateNukiAccountAndRedirectToNukiInteractor(_nukiExternalRepository.Object,
				_nukiPersistentRepository.Object);
	}

	[Fact]
	public async Task Handle_Success()
	{
		// Arrange
		var clientId = "VALID_CLIENT_ID";
		var nukiAccountEntity = new NukiAccount
		{
			ClientId = clientId
		};
		var uri = new Uri("http://api.nuki.io/oauth/authorize?response_type=code" +
		                  "&client_id=v7kn_NX7vQ7VjQdXFGK43g" +
		                  "&redirect_uri=https%3A%2F%2Ftest.com%2Fnuki%2Fcode%2Fv7kn_NX7vQ7VjQdXFGK43g" +
		                  "&scope=account notification smartlock smartlock.readOnly smartlock.action smartlock.auth smartlock.config smartlock.log");
		_nukiExternalRepository.Setup(c => c.BuildUriForCode(It.IsAny<NukiAccountBuildUriForCodeExternalRequest>()))
			.Returns(new NukiAccountBuildUriForCodeExternalResponse(uri));
		_nukiPersistentRepository.Setup(c => c.Create(It.IsAny<NukiAccount>()))
			.ReturnsAsync(() =>
			{
				nukiAccountEntity.Id = 1;
				return Result.Ok(nukiAccountEntity);
			});

		// Act
		var redirectUri = await 
			_createNukiAccountAndRedirectToNukiInteractor.Handle(
				new CreateNukiAccountRedirectPresentationRequest(clientId, Guid.NewGuid()));

		// Assert
		_nukiPersistentRepository.Verify(c =>
			c.Create(It.Is<NukiAccount>(e => e.ClientId == nukiAccountEntity.ClientId)));
		_createNukiAccountAndRedirectToNukiInteractor.Should()
			.BeAssignableTo<InteractorAsync<CreateNukiAccountRedirectPresentationRequest,
				CreateNukiAccountAndRedirectPresentationResponse>>();
		_nukiExternalRepository.Verify(c => c.BuildUriForCode(It.Is<NukiAccountBuildUriForCodeExternalRequest>(s =>
			s.ClientId.Equals("VALID_CLIENT_ID"))));


		redirectUri.Should().BeOfType<Result<CreateNukiAccountAndRedirectPresentationResponse>>();
		redirectUri.Value.RedirectUri.Should().BeEquivalentTo(uri);
	}

	[Theory]
	[MemberData(nameof(PersistentErrorToTest))]
	public async Task Handle_Return_ErrorResult(KerberoError error)
	{
		// Arrange
		var clientId = "VALID_CLIENT_ID";
		_nukiPersistentRepository.Setup(c => c.Create(It.IsAny<NukiAccount>()))
			.ReturnsAsync(Result.Fail(error));
		
		var result = await _createNukiAccountAndRedirectToNukiInteractor.Handle( 
			new CreateNukiAccountRedirectPresentationRequest(clientId, Guid.NewGuid()));

		result.IsFailed.Should().BeTrue();
		result.Errors.Single(e => e.Equals(error)).Should().NotBeNull().And.BeEquivalentTo(error);
	}
	
	public static IEnumerable<object[]> PersistentErrorToTest =>
		new List<object[]>
		{
			new object[] { new DuplicateEntryError("Nuki account")},
			new object[] { new PersistentResourceNotAvailableError()}
		};
}
