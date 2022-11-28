using System.ComponentModel.DataAnnotations.Schema;
using Kerbero.Domain.NukiAuthentication.Models;
using Kerbero.Identity.Modules.Users.Entities;
using Microsoft.EntityFrameworkCore;

namespace Kerbero.Infrastructure.NukiAuthentication.Entities;

[Index(nameof(UserId), nameof(NukiCredentialId), IsUnique = true)]
public class UserNukiCredentialEntity
{
  public int Id { get; set; }
  
  public Guid UserId { get; set; }

  [ForeignKey(nameof(UserId))]
  public User? User { get; set; }

  public int NukiCredentialId { get; set; }

  [ForeignKey(nameof(NukiCredentialId))]
  public NukiCredentialEntity? NukiCredential { get; set; }
}