using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using KerberoWebApi.Models.Device;

namespace KerberoWebApi.Models
{
    public class Host
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }  = null!;
        public string Surname { get; set; } = null!;
        public string? Icon { get; set; }
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}