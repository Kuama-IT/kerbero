using System.Dynamic;
using Flurl;
using Flurl.Http;
using Kerbero.Common.Entities;
using Kerbero.Common.Exceptions;
using Kerbero.Infrastructure.Options;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Kerbero.Infrastructure.Clients;

public class NukiHttpClient
{
	private readonly NukiClientOptions _options;

	public NukiHttpClient(IOptions<NukiClientOptions> options)
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
	/// <param name="clientId"></param>
	/// <param name="authCode"></param>
	/// <returns></returns>
	/// <exception cref="InvalidClientIdException"></exception>
	public async Task<NukiAccountEntity> GetAuthenticatedProvider(string clientId, string authCode)
	{
		if (string.IsNullOrWhiteSpace(clientId)) throw new InvalidClientIdException(clientId, InvalidClientIdException.Reason.EmptyOrNull);
		var redirectUriClientId = $"{_options.MainDomain}"
			.AppendPathSegment(_options.RedirectUriForCode)
			.AppendPathSegment(clientId);
		var response = await $"{_options.BaseUrl}"
		  .AppendPathSegment("oauth")
		  .AppendPathSegment("token")
		  .PostJsonAsync(new
		  {
		    client_id = clientId,
		    client_secret = _options.ClientSecret,
		    grant_type = "authorization_code",
		    code = authCode,
		    redirect_uri = redirectUriClientId,
		  }).ReceiveString(); // default Json converter not working
		var deserializeObject = JsonConvert.DeserializeObject<dynamic>(response);
		if (deserializeObject is not null && deserializeObject.access_token is not null)
			return new NukiAccountEntity()
			{
				Token = deserializeObject.access_token,
				ClientId = clientId,
				RefreshToken = deserializeObject.refresh_token,
				TokenType = deserializeObject.token_type,
				TokenExpiresIn = deserializeObject.expires_in
			};
		throw new InconsistentApiResponseException(deserializeObject, "");
	}
}