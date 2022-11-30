using Kerbero.Identity.Common;
using Kerbero.Infrastructure.Common.Interfaces;
using Kerbero.Infrastructure.NukiCredentials.Entities;
using Kerbero.Infrastructure.SmartLockKeys.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Kerbero.Infrastructure.Common.Context;

public class ApplicationDbContext : KerberoIdentityDbContext, IApplicationDbContext
{
  public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
  {
  }

  public DbSet<NukiCredentialEntity> NukiCredentials => Set<NukiCredentialEntity>();
  public DbSet<UserNukiCredentialEntity> UserNukiCredentials => Set<UserNukiCredentialEntity>();
  public DbSet<SmartLockKeyEntity> SmartLockKeys => Set<SmartLockKeyEntity>();


  protected sealed override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
  {
    configurationBuilder.Properties<DateTime>().HaveConversion(typeof(DateTimeToDateTimeUtc));
  }

  public class DateTimeToDateTimeUtc : ValueConverter<DateTime, DateTime>
  {
    public DateTimeToDateTimeUtc() : base(c => DateTime.SpecifyKind(c, DateTimeKind.Utc), c => c)
    {
    }
  }
}