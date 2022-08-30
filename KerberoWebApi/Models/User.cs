using System.ComponentModel.DataAnnotations;

namespace KerberoWebApi.Models
{
    // The class User models the basic entity to log inside the application; 
    public abstract class User
    {
        public int Id { get; set; } = 0;
        public string Email { get; set; }
        public string Password { get; set; }
        public string? Role { get; set; }
        public List<KerberoWebApi.Models.Permissions> Permissions { get; set; }
        protected User(int id, string email, string psw) { Id = id; Email = email; Password = psw; Permissions = new List<Permissions>(); } 
    }
}