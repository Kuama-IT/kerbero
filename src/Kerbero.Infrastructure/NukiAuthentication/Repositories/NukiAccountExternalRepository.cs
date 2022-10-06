using FluentResults;
using Flurl;
using Kerbero.Domain.Common.Errors;
using Kerbero.Domain.NukiAuthentication.Models;
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
	/// <param name="request"></param>
	/// <returns />
	public Result<NukiRedirectPresentationDto> BuildUriForCode(NukiRedirectExternalRequestDto request)
	{
		if (string.IsNullOrEmpty(request.ClientId)) return Result.Fail(new InvalidParametersError("client_id"));
		try
		{
			var redirectUriClientId = $"{_options.MainDomain}"
				.AppendPathSegment(_options.RedirectUriForCode)
				.AppendPathSegment(request.ClientId);
			return Result.Ok(new NukiRedirectPresentationDto($"{_options.BaseUrl}"
				.AppendPathSegments("oauth", "authorize")
				.SetQueryParams(new
				{
					response_type = "code",
					client_id = request.ClientId,
					redirect_uri = redirectUriClientId.ToString(),
					scope = _options.Scopes,
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
	/// <param name="nukiAccountExternalRequestDto"></param>
	/// <returns></returns>
	public async Task<Result<NukiAccountExternalResponseDto>> GetNukiAccount(NukiAccountExternalRequestDto nukiAccountExternalRequestDto)
	{
		if (string.IsNullOrWhiteSpace(nukiAccountExternalRequestDto.ClientId)) return Result.Fail(new InvalidParametersError("client_id"));
		Url redirectUriClientId;
		try
		{
			redirectUriClientId = $"{_options.MainDomain}"
				.AppendPathSegment(_options.RedirectUriForCode)
				.AppendPathSegment(nukiAccountExternalRequestDto.ClientId);
		}
		catch (ArgumentNullException e)
		{
			_logger.LogError(e, "Error while calling nuki Apis with request: {Message}", e.Message);
			return Result.Fail(new InvalidParametersError("options"));
		}
		
		var result = await AuthRequest(nukiAccountExternalRequestDto.ClientId, new
		{
			client_id = nukiAccountExternalRequestDto.ClientId,
			client_secret = _options.ClientSecret,
			grant_type = "authorization_code",
			code = nukiAccountExternalRequestDto.Code,
			redirect_uri = redirectUriClientId.ToString()
		});
		return result;
	}

	/// <summary>
	///  Update the authentication token with refresh token
	/// </summary>
	/// <param name="nukiAccountExternalRequestDto"></param>
	/// <returns></returns>
	public async Task<Result<NukiAccountExternalResponseDto>> RefreshToken(NukiAccountExternalRequestDto nukiAccountExternalRequestDto)
	{
		if (string.IsNullOrWhiteSpace(nukiAccountExternalRequestDto.ClientId)) return Result.Fail(new InvalidParametersError("client_id"));

		return await AuthRequest(nukiAccountExternalRequestDto.ClientId, new
		{
			client_id = nukiAccountExternalRequestDto.ClientId,
			client_secret = _options.ClientSecret,
			grant_type = "refresh_token",
			refresh_token = nukiAccountExternalRequestDto.RefreshToken,
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
