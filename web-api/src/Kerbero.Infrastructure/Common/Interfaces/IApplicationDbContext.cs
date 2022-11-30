using Kerbero.Infrastructure.NukiCredentials.Entities;
using Microsoft.EntityFrameworkCore;

namespace Kerbero.Infrastructure.Common.Interfaces;

public interface IApplicationDbContext
{
  DbSet<NukiCredentialEntity> NukiCredentials { get; }
  DbSet<UserNukiCredentialEntity> UserNukiCredentials { get; }

  Task<int> SaveChangesAsync(CancellationToken cancellationToken = default); // with or without CancellationToken
}
