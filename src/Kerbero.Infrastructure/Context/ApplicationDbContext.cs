using Kerbero.Common.Entities;
using Kerbero.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Kerbero.Infrastructure.Context;

public class ApplicationDbContext: DbContext, IApplicationDbContext 
{
	// TODO: implement a db context
	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		=> optionsBuilder.UseNpgsql("TODO");

	public DbSet<NukiAccount> NukiAccounts { get; set; }
}
