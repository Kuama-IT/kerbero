namespace Kerbero.Domain.NukiAuthentication.Dtos;

public class CreateNukiCredentialParams
{
  public string ClientId { get; init; } = null!;

  public string Code { get; init; } = null!;
}
