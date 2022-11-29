using System.ComponentModel.DataAnnotations.Schema;

namespace Kerbero.Infrastructure.NukiAuthentication.Entities;

public class NukiCredentialEntity
{
  public int Id { get; set; }
  public string Token { get; set; } = null!;
  public Guid UserId { get; set; }
  [ForeignKey((nameof(UserId)))]
  public string? User { get; set; }
}

