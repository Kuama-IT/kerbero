using Kerbero.Common.Entities;
using Kerbero.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Kerbero.Infrastructure.Context;

public class ApplicationDbContext: DbContext, IApplicationDbContext 
{
	public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options) { }

	public DbSet<NukiAccount> NukiAccounts { get; set; } = null!;
}
