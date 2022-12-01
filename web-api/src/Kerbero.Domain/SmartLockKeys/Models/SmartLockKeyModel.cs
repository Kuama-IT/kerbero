using CrypticWizard.RandomWordGenerator;

namespace Kerbero.Domain.SmartLockKeys.Models;

public class SmartLockKeyModel
{
	public Guid Id { get; set; }
	
	public DateTime CreationDate { get; set; }
	
	public DateTime ExpiryDate { get; set; }

	public string Password { get; set; } = null!;

	public bool IsDisabled { get; set; }
	
	public int UsageCounter { get; set; }
	
	public required string SmartLockId { get; set; }
	
	public int CredentialId { get; set; }
	
	public static SmartLockKeyModel CreateKey(string smartLockId, DateTime expiryDate, int credentialId)
	{
		var wordGenerator = new WordGenerator();
		return new SmartLockKeyModel
		{
			Password = wordGenerator.GetWord(WordGenerator.PartOfSpeech.noun),
			SmartLockId = smartLockId,
			ExpiryDate = expiryDate,
			CreationDate = DateTime.Now,
			IsDisabled = false,
			UsageCounter = 0,
			CredentialId = credentialId
		};
	}
}
