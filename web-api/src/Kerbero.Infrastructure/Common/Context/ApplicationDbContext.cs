using Kerbero.Identity.Common;
using Kerbero.Infrastructure.Common.Interfaces;
using Kerbero.Infrastructure.NukiCredentials.Entities;
using Microsoft.EntityFrameworkCore;

namespace Kerbero.Infrastructure.Common.Context;

public class ApplicationDbContext : KerberoIdentityDbContext, IApplicationDbContext
{
  public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
  {
  }

  public DbSet<NukiCredentialEntity> NukiCredentials => Set<NukiCredentialEntity>();
  public DbSet<UserNukiCredentialEntity> UserNukiCredentials => Set<UserNukiCredentialEntity>();
}
