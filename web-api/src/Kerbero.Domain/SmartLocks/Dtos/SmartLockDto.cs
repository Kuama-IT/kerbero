namespace Kerbero.Domain.SmartLocks.Dtos;

public class SmartLockDto
{
  public required string Id { get; set; }
  public required string Name { get; set; }

  public required string SmartLockProvider { get; set; }

  public int CredentialId { get; set; }

  public required SmartLockStateDto State { get; set; }
}