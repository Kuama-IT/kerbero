using FluentResults;
using Kerbero.Domain.NukiAuthentication.Entities;

namespace Kerbero.Domain.NukiAuthentication.Repositories;

public interface INukiPersistentAccountRepository
{
	Task<Result<NukiAccount>> Create(NukiAccount nukiAccount);
}
