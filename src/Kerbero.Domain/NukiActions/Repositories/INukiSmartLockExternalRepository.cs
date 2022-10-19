using FluentResults;
using Kerbero.Domain.NukiActions.Models;
using Kerbero.Domain.NukiAuthentication.Entities;

namespace Kerbero.Domain.NukiActions.Repositories;

public interface INukiSmartLockExternalRepository
{
	Task<Result<List<NukiSmartLockExternalResponseDto>>> GetNukiSmartLockList();
	void Authenticate(NukiAccount nukiAccount);
}
