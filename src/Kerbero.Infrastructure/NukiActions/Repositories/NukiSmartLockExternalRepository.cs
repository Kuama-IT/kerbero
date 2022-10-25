using FluentResults;
using Flurl;
using Flurl.Http;
using Kerbero.Domain.NukiActions.Models.ExternalRequests;
using Kerbero.Domain.NukiActions.Models.ExternalResponses;
using Kerbero.Domain.NukiActions.Repositories;
using Kerbero.Infrastructure.Common.Helpers;
using Kerbero.Infrastructure.Common.Options;
using Microsoft.Extensions.Options;

namespace Kerbero.Infrastructure.NukiActions.Repositories;

public class NukiSmartLockExternalRepository: INukiSmartLockExternalRepository
{
	private readonly NukiSafeHttpCallHelper _nukiSafeHttpCallHelper;
	private readonly NukiExternalOptions _options;

	public NukiSmartLockExternalRepository(IOptions<NukiExternalOptions> options, NukiSafeHttpCallHelper nukiSafeHttpCallHelper)
	{
		_nukiSafeHttpCallHelper = nukiSafeHttpCallHelper;
		_options = options.Value;
	}

	public async Task<Result<List<NukiSmartLockExternalResponse>>> GetNukiSmartLocks(string accessToken)
	{
		var apiResponse = await _nukiSafeHttpCallHelper.Handle( () => _options.BaseUrl
			.AppendPathSegments("smartlock")
			.WithOAuthBearerToken(accessToken)
			.GetJsonAsync<List<NukiSmartLockExternalResponse>>());

		return apiResponse.IsSuccess ? Result.Ok(apiResponse.Value) : apiResponse.ToResult();
	}

	public Task<Result<NukiSmartLockExternalResponse>> GetNukiSmartLock(NukiSmartLockExternalRequest request)
	{
		throw new NotImplementedException();
	}

	public Task<Result> OpenNukiSmartLock(NukiSmartLockExternalRequest request)
	{
		throw new NotImplementedException();
	}
	
	public Task<Result> CloseNukiSmartLock(NukiSmartLockExternalRequest requestDto)
	{
		throw new NotImplementedException();
	}
}
