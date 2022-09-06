using System.Reflection;
using Kerbero.NET.Application.Common.Interfaces;
using Kerbero.NET.Domain.Entities.Account;
using Kerbero.NET.Domain.Entities.Device;
using Kerbero.NET.Domain.Entities.Providers;
using Kerbero.NET.Domain.Entities.RentInformation;
using MediatR;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Kerbero.NET.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options)
        : base(options)
    { }

    /// <summary>
    /// Add property to the tables from here
    /// </summary>
    /// <param name="builder"></param>
    protected override void OnModelCreating(ModelBuilder builder)
    {
            builder.Entity<SmartLockProvider>(deviceVendor =>
            {
                    // initialize DeviceVendor with nuki vendor
                    deviceVendor.HasData(new SmartLockProvider() { Id = 1, Name = "nuki", Logo = " " });
                    // DeviceVendor can have only one name
                    deviceVendor.HasIndex(a => a.Name).IsUnique();
                });
            // An Host can have only one email associated
            builder.Entity<HostAccount>(host =>
            {
                host.HasIndex(a => a.Email).IsUnique();
            });
                

            builder.Entity<SmartLockProviderAccount>(deviceAccount =>
            {
                // relations 
                deviceAccount
                    .HasOne(a => a.HostAccount)
                    .WithMany(a => a.SmartLockProviderAccounts)
                    .OnDelete(DeleteBehavior.ClientCascade);
                deviceAccount
                    .HasOne(a => a.SmartLockProvider)
                    .WithMany(a => a.DeviceVendorAccounts);
                // unique properties
                deviceAccount.HasIndex(a => a.ClientId).IsUnique();
            });
            builder.Entity<SmartLock>(smartLock =>
            {
                // relations
                smartLock
                    .HasOne(a => a.SmartLockProviderAccount)
                    .WithMany(a => a.DeviceSmartLocks);
                // no unique                
            });
            builder.Entity<DeviceKey>(deviceKey =>
            {
                deviceKey
                    .HasOne(a => a.SmartLock)
                    .WithMany(a => a.DeviceKeys);
            });

            builder.Entity<SmartLockLog>(deviceLog =>
            {
                deviceLog
                    .HasOne(a => a.SmartLock)
                    .WithMany(a => a.DeviceLogs);
                deviceLog.HasIndex(a => a.Date).IsUnique();
            });

            builder.Entity<RentProvider>(rentProvider =>
            {
                rentProvider.HasData(new RentProvider { Id = 1, Logo = "", Name = "airbnb" });
                rentProvider.HasIndex(a => a.Name).IsUnique();
            });
            
            builder.Entity<RentProviderAccount>(deviceAccount =>
            {
                // relations 
                deviceAccount
                    .HasOne(a => a.HostAccount)
                    .WithMany(a => a.RentProviderAccounts);
                // unique properties
                deviceAccount.HasIndex(a => a.ClientId).IsUnique();
            });
            
            builder.Entity<Reservation>(reservation =>
            {
                // relations 
                reservation
                    .HasOne(a => a.SmartLock)
                    .WithMany(a => a.Reservations);
                reservation
                    .HasOne(a => a.RentProviderAccount)
                    .WithMany(a => a.Reservations);
                // unique properties
            });
        base.OnModelCreating(builder);
    }

    public DbSet<HostAccount> HostList => Set<HostAccount>();
    public DbSet<SmartLockProviderAccount> DeviceVendorAccountList => Set<SmartLockProviderAccount>();
    public DbSet<SmartLockProvider> DeviceVendorType  => Set<SmartLockProvider>();
    public DbSet<SmartLock> DeviceList  => Set<SmartLock>();
    public DbSet<SmartLockLog> DeviceLogList  => Set<SmartLockLog>();
    public DbSet<DeviceKey> DeviceKeyList  => Set<DeviceKey>();
    public DbSet<RentProviderAccount> RentProviderAccountList  => Set<RentProviderAccount>();
    public DbSet<RentProvider> RentProviderList  => Set<RentProvider>();
    public DbSet<Reservation> ReservationList  => Set<Reservation>();

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await base.SaveChangesAsync(cancellationToken);
    }
}
