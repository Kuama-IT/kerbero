namespace Kerbero.Domain.NukiActions.Models.PresentationResponse;

public class KerberoSmartLockPresentationResponse
{
	public string? ExternalName { get; set; }
	
	public int ExternalType { get; set; }

	public int AccountId { get; set; }

	public int ExternalSmartLockId { get; set; }
	
	public int SmartLockId { get; set; }
}
