using FluentAssertions;
using Kerbero.Common.Entities;
using Kerbero.Common.Exceptions;
using Kerbero.Common.Interactors;
using Kerbero.Common.Models;
using Kerbero.Common.Repositories;
using Moq;

namespace Kerbero.Common.Tests.Interactors;

public class CreateNukiAccountInteractorTests
{
	private readonly Mock<INukiPersistentAccountRepository> _repository;
	private readonly CreateNukiAccountInteractor _interactor;
	private readonly Mock<INukiExternalAuthenticationRepository> _nukiClient;

	public CreateNukiAccountInteractorTests()
	{
		_nukiClient = new Mock<INukiExternalAuthenticationRepository>();
		_repository = new Mock<INukiPersistentAccountRepository>();
		_interactor = new CreateNukiAccountInteractor(_repository.Object, _nukiClient.Object);
	}

	// Handle should create an entity from an input DTO and upload it into the DB.
	[Fact]
	public async Task Handle_UploadSuccessfullyInDb_Test()
	{
		// Arrange
		var nukiAccountDto = new NukiAccountExternalResponseDto
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
			It.IsAny<NukiAccountExternalRequestDto>()))
			.Returns(Task.FromResult(nukiAccountDto));
		_repository.Setup(c => 
			c.Create(It.IsAny<NukiAccount>())).Returns(
			() => { nukiAccountEntity.Id = 1; return Task.FromResult(nukiAccountEntity); });

		// Act
		var nukiAccountPresentationDto = await _interactor.Handle(new NukiAccountExternalRequestDto
			{ ClientId = "VALID_CLIENT_ID", Code = "VALID_CODE"});
		
		// Assert
		_nukiClient.Verify(c => c.GetNukiAccount(
			It.Is<NukiAccountExternalRequestDto>(s => 
				s.ClientId.Contains("VALID_CLIENT_ID") &&
				s.Code.Contains("VALID_CODE"))));
		_repository.Verify(c => c
			.Create(It.Is<NukiAccount>(account => 
				account.Token == nukiAccountEntity.Token &&
				account.RefreshToken == nukiAccountEntity.RefreshToken &&
				account.TokenExpiringTimeInSeconds == nukiAccountEntity.TokenExpiringTimeInSeconds &&
				account.TokenType == nukiAccountEntity.TokenType &&
				account.ClientId == nukiAccountEntity.ClientId)));
		nukiAccountPresentationDto.Should().BeEquivalentTo(new NukiAccountPresentationDto()
		{
			Id = 1,
			ClientId = "VALID_CLIENT_ID"
		});
	}

	[Fact]
	public async Task Handle_ThrowsInvalidAccountInfoException_Test()
	{
		// Arrange
		var nukiAccount = new NukiAccountExternalResponseDto
		{
			Token = " ",
			RefreshToken = "VALID_REFRESH_TOKEN",
			ClientId = "VALID_CLIENT_ID",
			TokenType = "bearer",
			TokenExpiresIn = 2592000
		};
		_nukiClient.Setup(c => c.GetNukiAccount(
				It.IsAny<NukiAccountExternalRequestDto>()))
			.Returns(Task.FromResult(nukiAccount));
		// Act
		// Assert
		InvalidTokenException ex = await Assert.ThrowsAsync<InvalidTokenException>(async () => await _interactor.Handle(new NukiAccountExternalRequestDto
			{ ClientId = "VALID_CLIENT_ID", Code = "VALID_CODE"}));
		ex.Message.Should().Match(c => c.Contains("The provider account contains an invalid token."));
	}

	[Fact]
	public async Task Handle_ThrowsExpiredToken_Test()
	{
		// Arrange
		var nukiAccount = new NukiAccountExternalResponseDto
		{
			Token = "VALID_TOKEN",
			RefreshToken = "VALID_REFRESH_TOKEN",
			ClientId = "VALID_CLIENT_ID",
			TokenType = "bearer",
			TokenExpiresIn = 0
		};
		_nukiClient.Setup(c => c.GetNukiAccount(
				It.IsAny<NukiAccountExternalRequestDto>()))
			.Returns(Task.FromResult(nukiAccount));
		// Act
		// Assert
		Exception ex = await Assert.ThrowsAsync<TokenExpiredException>(async () => await _interactor.Handle(new NukiAccountExternalRequestDto
			{ ClientId = "VALID_CLIENT_ID", Code = "VALID_CODE"}));
		ex.Message.Should()
			.Match(c => c.Contains("The provided Token is expired."));
	}

	[Fact]
	public async Task Handle_ThrowsNotTrackedException_Test()
	{
		// Arrange
		var nukiAccountDto = new NukiAccountExternalResponseDto
		{
			Token = "VALID_TOKEN",
			RefreshToken = "VALID_REFRESH_TOKEN",
			ClientId = "VALID_CLIENT_ID",
			TokenType = "bearer",
			TokenExpiresIn = 2592000,
		};
		_nukiClient.Setup(c => c.GetNukiAccount(
				It.IsAny<NukiAccountExternalRequestDto>()))
			.Returns(Task.FromResult(nukiAccountDto));
		_repository.Setup(c =>  c
				.Create(It.IsAny<NukiAccount>( )))
				.Returns(() => Task.FromResult(
					new NukiAccount
					{
						Token = "VALID_TOKEN",
						RefreshToken = "VALID_REFRESH_TOKEN",
						TokenExpiringTimeInSeconds = 2592000,
						ClientId = "VALID_CLIENT_ID",
						TokenType = "bearer"
					}));

		// Act 
		// Assert
		Exception ex = await Assert.ThrowsAsync<AccountNotTrackedException>(async () => await _interactor.Handle(new NukiAccountExternalRequestDto
			{ ClientId = "VALID_CLIENT_ID", Code = "VALID_CODE"}));
		((AccountNotTrackedException)ex).Message.Should().Contain("The account does not provide an identification");
	}
}
