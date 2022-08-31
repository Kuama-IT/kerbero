using System.ComponentModel.DataAnnotations;

namespace KerberoWebApi.Models
{
    // The class User models the basic entity to log inside the application; 
    public abstract class User
    {
        private int Id { get; } = 0;
        private string Email { get; }
        private string Password { get; }
        public string? Role { get; set; }
        protected User(int id, string email, string psw) { Id = id; Email = email; Password = psw; } 
    }
}