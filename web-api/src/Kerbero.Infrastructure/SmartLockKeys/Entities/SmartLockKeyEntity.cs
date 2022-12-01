namespace Kerbero.Infrastructure.SmartLockKeys.Entities;

public class SmartLockKeyEntity
{
	public Guid Id { get; set; }
	
	public required DateTime CreationDate { get; set; }
	
	public required DateTime ExpiryDate { get; set; }

	public required string Password { get; set; }

	public bool IsDisabled { get; set; }
	
	public int UsageCounter { get; set; }
	
	public required string SmartLockId { get; set; }

	public required int CredentialId { get; set; }
}
