namespace Kerbero.Domain.NukiActions.Models.ExternalResponses;

public class NukiSmartLockExternalResponse
{
	public int SmartLockId { get; set; }
	public int AccountId { get; set; }
	public int Type { get; set; }
	public int LmType { get; set; }
	public int AuthId { get; set; }
	public string Name { get; set; } = null!;
	public bool Favourite { get; set; }
	public NukiSmartLockStateExternalResponse? State { get; set; }
}
