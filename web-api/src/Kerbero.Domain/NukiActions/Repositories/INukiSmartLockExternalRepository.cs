using FluentResults;
using Kerbero.Domain.NukiActions.Models.ExternalRequests;
using Kerbero.Domain.NukiActions.Models.ExternalResponses;

namespace Kerbero.Domain.NukiActions.Repositories;

public interface INukiSmartLockExternalRepository
{
	Task<Result> CloseNukiSmartLock(NukiSmartLockExternalRequest requestDto);
	Task<Result<List<NukiSmartLockExternalResponse>>> GetNukiSmartLocks(string accessToken);
	Task<Result<NukiSmartLockExternalResponse>> GetNukiSmartLock(NukiSmartLockExternalRequest request);
	Task<Result> OpenNukiSmartLock(NukiSmartLockExternalRequest request);
}
