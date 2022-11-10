using FluentResults;
using Kerbero.Domain.NukiAuthentication.Entities;

namespace Kerbero.Domain.NukiAuthentication.Repositories;

public interface INukiAccountPersistentRepository
{
	Task<Result<NukiAccount>> Create(NukiAccount nukiAccount);
	Task<Result<NukiAccount>> GetById(int id);
	Task<Result<NukiAccount>> Update(NukiAccount nukiAccount);
}
