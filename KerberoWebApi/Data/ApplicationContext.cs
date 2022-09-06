using KerberoWebApi.Models.Device;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;
using KerberoWebApi.Models.Rent;
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
        
        /// <summary>
        /// Add property to the tables from here
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DeviceVendor>(deviceVendor =>
            {
                    // initialize DeviceVendor with nuki vendor
                    deviceVendor.HasData(new DeviceVendor { Id = 1, Name = "nuki", Logo = " " });
                    // DeviceVendor can have only one name
                    deviceVendor.HasIndex(a => a.Name).IsUnique();
                });
            // An Host can have only one email associated
            modelBuilder.Entity<Host>(host =>
            {
                host.HasIndex(a => a.Email).IsUnique();
            });
                

            modelBuilder.Entity<DeviceVendorAccount>(deviceAccount =>
            {
                // relations 
                deviceAccount
                    .HasOne(a => a.Host)
                    .WithMany(a => a.DeviceVendorAccounts)
                    .OnDelete(DeleteBehavior.ClientCascade);
                deviceAccount
                    .HasOne(a => a.DeviceVendor)
                    .WithMany(a => a.DeviceVendorAccounts);
                // unique properties
                deviceAccount.HasIndex(a => a.ClientId).IsUnique();
            });
            modelBuilder.Entity<DeviceSmartLock>(smartlock =>
            {
                // relations
                smartlock
                    .HasOne(a => a.DeviceVendorAccount)
                    .WithMany(a => a.DeviceSmartLocks);
                // no unique                
            });
            modelBuilder.Entity<DeviceKey>(deviceKey =>
            {
                deviceKey
                    .HasOne(a => a.DeviceSmartLock)
                    .WithMany(a => a.DeviceKeys);
            });

            modelBuilder.Entity<DeviceLog>(deviceLog =>
            {
                deviceLog
                    .HasOne(a => a.DeviceSmartLock)
                    .WithMany(a => a.DeviceLogs);
                deviceLog.HasIndex(a => a.Date).IsUnique();
            });

            modelBuilder.Entity<RentProvider>(rentProvider =>
            {
                rentProvider.HasData(new RentProvider { Id = 1, Logo = "", Name = "airbnb" });
                rentProvider.HasIndex(a => a.Name).IsUnique();
            });
            
            modelBuilder.Entity<RentProviderAccount>(deviceAccount =>
            {
                // relations 
                deviceAccount
                    .HasOne(a => a.Host)
                    .WithMany(a => a.RentProviderAccounts);
                // unique properties
                deviceAccount.HasIndex(a => a.ClientId).IsUnique();
            });
            
            modelBuilder.Entity<Reservation>(reservation =>
            {
                // relations 
                reservation
                    .HasOne(a => a.DeviceSmartLock)
                    .WithMany(a => a.Reservations);
                reservation
                    .HasOne(a => a.RentProviderAccount)
                    .WithMany(a => a.Reservations);
                // unique properties
            });
        }
        
        // Here it is possible to add the table related to the authentication
        public DbSet<Host> HostList { get; set; } = null!;
        public DbSet<DeviceVendorAccount> DeviceVendorAccountList { get; set; } = null!;
        public DbSet<DeviceVendor> DeviceVendorType { get; set; } = null!;
        public DbSet<DeviceSmartLock> DeviceList { get; set; } = null!;
        public DbSet<DeviceLog> DeviceLogList { get; set; } = null!;
        public DbSet<DeviceKey> DeviceKeyList { get; set; } = null!;
        public DbSet<RentProviderAccount> RentProviderAccountList = null!;
        public DbSet<RentProvider> RentProviderList = null!;
        public DbSet<Reservation> ReservationList = null!;

    }
}