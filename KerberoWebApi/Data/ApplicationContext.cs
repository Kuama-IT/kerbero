using KerberoWebApi.Models.Device;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace KerberoWebApi.Models
{
    // every context connection should be open one at the time. If more than a connection,
    // it must be active concurrently should be created another context.
    public class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<DeviceVendor>()
                .HasData(new DeviceVendor { Id = 1, Name = "nuki", Logo = " " });
        }
        
        // from On... it is possible to add property to the tables

        // Here it is possible to add the table related to the authentication
        public DbSet<Host> HostList { get; set; } = null!;
        public DbSet<DeviceVendorAccount> DeviceVendorAccountList { get; set; } = null!;
        public DbSet<DeviceVendor> DeviceVendorType { get; set; } = null!;
        public DbSet<DeviceSmartLock> DeviceList { get; set; } = null!;
        public DbSet<DeviceLog> DeviceLogList { get; set; } = null!;
        public DbSet<DeviceKey> DeviceKeyList { get; set; } = null!;
    }
}