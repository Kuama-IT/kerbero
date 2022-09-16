using FluentResults;
using Kerbero.Common.Entities;

namespace Kerbero.Common.Repositories;

public interface INukiPersistentAccountRepository
{
	Task<Result<NukiAccount>> Create(NukiAccount nukiAccount);
}
