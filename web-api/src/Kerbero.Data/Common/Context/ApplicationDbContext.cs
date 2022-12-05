using Kerbero.Identity.Common;
using Kerbero.Data.Common.Interfaces;
using Kerbero.Data.NukiCredentials.Entities;
using Kerbero.Data.SmartLockKeys.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Kerbero.Data.Common.Context;

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