using Kerbero.Domain.NukiActions.Entities;
using Kerbero.Identity.Common;
using Kerbero.Infrastructure.Common.Interfaces;
using Kerbero.Infrastructure.NukiAuthentication.Entities;
using Microsoft.EntityFrameworkCore;

namespace Kerbero.Infrastructure.Common.Context;

public class ApplicationDbContext : KerberoIdentityDbContext, IApplicationDbContext
{
  public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
  {
  }

  public DbSet<NukiCredentialEntity> NukiCredentials => Set<NukiCredentialEntity>();
  public DbSet<NukiSmartLockEntity> NukiSmartLocks => Set<NukiSmartLockEntity>();
  public DbSet<UserNukiCredentialEntity> UserNukiCredentials => Set<UserNukiCredentialEntity>();
  public DbSet<NukiSmartLockStateEntity> NukiSmartLockStates => Set<NukiSmartLockStateEntity>();
  
}
