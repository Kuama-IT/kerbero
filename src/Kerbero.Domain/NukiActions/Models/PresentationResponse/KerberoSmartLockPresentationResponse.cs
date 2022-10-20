namespace Kerbero.Domain.NukiActions.Models.PresentationResponse;

public class KerberoSmartLockPresentationResponse
{
	public string? ExternalName { get; set; }
	public int ExternalType { get; set; }
	public int ExternalAccountId { get; set; }
	public int ExternalSmartLockId { get; set; }
}
