using Flurl;
using Flurl.Http;
using Kerbero.Common.Exceptions;
using Kerbero.Common.Models;
using Kerbero.Common.Repositories;
using Kerbero.Infrastructure.Options;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Kerbero.Infrastructure.Clients;

public class NukiExternalAuthenticationRepository: INukiExternalAuthenticationRepository
{
	private readonly NukiExternalOptions _options;

	public NukiExternalAuthenticationRepository(IOptions<NukiExternalOptions> options)
	{
		_options = options.Value;
	}

	/// <summary>
	/// Builds a Uri where the user who wants to authenticate should be redirected
	/// </summary>
	/// <param name="clientId"></param>
	/// <returns />
	public Uri BuildUriForCode(string clientId)
	{
		if (string.IsNullOrEmpty(clientId)) throw new InvalidClientIdException(clientId, InvalidClientIdException.Reason.EmptyOrNull);
		var redirectUriClientId = $"{_options.MainDomain}"
			.AppendPathSegment(_options.RedirectUriForCode)
			.AppendPathSegment(clientId);
		return $"{_options.BaseUrl}"
			.AppendPathSegments("oauth", "authorize")
			.SetQueryParams(new
			{
				response_type = "code",
				client_id = clientId,
				redirect_uri = redirectUriClientId.ToString(),
				scope = _options.Scopes,
			})
			.ToUri();
	}

	/// <summary>
	///  Retrieves an authentication token from the Nuki Apis
	/// </summary>
	/// <param name="externalRequestDto"></param>
	/// <returns></returns>
	/// <exception cref="InvalidClientIdException"></exception>
	public async Task<NukiAccountExternalResponseDto> GetNukiAccount(NukiAccountExternalRequestDto externalRequestDto)
	{
		if (string.IsNullOrWhiteSpace(externalRequestDto.ClientId)) throw new InvalidClientIdException(externalRequestDto.ClientId, InvalidClientIdException.Reason.EmptyOrNull);
		var redirectUriClientId = $"{_options.MainDomain}"
			.AppendPathSegment(_options.RedirectUriForCode)
			.AppendPathSegment(externalRequestDto.ClientId);
		var response = await $"{_options.BaseUrl}"
		  .AppendPathSegment("oauth")
		  .AppendPathSegment("token")
		  .PostJsonAsync(new
		  {
		    client_id = externalRequestDto.ClientId,
		    client_secret = _options.ClientSecret,
		    grant_type = "authorization_code",
		    code = externalRequestDto.Code,
		    redirect_uri = redirectUriClientId,
		  }).ReceiveString(); // default Json converter not working
		var deserializeObject = JsonConvert.DeserializeObject<dynamic>(response);
		if (deserializeObject is not null && deserializeObject.access_token is not null)
			return new NukiAccountExternalResponseDto()
			{
				Token = deserializeObject.access_token,
				ClientId = externalRequestDto.ClientId,
				RefreshToken = deserializeObject.refresh_token,
				TokenType = deserializeObject.token_type,
				TokenExpiresIn = deserializeObject.expires_in
			};
		throw new InconsistentApiResponseException(deserializeObject, "");
	}
}