namespace Kerbero.WebApi.Dtos;

public record SmartLockKeyResponseDto
{
	public Guid Id { get; set; }
	public DateTime CreationDate { get; set; }
	public DateTime ExpiryDate { get; set; }
	public required string Password { get; set; }
}
