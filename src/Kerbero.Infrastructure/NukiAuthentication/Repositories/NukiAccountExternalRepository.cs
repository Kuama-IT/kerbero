using FluentResults;
using Flurl;
using Kerbero.Domain.Common.Errors;
using Kerbero.Domain.NukiAuthentication.Models;
using Kerbero.Domain.NukiAuthentication.Models.ExternalRequests;
using Kerbero.Domain.NukiAuthentication.Models.ExternalResponses;
using Kerbero.Domain.NukiAuthentication.Models.PresentationResponses;
using Kerbero.Domain.NukiAuthentication.Repositories;
using Kerbero.Infrastructure.Common.Extensions;
using Kerbero.Infrastructure.NukiAuthentication.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ArgumentNullException = System.ArgumentNullException;

namespace Kerbero.Infrastructure.NukiAuthentication.Repositories;

public class NukiAccountExternalRepository: INukiAccountExternalRepository
{
	private readonly NukiExternalOptions _options;
	private readonly ILogger<NukiAccountExternalRepository> _logger;

	public NukiAccountExternalRepository(IOptions<NukiExternalOptions> options,
		ILogger<NukiAccountExternalRepository> logger)
	{
		_options = options.Value;
		_logger = logger;
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
			var redirectUriClientId = $"{_options.MainDomain}"
				.AppendPathSegment(_options.RedirectUriForCode)
				.AppendPathSegment(redirectExternalRequest.ClientId);
			return Result.Ok(new NukiRedirectPresentationResponse($"{_options.BaseUrl}"
				.AppendPathSegments("oauth", "authorize")
				.SetQueryParams(new
				{
					response_type = "code",
					client_id = redirectExternalRequest.ClientId,
					redirect_uri = redirectUriClientId.ToString(),
					scope = _options.Scopes
				})
				.ToUri()));
		}
		catch (ArgumentNullException e)
		{
			_logger.LogError(e, "Error while building redirect URI");
			return Result.Fail(new InvalidParametersError("options"));
		}
		catch(Exception e)
		{
			_logger.LogError(e, "Error while building redirect URI");
			return Result.Fail(new KerberoError());
		}
	}

	/// <summary>
	///  Retrieves an authentication token from the Nuki Apis
	/// </summary>
	/// <param name="accountExternalRequest"></param>
	/// <returns></returns>
	public async Task<Result<NukiAccountExternalResponseDto>> GetNukiAccount(NukiAccountExternalRequest accountExternalRequest)
	{
		if (string.IsNullOrWhiteSpace(accountExternalRequest.ClientId)) return Result.Fail(new InvalidParametersError("client_id"));
		Url redirectUriClientId;
		try
		{
			redirectUriClientId = $"{_options.MainDomain}"
				.AppendPathSegment(_options.RedirectUriForCode)
				.AppendPathSegment(accountExternalRequest.ClientId);
		}
		catch (ArgumentNullException e)
		{
			_logger.LogError(e, "Error while calling nuki Apis with redirectExternalRequest: {Message}", e.Message);
			return Result.Fail(new InvalidParametersError("options"));
		}
		
		var result = await AuthRequest(accountExternalRequest.ClientId, new
		{
			client_id = accountExternalRequest.ClientId,
			client_secret = _options.ClientSecret,
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
	public async Task<Result<NukiAccountExternalResponseDto>> RefreshToken(NukiAccountExternalRequest accountExternalRequest)
	{
		if (string.IsNullOrWhiteSpace(accountExternalRequest.ClientId)) return Result.Fail(new InvalidParametersError("client_id"));

		return await AuthRequest(accountExternalRequest.ClientId, new
		{
			client_id = accountExternalRequest.ClientId,
			client_secret = _options.ClientSecret,
			grant_type = "refresh_token",
			refresh_token = accountExternalRequest.RefreshToken
		});
	}

	private async Task<Result<NukiAccountExternalResponseDto>> AuthRequest(string clientId, object postBody)
	{
		var response = await $"{_options.BaseUrl}"
			.AppendPathSegment("oauth")
			.AppendPathSegment("token")
			.NukiPostJsonAsync<NukiAccountExternalResponseDto>(postBody, _logger);

		if (response.IsFailed)
		{
			return response.ToResult();
		}

		response.Value.ClientId = clientId;
		return response;
	}
}
