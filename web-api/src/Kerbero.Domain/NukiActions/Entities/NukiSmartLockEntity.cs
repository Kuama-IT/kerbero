using Kerbero.Domain.NukiAuthentication.Models;

namespace Kerbero.Domain.NukiActions.Entities;

// TODO FIX ME
public class NukiSmartLockEntity
{
  public int Id { get; set; }

  public int ExternalSmartLockId { get; set; }

  public int Type { get; set; }

  public int AuthId { get; set; }

  public string? Name { get; set; }

  public bool Favourite { get; set; }

  // TODO FIX ME
  public int NukiAccountId { get; set; }

  public NukiSmartLockStateEntity? State { get; set; }
}