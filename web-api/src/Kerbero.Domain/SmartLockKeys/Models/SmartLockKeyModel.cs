using CrypticWizard.RandomWordGenerator;
using Kerbero.Domain.Common.Models;

namespace Kerbero.Domain.SmartLockKeys.Models;

public class SmartLockKeyModel
{
	public Guid Id { get; set; }
	
	public DateTime CreationDate { get; set; }
	
	public DateTime ExpiryDate { get; set; }

	public required string Password { get; set; }

	public bool IsDisabled { get; set; }
	
	public int UsageCounter { get; set; }
	
	public required string SmartLockId { get; set; }
	
	public int CredentialId { get; set; }
	public required string SmartLockProvider { get; set; }
	
	public static SmartLockKeyModel CreateKey(string smartLockId, DateTime expiryDate, int credentialId, SmartLockProvider smartLockProvider)
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
			CredentialId = credentialId,
			SmartLockProvider = smartLockProvider.Name
		};
	}
}
