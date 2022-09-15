using Kerbero.Common.Entities;

namespace Kerbero.Common.Repositories;

public interface INukiPersistentAccountRepository
{
	Task<NukiAccount> Create(NukiAccount nukiAccount);
}
