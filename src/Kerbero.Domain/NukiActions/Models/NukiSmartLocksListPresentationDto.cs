using Kerbero.Domain.Common.Models;

namespace Kerbero.Domain.NukiActions.Models;

public class NukiSmartLocksListPresentationDto
{
	public List<KerberoSmartLockPresentationDto<NukiSmartLockState>> NukiSmartLocksList { get; } = new();
}
