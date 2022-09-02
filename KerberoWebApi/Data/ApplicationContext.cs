using KerberoWebApi.Models.Device;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace KerberoWebApi.Models
{
    // every context connection should be open one at the time. If more than a connection,
    // it must be active concurrently should be created another context.
    public class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        { }

        // from On... it is possible to add property to the tables

        // Here it is possible to add the table related to the authentication
        public DbSet<Host> HostList { get; set; } = null!;
        public DbSet<DeviceVendorAccount> DeviceVendorAccountList { get; set; } = null!;
        public DbSet<DeviceVendor> DeviceVendorType { get; set; } = null!;
    }
}