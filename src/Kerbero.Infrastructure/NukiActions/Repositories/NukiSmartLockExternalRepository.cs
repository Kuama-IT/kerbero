using FluentResults;
using Flurl;
using Kerbero.Domain.Common.Errors;
using Kerbero.Domain.NukiActions.Models.ExternalResponses;
using Kerbero.Domain.NukiActions.Repositories;
using Kerbero.Infrastructure.Common.Extensions;
using Kerbero.Infrastructure.Common.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Kerbero.Infrastructure.NukiActions.Repositories;

public class NukiSmartLockExternalRepository: INukiSmartLockExternalRepository
{
	private readonly NukiExternalOptions _options;
	private readonly ILogger<NukiSmartLockExternalRepository> _logger;

	public NukiSmartLockExternalRepository(IOptions<NukiExternalOptions> options, 
		ILogger<NukiSmartLockExternalRepository> logger)
	{
		_options = options.Value;
		_logger = logger;
	}
	
	public async Task<Result<List<NukiSmartLockExternalResponse>>> GetNukiSmartLocks(string accessToken)
	{
		var apiResponse = await $"{_options.BaseUrl}"
			.AppendPathSegments("smartlock")
			.NukiAuthGetJsonAsync<List<NukiSmartLockExternalResponse>>(accessToken, _logger);

		return apiResponse.IsSuccess ? Result.Ok(apiResponse.Value) : apiResponse.ToResult();
	}
}
