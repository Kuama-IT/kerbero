using Kerbero.Data.NukiCredentials.Entities;
using Kerbero.Data.SmartLockKeys.Entities;
using Microsoft.EntityFrameworkCore;

namespace Kerbero.Data.Common.Interfaces;

public interface IApplicationDbContext
{
  DbSet<NukiCredentialEntity> NukiCredentials { get; }
  DbSet<UserNukiCredentialEntity> UserNukiCredentials { get; }
  DbSet<SmartLockKeyEntity> SmartLockKeys { get; }

  Task<int> SaveChangesAsync(CancellationToken cancellationToken = default); // with or without CancellationToken
}
