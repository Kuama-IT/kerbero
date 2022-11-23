namespace Kerbero.Domain.SmartLockKeys.Dtos;

public record SmartLockKeyDto
{
	public Guid Id { get; set; }
	public DateTime CreationDate { get; set; }
	public DateTime ExpiryDate { get; set; }
	public string Token { get; set; } = null!;
}
