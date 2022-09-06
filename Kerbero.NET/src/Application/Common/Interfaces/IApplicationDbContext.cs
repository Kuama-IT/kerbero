using Kerbero.NET.Domain.Entities;
using Kerbero.NET.Domain.Entities.Account;
using Kerbero.NET.Domain.Entities.Device;
using Kerbero.NET.Domain.Entities.Providers;
using Kerbero.NET.Domain.Entities.RentInformation;
using Microsoft.EntityFrameworkCore;

namespace Kerbero.NET.Application.Common.Interfaces;

public interface IApplicationDbContext
{
        public DbSet<HostAccount> HostList { get; }
        
        public DbSet<SmartLockProviderAccount> DeviceVendorAccountList { get; } 
        
        public DbSet<SmartLockProvider> DeviceVendorType { get; } 
        
        public DbSet<SmartLock> DeviceList { get; } 
        
        public DbSet<SmartLockLog> DeviceLogList { get; } 
        
        public DbSet<DeviceKey> DeviceKeyList { get; } 
        
        public DbSet<RentProviderAccount> RentProviderAccountList { get; } 
        
        public DbSet<RentProvider> RentProviderList { get; } 
        
        public DbSet<Reservation> ReservationList { get; } 

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
