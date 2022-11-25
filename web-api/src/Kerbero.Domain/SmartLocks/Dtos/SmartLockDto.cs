namespace Kerbero.Domain.SmartLocks.Dtos;

public class SmartLockDto
{
  public string Id { get; set; }
  public string Name { get; set; } = null!;

  public string Provider { get; set; } = null!;

  public int CredentialId { get; set; }
}