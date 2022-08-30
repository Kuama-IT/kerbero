using System.ComponentModel.DataAnnotations;

namespace KerberoWebApi.Models
{
    public class Host: User
    {
        public string? Name { get; set;}
        public string? Surname { get; set;}
        public string? Icon { get; set;}
        public List<DeviceVendorAccount> VendorAccounts { get;}
        public Host(int id, string email, string psw): base(id, email, psw) { VendorAccounts = new List<DeviceVendorAccount>(); }
    }
}