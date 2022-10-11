using FluentAssertions;
using FluentResults;
using Kerbero.Domain.Common.Interfaces;
using Kerbero.Domain.NukiAuthentication.Interactors;
using Kerbero.Domain.NukiAuthentication.Models;
using Kerbero.Domain.NukiAuthentication.Repositories;
using Moq;

namespace Kerbero.Domain.Tests.Interactors;

public class ProvideNukiAuthRedirectUrlTest
{
	private readonly ProvideNukiAuthRedirectUrlInteractor _redirectInteractor;
	private readonly Mock<INukiAccountExternalRepository> _nukiExternalRepository;

	public ProvideNukiAuthRedirectUrlTest()
	{
		_nukiExternalRepository = new Mock<INukiAccountExternalRepository>();
		_redirectInteractor = new ProvideNukiAuthRedirectUrlInteractor(_nukiExternalRepository.Object);
	}

	[Fact]
	public void Handle_Success()
	{
		// Arrange
		var uri = new Uri("http://api.nuki.io/oauth/authorize?response_type=code" +
		                  "&client_id=v7kn_NX7vQ7VjQdXFGK43g" +
		                  "&redirect_uri=https%3A%2F%2Ftest.com%2Fnuki%2Fcode%2Fv7kn_NX7vQ7VjQdXFGK43g" +
		                  "&scope=account notification smartlock smartlock.readOnly smartlock.action smartlock.auth smartlock.config smartlock.log");
		_nukiExternalRepository.Setup(c => c.BuildUriForCode(It.IsAny<NukiRedirectExternalRequestDto>()))
			.Returns(new NukiRedirectPresentationDto(uri));

		// Act
		var redirectUri = _redirectInteractor.Handle(new NukiRedirectExternalRequestDto("VALID_CLIENT_ID"));

		// Assert
		_redirectInteractor.Should().BeAssignableTo<Interactor<NukiRedirectExternalRequestDto, NukiRedirectPresentationDto>>();
		_nukiExternalRepository.Verify(c => c.BuildUriForCode(It.Is<NukiRedirectExternalRequestDto>(s => 
			s.ClientId.Equals("VALID_CLIENT_ID"))));
		redirectUri.Should().BeOfType<Result<NukiRedirectPresentationDto>>();
		redirectUri.Value.RedirectUri.Should().BeEquivalentTo(uri);
	}
}
