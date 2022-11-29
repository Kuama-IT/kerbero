namespace Kerbero.Domain.NukiAuthentication.Dtos;

public class CreateNukiCredentialParams
{
  public Guid UserId { get; init; }
  public string Token { get; init; } = null!;
}
