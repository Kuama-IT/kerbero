using Kerbero.Domain.Common.Entities;
using Kerbero.Domain.NukiActions.Entities;
using Kerbero.Identity.Modules.Users.Entities;

namespace Kerbero.Domain.NukiAuthentication.Entities;
public class NukiAccount: IKerberoIdentityEntity
{
	public int Id { get; set; }
	public string? Token { get; set; }
	public string? RefreshToken { get; set; }
	public int TokenExpiringTimeInSeconds { get; set; }
	public DateTime? ExpiryDate { get; set; }
	public string? TokenType { get; set; }
	public string ClientId { get; set; } = null!;
	
	public List<NukiSmartLock>? NukiSmartLocks { get; set; }
	public Guid UserId { get; set; }
	public User User { get; set; } = null!;
}
