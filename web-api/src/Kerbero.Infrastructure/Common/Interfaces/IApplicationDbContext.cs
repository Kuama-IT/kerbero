using Kerbero.Domain.NukiActions.Entities;
using Kerbero.Infrastructure.NukiAuthentication.Entities;
using Microsoft.EntityFrameworkCore;

namespace Kerbero.Infrastructure.Common.Interfaces;

public interface IApplicationDbContext
{
  DbSet<NukiCredentialEntity> NukiCredentials { get; }
  DbSet<NukiCredentialDraftEntity> NukiCredentialDrafts { get; }

  DbSet<NukiSmartLockEntity> NukiSmartLocks { get; }
  DbSet<NukiSmartLockStateEntity> NukiSmartLockStates { get; }
  DbSet<UserNukiCredentialEntity> UserNukiCredentials { get; }

  Task<int> SaveChangesAsync(CancellationToken cancellationToken = default); // with or without CancellationToken
}