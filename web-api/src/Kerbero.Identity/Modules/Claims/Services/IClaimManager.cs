using System.Security.Claims;

namespace Kerbero.Identity.Modules.Claims.Services;

public interface IClaimManager
{
  List<Claim> GetAll();
  void AddRange(IEnumerable<Claim> claims);
  bool Exists(string type, string value);
}