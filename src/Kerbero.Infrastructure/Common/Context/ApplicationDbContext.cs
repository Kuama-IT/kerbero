using System.Reflection;
using Kerbero.Domain.NukiAuthentication.Entities;
using Kerbero.Infrastructure.Common.Interfaces;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Kerbero.Infrastructure.Common.Context;

public class ApplicationDbContext: IdentityDbContext, IApplicationDbContext 
{
	public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options) { }

	public DbSet<NukiAccount> NukiAccounts { get; set; } = null!;
	
	protected override void OnModelCreating(ModelBuilder builder)
	{
		builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

		base.OnModelCreating(builder);
	}
}
