using Kerbero.Domain.NukiActions.Entities;

namespace Kerbero.Domain.SmartLockKeys.Entities;

public class SmartLockKey
{
	public Guid Id { get; set; }
	
	public DateTime CreationDate { get; set; }
	
	public DateTime ExpiryDate { get; set; }

	public string Token { get; set; } = null!;

	public bool IsDisabled { get; set; }
	
	public int UsageCounter { get; set; }
	
	public int NukiSmartLockId { get; set; }
	
	public NukiSmartLockEntity NukiSmartLockEntity { get; set; } = null!;
}
