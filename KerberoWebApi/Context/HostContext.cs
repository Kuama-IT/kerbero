using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace KerberoWebApi.Models
{
    public class HostContext : DbContext
    {
        public HostContext(DbContextOptions<HostContext> options)
            : base(options)
        {
        }

        public DbSet<Host> HostLists { get; set; } = null!;
    }
}