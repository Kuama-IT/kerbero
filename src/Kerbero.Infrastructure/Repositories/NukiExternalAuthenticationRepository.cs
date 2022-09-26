using System.Net;
using FluentResults;
using Flurl;
using Flurl.Http;
using Kerbero.Common.Errors;
using Kerbero.Common.Models;
using Kerbero.Common.Repositories;
using Kerbero.Infrastructure.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Kerbero.Infrastructure.Repositories;

public class NukiExternalAuthenticationRepository: INukiExternalAuthenticationRepository
{
	private readonly NukiExternalOptions _options;
	private readonly ILogger<NukiExternalAuthenticationRepository> _logger;

	public NukiExternalAuthenticationRepository(IOptions<NukiExternalOptions> options, ILogger<NukiExternalAuthenticationRepository> logger)
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
		try
		{
			var redirectUriClientId = $"{_options.MainDomain}"
				.AppendPathSegment(_options.RedirectUriForCode)
				.AppendPathSegment(nukiAccountExternalRequestDto.ClientId);

			var response = await $"{_options.BaseUrl}"
				.AppendPathSegment("oauth")
				.AppendPathSegment("token")
				.PostJsonAsync(new
				{
					client_id = nukiAccountExternalRequestDto.ClientId,
					client_secret = _options.ClientSecret,
					grant_type = "authorization_code",
					code = nukiAccountExternalRequestDto.Code,
					redirect_uri = redirectUriClientId,
				}).ReceiveJson<NukiAccountExternalResponseDto>();
			
			if (response is null)
			{
				return Result.Fail(new UnableToParseResponseError("Response is null"));
			}
			response.ClientId = nukiAccountExternalRequestDto.ClientId;
			return response;
		}
		#region ErrorManagement
		catch (FlurlHttpException ex)
		{
			_logger.LogError(ex, "Error while retrieving tokens from Nuki Web");
			if (ex.StatusCode == (int)HttpStatusCode.Unauthorized)
			{
				var error = await ex.GetResponseJsonAsync<NukiAccountExternalResponseDto>();
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
