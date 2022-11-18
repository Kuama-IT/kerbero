using FluentResults;
using Flurl;
using Flurl.Http;
using Kerbero.Domain.Common.Errors;
using Kerbero.Domain.NukiAuthentication.Models.ExternalRequests;
using Kerbero.Domain.NukiAuthentication.Models.ExternalResponses;
using Kerbero.Domain.NukiAuthentication.Models.PresentationResponses;
using Kerbero.Domain.NukiAuthentication.Repositories;
using Kerbero.Infrastructure.Common.Helpers;
using Microsoft.Extensions.Configuration;
using ArgumentNullException = System.ArgumentNullException;

namespace Kerbero.Infrastructure.NukiAuthentication.Repositories;

public class NukiAccountExternalRepository: INukiAccountExternalRepository
{
	private readonly IConfiguration _configuration;
	private readonly NukiSafeHttpCallHelper _nukiSafeHttpCallHelper;

	public NukiAccountExternalRepository(IConfiguration configuration,
		NukiSafeHttpCallHelper nukiSafeHttpCallHelper)
	{
		_configuration = configuration;
		_nukiSafeHttpCallHelper = nukiSafeHttpCallHelper;
	}

	/// <summary>
	/// Builds a Uri where the user who wants to authenticate should be redirected
	/// </summary>
	/// <param name="redirectExternalRequest"></param>
	/// <returns />
	public Result<NukiRedirectPresentationResponse> BuildUriForCode(NukiRedirectExternalRequest redirectExternalRequest)
	{
		if (string.IsNullOrEmpty(redirectExternalRequest.ClientId)) return Result.Fail(new InvalidParametersError("client_id"));
		try
		{
			var redirectUriClientId = _configuration["ALIAS_DOMAIN"]
				.AppendPathSegment(_configuration["NUKI_REDIRECT_FOR_TOKEN"])
				.AppendPathSegment(redirectExternalRequest.ClientId);
			return Result.Ok(new NukiRedirectPresentationResponse(_configuration["NUKI_DOMAIN"]
				.AppendPathSegments("oauth", "authorize")
				.SetQueryParams(new
				{
					response_type = "code",
					client_id = redirectExternalRequest.ClientId,
					redirect_uri = redirectUriClientId.ToString(),
					scope = _configuration["NUKI_SCOPES"]
				})
				.ToUri()));
		}
		catch (ArgumentNullException)
		{
			return Result.Fail(new InvalidParametersError("options"));
		}
		catch(Exception)
		{
			return Result.Fail(new KerberoError());
		}
	}

	/// <summary>
	///  Retrieves an authentication token from the Nuki Apis
	/// </summary>
	/// <param name="accountExternalRequest"></param>
	/// <returns></returns>
	public async Task<Result<NukiAccountExternalResponse>> GetNukiAccount(NukiAccountExternalRequest accountExternalRequest)
	{
		if (string.IsNullOrWhiteSpace(accountExternalRequest.ClientId)) return Result.Fail(new InvalidParametersError("client_id"));
		Url redirectUriClientId;
		try
		{
			redirectUriClientId = _configuration["ALIAS_DOMAIN"]
				.AppendPathSegment(_configuration["NUKI_REDIRECT_FOR_TOKEN"])
				.AppendPathSegment(accountExternalRequest.ClientId);
		}
		catch (ArgumentNullException)
		{
			return Result.Fail(new InvalidParametersError("options"));
		}
		
		var result = await AuthRequest(accountExternalRequest.ClientId, new
		{
			client_id = accountExternalRequest.ClientId,
			client_secret = _configuration["NUKI_CLIENT_SECRET"],
			grant_type = "authorization_code",
			code = accountExternalRequest.Code,
			redirect_uri = redirectUriClientId.ToString()
		});
		return result;
	}

	/// <summary>
	///  Update the authentication token with refresh token
	/// </summary>
	/// <param name="accountExternalRequest"></param>
	/// <returns></returns>
	public async Task<Result<NukiAccountExternalResponse>> RefreshToken(NukiAccountExternalRequest accountExternalRequest)
	{
		if (string.IsNullOrWhiteSpace(accountExternalRequest.ClientId)) return Result.Fail(new InvalidParametersError("client_id"));

		return await AuthRequest(accountExternalRequest.ClientId, new
		{
			client_id = accountExternalRequest.ClientId,
			client_secret = _configuration["NUKI_CLIENT_SECRET"],
			grant_type = "refresh_token",
			refresh_token = accountExternalRequest.RefreshToken,
			scope = _configuration["NUKI_SCOPES"]
		});
	}

	private async Task<Result<NukiAccountExternalResponse>> AuthRequest(string clientId, object postBody)
	{
		var response = await _nukiSafeHttpCallHelper.Handle(
			async () => await _configuration["ALIAS_DOMAIN"]
					.AppendPathSegment("oauth")
					.AppendPathSegment("token")
					.PostUrlEncodedAsync(postBody)
					.ReceiveJson<NukiAccountExternalResponse>());

		if (response.IsFailed)
		{
			return response.ToResult();
		}

		response.Value.ClientId = clientId;
		return response;
	}
}
