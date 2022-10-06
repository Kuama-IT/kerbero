using System.Net;
using FluentResults;
using Flurl;
using Flurl.Http;
using Kerbero.Domain.Common.Errors;
using Kerbero.Domain.Common.Errors.CommonErrors;
using Kerbero.Domain.Common.Errors.CreateNukiAccountErrors;
using Kerbero.Domain.NukiAuthentication.Models;
using Kerbero.Domain.NukiAuthentication.Repositories;
using Kerbero.Infrastructure.NukiAuthentication.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ArgumentNullException = System.ArgumentNullException;

namespace Kerbero.Infrastructure.NukiAuthentication.Repositories;

public class NukiAccountExternalRepository: INukiAccountExternalRepository
{
	private readonly NukiExternalOptions _options;
	private readonly ILogger<NukiAccountExternalRepository> _logger;

	public NukiAccountExternalRepository(IOptions<NukiExternalOptions> options, ILogger<NukiAccountExternalRepository> logger)
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
		try
		{
			var response = await $"{_options.BaseUrl}"
				.AppendPathSegment("oauth")
				.AppendPathSegment("token")
				.PostJsonAsync(postBody).ReceiveJson<NukiAccountExternalResponseDto>();

			if (response is null)
			{
				return Result.Fail(new UnableToParseResponseError("Response is null"));
			}

			response.ClientId = clientId;
			return response;
		}
		#region ErrorManagement
		catch (FlurlHttpException ex)
		{
			_logger.LogError(ex, "Error while retrieving tokens from Nuki Web");
			if (ex.StatusCode == (int)HttpStatusCode.Unauthorized)
			{
				var error = await ex.GetResponseJsonAsync<NukiErrorExternalResponseDto>();
				if (error?.Error != null && error.Error.Contains("invalid"))
					return Result.Fail(new InvalidParametersError(error.Error + ": " + error.ErrorMessage));
				return Result.Fail(new UnauthorizedAccessError());
			}

			if (ex.StatusCode == (int)HttpStatusCode.RequestTimeout || ex.StatusCode % 100 == 5)
			{
				return Result.Fail(new ExternalServiceUnreachableError());
			}

			return Result.Fail(new UnknownExternalError());
		}
		catch (ArgumentNullException)
		{
			return Result.Fail(new InvalidParametersError("options"));
		}
		catch
		{
			return Result.Fail(new KerberoError());
		}

		#endregion
	}
}
