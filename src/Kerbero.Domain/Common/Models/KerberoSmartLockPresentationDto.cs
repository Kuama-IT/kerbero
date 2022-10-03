namespace Kerbero.Domain.Common.Models;

public class KerberoSmartLockPresentationDto<TState>
{
	public TState? ExternalState { get; set; }
	public string? ExternalName { get; set; }
	public int ExternalType { get; set; }
	public int ExternalAccountId { get; set; }
	public int ExternalSmartLockId { get; set; }
}
