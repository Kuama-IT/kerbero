namespace Kerbero.Domain.NukiActions.Models;

public class NukiSmartLocksListExternalResponseDto
{
	public List<NukiSmartLockExternalResponseDto> NukiSmartLockList { get; } = new();
}
