namespace Kerbero.Domain.SmartLockKeys.Models.PresentationResponses;

public record CreateSmartLockKeyPresentationResponse
{
	public Guid Id { get; set; }
	public DateTime CreationDate { get; set; }
	public DateTime ExpiryDate { get; set; }
	public string Token { get; set; } = null!;
}
