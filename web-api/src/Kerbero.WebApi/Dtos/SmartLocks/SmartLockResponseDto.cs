namespace Kerbero.WebApi.Dtos.SmartLocks;

public class SmartLockResponseDto
{
  public required string Id { get; set; }
  
  public required string Name { get; set; }

  public required string SmartLockProvider { get; set; }

  public int CredentialId { get; set; }

  public required SmartLockStateDto State { get; set; }
}