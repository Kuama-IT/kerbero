using System.Reflection;
using Kerbero.Domain.NukiActions.Entities;
using Kerbero.Domain.NukiAuthentication.Entities;
using Kerbero.Identity.Common;
using Kerbero.Infrastructure.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Kerbero.Infrastructure.Common.Context;

public class ApplicationDbContext: KerberoIdentityDbContext, IApplicationDbContext 
{
	public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options) { }

	public DbSet<NukiAccount> NukiAccounts => Set<NukiAccount>();
	
	public DbSet<NukiSmartLock> NukiSmartLocks => Set<NukiSmartLock>();
	
	public DbSet<NukiSmartLockState> NukiSmartLockStates => Set<NukiSmartLockState>();

	protected override void OnModelCreating(ModelBuilder builder)
	{
		builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

		base.OnModelCreating(builder);
	}
}
