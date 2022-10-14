using FluentResults;
using Kerbero.Domain.NukiActions.Models;
using Kerbero.Domain.NukiAuthentication.Entities;

namespace Kerbero.Domain.NukiActions.Repositories;

public interface INukiSmartLockExternalRepository
{
	Task<Result<NukiSmartLocksListExternalResponseDto>> GetNukiSmartLockList();
	void Authenticate(NukiAccount nukiAccount);
}
