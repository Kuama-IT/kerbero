namespace Kerbero.Common.Entities;

public class NukiAccountEntity
{
	public string Token { get; set; } = null!;
	public string RefreshToken { get; set; } = null!;
	public string ClientId { get; set; } = null!;
	public int TokenExpiresIn { get; set; }
	public string TokenType { get; set; } = null!;
}