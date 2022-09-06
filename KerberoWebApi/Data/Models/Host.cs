using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using KerberoWebApi.Models.Device;
using KerberoWebApi.Models.Rent;

namespace KerberoWebApi.Models
{
    public class Host
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }  = null!;
        public string Surname { get; set; } = null!;
        public string? Icon { get; set; }
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;

        public List<DeviceVendorAccount>? DeviceVendorAccounts { get; set; }
        public List<RentProviderAccount>? RentProviderAccounts { get; set; }
    }
}