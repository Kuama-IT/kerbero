using Kerbero.Identity.Library.Modules.Claims.Dtos;

namespace Kerbero.Identity.Library.Modules.Claims.Utils;

public static class ClaimUtils
{
  public static bool IsEqual(ClaimReadDto a, ClaimReadDto b)
  {
    return a.Type == b.Type && a.Value == b.Value;
  }
}