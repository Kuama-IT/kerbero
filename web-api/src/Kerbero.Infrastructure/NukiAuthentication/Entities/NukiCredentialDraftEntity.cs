using System.ComponentModel.DataAnnotations.Schema;
using Kerbero.Identity.Modules.Users.Entities;

namespace Kerbero.Infrastructure.NukiAuthentication.Entities;

public class NukiCredentialDraftEntity
{
  public int Id { get; set; }
  public string ClientId { get; set; } = null!;
  public string RedirectUrl { get; set; } = null!;
  public Guid UserId { get; set; }

  [ForeignKey(nameof(UserId))]
  public User? User { get; set; }
}