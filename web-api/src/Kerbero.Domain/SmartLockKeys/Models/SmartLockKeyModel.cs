using CrypticWizard.RandomWordGenerator;
using Kerbero.Domain.Common.Models;

namespace Kerbero.Domain.SmartLockKeys.Models;

public class SmartLockKeyModel
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

  public int CredentialId { get; set; }

  public required string SmartLockProvider { get; set; }

  public static SmartLockKeyModel CreateKey(string smartLockId, DateTime validUntilDate, DateTime validFromDate,
    int credentialId, SmartLockProvider smartLockProvider)
  {
    var wordGenerator = new WordGenerator();
    return new SmartLockKeyModel
    {
      Password = wordGenerator.GetWord(WordGenerator.PartOfSpeech.noun),
      SmartLockId = smartLockId,
      ValidFrom = validFromDate,
      ValidUntil = validUntilDate,
      CreatedAt = DateTime.Now,
      IsDisabled = false,
      UsageCounter = 0,
      CredentialId = credentialId,
      SmartLockProvider = smartLockProvider.Name
    };
  }
}