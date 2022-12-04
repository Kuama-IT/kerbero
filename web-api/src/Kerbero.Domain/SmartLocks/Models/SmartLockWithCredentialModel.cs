namespace Kerbero.Domain.SmartLocks.Models;

public class SmartLockWithCredentialModel : SmartLockModel
{
  /// <summary>
  /// External service credential used to retrieve this smart lock
  /// </summary>
  public int CredentialId { get; set; }
}