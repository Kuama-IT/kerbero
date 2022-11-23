using Kerbero.Domain.NukiActions.Entities;
using Kerbero.Domain.NukiAuthentication.Entities;
using Kerbero.Domain.SmartLockKeys.Entities;
using Microsoft.EntityFrameworkCore;

namespace Kerbero.Infrastructure.Common.Interfaces;

public interface IApplicationDbContext
{
	DbSet<NukiAccount> NukiAccounts { get; }
	DbSet<NukiSmartLock> NukiSmartLocks { get; }
	DbSet<NukiSmartLockState> NukiSmartLockStates { get; }
	DbSet<SmartLockKey> SmartLockKeys { get; }

	Task<int> SaveChangesAsync(CancellationToken cancellationToken = default); // with or without CancellationToken
}
