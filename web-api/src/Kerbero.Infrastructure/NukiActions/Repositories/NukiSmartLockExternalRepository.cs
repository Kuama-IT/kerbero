using FluentResults;
using Flurl;
using Flurl.Http;
using Kerbero.Domain.NukiActions.Models.ExternalRequests;
using Kerbero.Domain.NukiActions.Models.ExternalResponses;
using Kerbero.Domain.NukiActions.Repositories;
using Kerbero.Infrastructure.Common.Helpers;
using Microsoft.Extensions.Configuration;

namespace Kerbero.Infrastructure.NukiActions.Repositories;

public class NukiSmartLockExternalRepository: INukiSmartLockExternalRepository
{
	private readonly NukiSafeHttpCallHelper _nukiSafeHttpCallHelper;
	private readonly string _domainAlias;

	public NukiSmartLockExternalRepository(IConfiguration configuration, NukiSafeHttpCallHelper nukiSafeHttpCallHelper)
	{
		_domainAlias = configuration["ALIAS_DOMAIN"] ?? throw new ArgumentNullException(configuration["ALIAS_DOMAIN"]);	
		_nukiSafeHttpCallHelper = nukiSafeHttpCallHelper;
	}

	public async Task<Result<List<NukiSmartLockExternalResponse>>> GetNukiSmartLocks(string accessToken)
	{
		var apiResponse = await _nukiSafeHttpCallHelper.Handle( () => _domainAlias
			.AppendPathSegments("smartlock")
			.WithOAuthBearerToken(accessToken)
			.GetJsonAsync<List<NukiSmartLockExternalResponse>>());

		return apiResponse.IsSuccess ? Result.Ok(apiResponse.Value) : apiResponse.ToResult();
	}
	public async Task<Result<NukiSmartLockExternalResponse>> GetNukiSmartLock(NukiSmartLockExternalRequest request)
	{
		return await _nukiSafeHttpCallHelper.Handle( () =>  _domainAlias
			.AppendPathSegment("smartlock")
			.AppendPathSegment(request.ExternalId)
			.WithOAuthBearerToken(request.AccessToken)
			.GetJsonAsync<NukiSmartLockExternalResponse>());
	}

	public async Task<Result> OpenNukiSmartLock(NukiSmartLockExternalRequest request)
	{
		var response = await _nukiSafeHttpCallHelper.Handle(() => _domainAlias
			.AppendPathSegments("smartlock", 
				$"{request.ExternalId}", 
				"action", 
				"unlock")
			.WithOAuthBearerToken(request.AccessToken)
			.WithHeader("accept", "application/json")
			.PostAsync());
		return response.IsFailed ? response.ToResult() : Result.Ok();
	}
	
	public async Task<Result> CloseNukiSmartLock(NukiSmartLockExternalRequest request)
	{
		var response = await _nukiSafeHttpCallHelper.Handle( () => _domainAlias
			.AppendPathSegments("smartlock", 
				request.ExternalId, 
				"action", 
				"lock")
			.WithOAuthBearerToken(request.AccessToken)
			.PostAsync());
		return response.IsFailed ? response.ToResult() : Result.Ok();
	}
}
