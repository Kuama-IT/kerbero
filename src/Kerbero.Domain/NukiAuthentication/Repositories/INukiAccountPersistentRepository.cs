using FluentResults;
using Kerbero.Domain.NukiAuthentication.Entities;

namespace Kerbero.Domain.NukiAuthentication.Repositories;

public interface INukiAccountPersistentRepository
{
	Task<Result<NukiAccount>> Create(NukiAccount nukiAccount);
	Result<NukiAccount> GetAccount(int kerberoAccountId);
	Task<Result<NukiAccount>> Update(NukiAccount nukiAccount);
}
