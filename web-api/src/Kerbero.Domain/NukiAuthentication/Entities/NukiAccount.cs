using Kerbero.Domain.NukiActions.Entities;

namespace Kerbero.Domain.NukiAuthentication.Entities;
public class NukiAccount
{
	public int Id { get; set; }
	public string? Token { get; set; }
	public string RefreshToken { get; set; } = null!;
	public int TokenExpiringTimeInSeconds { get; set; }
	public DateTime ExpiryDate { get; set; }
	public string? TokenType { get; set; }
	public string ClientId { get; set; } = null!;
	
	public List<NukiSmartLock>? NukiSmartLocks { get; set; }
}
