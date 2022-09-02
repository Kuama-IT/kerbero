using KerberoWebApi.Models.Device;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace KerberoWebApi.Models
{
    // every context connection should be open one at the time. If more than a connection,
    // it must be active concurrently should be created another context.
    public class VendorAuthenticationContext : DbContext
    {
        public VendorAuthenticationContext(DbContextOptions<VendorAuthenticationContext> options)
            : base(options)
        { }

        // Here it is possible to add the table related to the authentication
        public DbSet<DeviceVendorAccount> DeviceVendorAccountList { get; set; } = null!;
    }
}