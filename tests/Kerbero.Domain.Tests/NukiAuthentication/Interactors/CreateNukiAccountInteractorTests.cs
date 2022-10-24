using FluentAssertions;
using FluentResults;
using Kerbero.Domain.Common.Errors;
using Kerbero.Domain.NukiAuthentication.Entities;
using Kerbero.Domain.NukiAuthentication.Interactors;
using Kerbero.Domain.NukiAuthentication.Models.ExternalRequests;
using Kerbero.Domain.NukiAuthentication.Models.ExternalResponses;
using Kerbero.Domain.NukiAuthentication.Models.PresentationRequests;
using Kerbero.Domain.NukiAuthentication.Models.PresentationResponses;
using Kerbero.Domain.NukiAuthentication.Repositories;
using Moq;

namespace Kerbero.Domain.Tests.NukiAuthentication.Interactors;

public class CreateNukiAccountInteractorTests
{
	private readonly Mock<INukiAccountPersistentRepository> _repository;
	private readonly CreateNukiAccountInteractor _interactor;
	private readonly Mock<INukiAccountExternalRepository> _nukiClient;

	public CreateNukiAccountInteractorTests()
	{
		_nukiClient = new Mock<INukiAccountExternalRepository>();
		_repository = new Mock<INukiAccountPersistentRepository>();
		_interactor = new CreateNukiAccountInteractor(_repository.Object, _nukiClient.Object);
	}

	// Handle should create an entity from an input DTO and upload it into the DB.
	[Fact]
	public async Task Handle_ReturnASuccessfulResponse_Test()
	{
		// Arrange
		var nukiAccountDto = new NukiAccountExternalResponse
		{
			Token = "VALID_TOKEN",
			RefreshToken = "VALID_REFRESH_TOKEN",
			ClientId = "VALID_CLIENT_ID",
			TokenType = "bearer",
			TokenExpiresIn = 2592000,
		};
		var nukiAccountEntity = new NukiAccount
		{
			Token = "VALID_TOKEN",
			RefreshToken = "VALID_REFRESH_TOKEN",
			TokenExpiringTimeInSeconds = 2592000,
			ClientId = "VALID_CLIENT_ID",
			TokenType = "bearer",
		};
		_nukiClient.Setup(c => c.GetNukiAccount(
			It.IsAny<NukiAccountExternalRequest>()))
			.Returns(() => Task.FromResult(Result.Ok(nukiAccountDto)));
		_repository.Setup(c => 
			c.Create(It.IsAny<NukiAccount>())).Returns(
			async () => { nukiAccountEntity.Id = 1; return await Task.FromResult(Result.Ok(nukiAccountEntity)); });
		
		// Act
		var nukiAccountPresentationDto = await _interactor.Handle(new NukiAccountPresentationRequest
			{ ClientId = "VALID_CLIENT_ID", Code = "VALID_CODE"});
		
		// Assert
		_nukiClient.Verify(c => c.GetNukiAccount(
			It.Is<NukiAccountExternalRequest>(s => 
				s.ClientId.Contains("VALID_CLIENT_ID") &&
				s.Code!.Contains("VALID_CODE"))));
		_repository.Verify(c => c
			.Create(It.Is<NukiAccount>(account => 
				account.Token == nukiAccountEntity.Token &&
				account.RefreshToken == nukiAccountEntity.RefreshToken &&
				account.TokenExpiringTimeInSeconds == nukiAccountEntity.TokenExpiringTimeInSeconds &&
				account.TokenType == nukiAccountEntity.TokenType &&
				account.ClientId == nukiAccountEntity.ClientId)));
		nukiAccountPresentationDto.Should().BeOfType<Result<NukiAccountPresentationResponse>>();
		nukiAccountPresentationDto.Value.Should().BeEquivalentTo(new NukiAccountPresentationResponse
		{
			Id = 1,
			ClientId = "VALID_CLIENT_ID"
		});
	}

	[Theory]
	[MemberData(nameof(ExternalErrorToTest))]
	public async Task Handle_ExternalReturnAnError_Test(KerberoError error)
	{
		// Arrange
		
		_nukiClient.Setup(c => c.GetNukiAccount(
				It.IsAny<NukiAccountExternalRequest>()))
			.Returns(async () => await Task.FromResult(Result.Fail(error)));

		// Act 
		var ex = await _interactor.Handle(new NukiAccountPresentationRequest
			{ ClientId = "VALID_CLIENT_ID", Code = "VALID_CODE" });
		// Assert
		ex.IsFailed.Should().BeTrue();
		ex.Errors.FirstOrDefault()!.Should().BeEquivalentTo(error);
	}	
	public static IEnumerable<object[]> ExternalErrorToTest =>
		new List<object[]>
		{
			new object[] { new ExternalServiceUnreachableError()},
			new object[] { new UnableToParseResponseError()},
			new object[] { new UnauthorizedAccessError()},
			new object[] { new KerberoError()},
			new object[] { new InvalidParametersError("VALID_CLIENT_ID") }
		};

	[Theory]
	[MemberData(nameof(PersistentErrorToTest))]
	public async Task Handle_PersistentReturnAnError_Test(KerberoError error)
	{
		// Arrange
		var nukiAccountDto = new NukiAccountExternalResponse
		{
			Token = "VALID_TOKEN",
			RefreshToken = "VALID_REFRESH_TOKEN",
			ClientId = "VALID_CLIENT_ID",
			TokenType = "bearer",
			TokenExpiresIn = 2592000,
		};
		_nukiClient.Setup(c => c.GetNukiAccount(
				It.IsAny<NukiAccountExternalRequest>()))
			.Returns(async () => await Task.FromResult(Result.Ok(nukiAccountDto)));
		_repository.Setup(c => c.Create(It.IsAny<NukiAccount>()))
			.Returns(async () => await Task.FromResult(Result.Fail(error)));

		// Act 
		var ex = await _interactor.Handle(new NukiAccountPresentationRequest
			{ ClientId = "VALID_CLIENT_ID", Code = "VALID_CODE" });
		// Assert
		ex.IsFailed.Should().BeTrue();
		ex.Errors.FirstOrDefault()!.Should().BeEquivalentTo(error);
	}	
	public static IEnumerable<object[]> PersistentErrorToTest =>
		new List<object[]>
		{
			new object[] { new DuplicateEntryError("Nuki account")},
			new object[] { new PersistentResourceNotAvailableError()}
		};

}
