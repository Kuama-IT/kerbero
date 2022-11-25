using System.Reflection;
using Kerbero.Domain.NukiActions.Entities;
using Kerbero.Domain.NukiAuthentication.Models;
using Kerbero.Identity.Common;
using Kerbero.Infrastructure.Common.Interfaces;
using Kerbero.Infrastructure.NukiAuthentication.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace Kerbero.Infrastructure.Common.Context;

public class ApplicationDbContext : KerberoIdentityDbContext, IApplicationDbContext
{
  public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
  {
  }

  public DbSet<NukiCredentialEntity> NukiCredentials => Set<NukiCredentialEntity>();
  public DbSet<NukiCredentialDraftEntity> NukiCredentialDrafts => Set<NukiCredentialDraftEntity>();
  public DbSet<NukiSmartLockEntity> NukiSmartLocks => Set<NukiSmartLockEntity>();
  
  public DbSet<NukiSmartLockStateEntity> NukiSmartLockStates => Set<NukiSmartLockStateEntity>();

  public DbSet<UserNukiCredentialEntity> UserNukiCredentials => Set<UserNukiCredentialEntity>();
}