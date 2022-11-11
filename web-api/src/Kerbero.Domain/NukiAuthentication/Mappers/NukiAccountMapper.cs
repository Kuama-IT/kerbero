using Kerbero.Domain.NukiAuthentication.Entities;
using Kerbero.Domain.NukiAuthentication.Models;
using Kerbero.Domain.NukiAuthentication.Models.ExternalResponses;
using Kerbero.Domain.NukiAuthentication.Models.PresentationResponses;

namespace Kerbero.Domain.NukiAuthentication.Mappers;

public static class NukiAccountMapper
{
	public static NukiAccount MapToEntity(NukiAccountExternalResponse nukiAccountExternalResponse)
	{
		return new NukiAccount
		{
			Token = nukiAccountExternalResponse.Token,
			RefreshToken = nukiAccountExternalResponse.RefreshToken,
			TokenExpiringTimeInSeconds = nukiAccountExternalResponse.TokenExpiresIn,
			TokenType = nukiAccountExternalResponse.TokenType,
			ClientId = nukiAccountExternalResponse.ClientId,
			// UTC date?
			ExpiryDate =  DateTime.Now.ToUniversalTime().AddSeconds(nukiAccountExternalResponse.TokenExpiresIn)
		};
	}
	public static NukiAccountPresentationResponse MapToPresentation(NukiAccount nukiAccount)
	{
		return new NukiAccountPresentationResponse
		{
			Id = nukiAccount.Id,
			ClientId = nukiAccount.ClientId
		};
	}

	public static AuthenticateRepositoryPresentationResponse MapToAuthenticatedResponse(NukiAccount nukiAccount)
	{
		return new AuthenticateRepositoryPresentationResponse
		{
			NukiAccountId = nukiAccount.Id,
			ClientId = nukiAccount.ClientId,
			Token = nukiAccount.Token!
		};
	}
	
}
