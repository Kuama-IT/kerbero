namespace Kerbero.Domain.Common.Models;

public class KerberoSmartLockPresentationDto
{
	public string? ExternalName { get; set; }
	public int ExternalType { get; set; }
	public int ExternalAccountId { get; set; }
	public int ExternalSmartLockId { get; set; }
}
