namespace Kerbero.Identity.Library.Modules.Users.Dtos;

public class UserReadDto
{
  public Guid Id { get; set; }

  public string UserName { get; set; } = null!;
  public string Email { get; set; } = null!;

  public bool EmailConfirmed { get; set; }
}