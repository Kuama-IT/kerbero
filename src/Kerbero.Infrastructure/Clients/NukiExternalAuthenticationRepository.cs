using System.Net;
using FluentResults;
using Flurl;
using Flurl.Http;
using Kerbero.Common.Errors;
using Kerbero.Common.Models;
using Kerbero.Common.Repositories;
using Kerbero.Infrastructure.Options;
using Microsoft.Extensions.Options;

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
	public Result<Uri> BuildUriForCode(string clientId)
	{
		if (string.IsNullOrEmpty(clientId)) return Result.Fail(new InvalidParametersError("client_id"));
		try
		{
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
		catch (ArgumentNullException)
		{
			return Result.Fail(new InvalidParametersError("options"));
		}
		catch
		{
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
		var redirectUriClientId = $"{_options.MainDomain}"
			.AppendPathSegment(_options.RedirectUriForCode)
			.AppendPathSegment(nukiAccountExternalRequestDto.ClientId);
		try
		{
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
		// generic catch is not useful, because FlurlHttpException catch everything
		catch (FlurlHttpException ex)
		{
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

		#endregion
	}
}
