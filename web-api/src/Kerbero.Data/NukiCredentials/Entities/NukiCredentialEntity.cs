using System.ComponentModel.DataAnnotations.Schema;
using Kerbero.Identity.Modules.Users.Entities;

namespace Kerbero.Data.NukiCredentials.Entities;

/// <summary>
/// Nuki credentials can both be generated with a persistent token and a refreshable token
/// </summary>
public class NukiCredentialEntity
{
  public int Id { get; set; }
  public string? Token { get; set; }

  public bool IsDraft { get; set; }

  public bool IsRefreshable { get; set; }

  public string? RefreshToken { get; set; }

  public int? ExpiresIn { get; set; }

  public DateTime CreatedAt { get; set; }

  public Guid UserId { get; set; }

  [ForeignKey((nameof(UserId)))] public User? User { get; set; }

  // Email used by our user to signup into Nuki web services 
  public string? NukiEmail { get; set; }
}
