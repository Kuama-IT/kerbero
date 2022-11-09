using Kerbero.Identity.Library.Modules.Users.Models;

namespace Kerbero.Identity.Library.Modules.Users.Dtos;

public class UserCreateDto : UserUpdateDto, IHavePassword
{
  public string Password { get; set; } = null!;
}