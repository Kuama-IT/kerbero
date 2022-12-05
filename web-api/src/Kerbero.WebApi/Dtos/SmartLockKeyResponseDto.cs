namespace Kerbero.WebApi.Dtos;

public record SmartLockKeyResponseDto
{
  public Guid Id { get; set; }

  public DateTime ValidFromDate { get; set; }
  public DateTime ValidUntilDate { get; set; }
  public required string Password { get; set; }
}