namespace Kerbero.Domain.SmartLockKeys.Models;

public class SmartLockKeyModel
{
	public Guid Id { get; set; }
	
	public DateTime CreationDate { get; set; }
	
	public DateTime ExpiryDate { get; set; }

	public string Token { get; set; } = null!;

	public bool IsDisabled { get; set; }
	
	public int UsageCounter { get; set; }
	
	public required string SmartLockId { get; set; }
	
	public int CredentialId { get; set; }
	
	public static SmartLockKeyModel CreateKey(string smartLockId, DateTime expiryDate, int credentialId)
	{
		return new SmartLockKeyModel
		{
			Token = Convert.ToBase64String(Guid.NewGuid().ToByteArray()),
			SmartLockId = smartLockId,
			ExpiryDate = expiryDate,
			CreationDate = DateTime.Now,
			IsDisabled = false,
			UsageCounter = 0,
			CredentialId = credentialId
		};
	}
}
