namespace Kerbero.Identity.Library.Modules.Roles.Dtos;

public class RoleReadDto
{
  public required Guid Id { get; set; }
  public required string Name { get; set; } = null!;
}