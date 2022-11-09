using System.Security.Claims;
using Kerbero.Identity.Modules.Claims.Utils;

namespace Kerbero.Identity.Modules.Claims.Services;

public class ClaimManager : IClaimManager
{
  private readonly List<Claim> _claims = new();

  public List<Claim> GetAll()
  {
    return _claims.Concat(ClaimsDefinition.KuIdentityClaims).ToList();
  }

  public void AddRange(IEnumerable<Claim> claims)
  {
    _claims.AddRange(claims);
  }

  public bool Exists(string type, string value)
  {
    return _claims.Any(e => e.Type == type && e.Value == value);
  }

  public static ClaimManager Create(IEnumerable<Claim>? claims = null)
  {
    var claimManager = new ClaimManager();
    if (claims is not null)
    {
      claimManager.AddRange(claims);
    }

    return claimManager;
  }
}