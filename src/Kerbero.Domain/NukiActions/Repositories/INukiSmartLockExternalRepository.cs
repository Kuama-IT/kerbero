using FluentResults;
using Kerbero.Domain.NukiActions.Models;

namespace Kerbero.Domain.NukiActions.Repositories;

public interface INukiSmartLockExternalRepository
{
	Task<Result<NukiSmartLocksListExternalResponseDto>> GetNukiSmartLockList(int authorizationToken);
}
