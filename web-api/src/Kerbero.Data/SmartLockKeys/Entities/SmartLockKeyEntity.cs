namespace Kerbero.Data.SmartLockKeys.Entities;

public class SmartLockKeyEntity
{
	public Guid Id { get; set; }
	
	/// <summary>
	/// Internal information (when the record was created)
	/// </summary>
	public required DateTime CreatedAt { get; set; }

	/// <summary>
	/// From which date this key can be used
	/// </summary>
	public required DateTime ValidFrom { get; set; }

	/// <summary>
	/// Last date this key can be used
	/// </summary>
	public required DateTime ValidUntil { get; set; }

	public required string Password { get; set; }

	public bool IsDisabled { get; set; }
	
	public int UsageCounter { get; set; }
	
	public required string SmartLockId { get; set; }

	public required int CredentialId { get; set; }
	
	public required string SmartLockProvider { get; set; }
}
