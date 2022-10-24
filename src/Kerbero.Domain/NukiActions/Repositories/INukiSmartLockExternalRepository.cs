using FluentResults;
using Kerbero.Domain.NukiActions.Models.ExternalRequests;
using Kerbero.Domain.NukiActions.Models.ExternalResponses;
using Kerbero.Domain.NukiAuthentication.Interfaces;

namespace Kerbero.Domain.NukiActions.Repositories;

public interface INukiSmartLockExternalRepository
{
	Task<Result<List<NukiSmartLockExternalResponse>>> GetNukiSmartLocks(string accessToken);
	Task<Result<NukiSmartLockExternalResponse>> GetNukiSmartLock(NukiSmartLockExternalRequest request);
}
