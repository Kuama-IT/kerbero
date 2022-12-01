namespace Kerbero.Domain.Common.Models;

/// <summary>
/// Providers of smartlocks supported from Kerbero platform
/// </summary>
public class SmartLockProvider
{
  public static readonly SmartLockProvider Nuki = new SmartLockProvider("nuki");

  public readonly string Name;

  private SmartLockProvider(string name)
  {
    Name = name;
  }

  public static SmartLockProvider? TryParse(string rawProviderName)
  {
    if (rawProviderName == Nuki.Name)
    {
      return Nuki;
    }

    return null;
  }
}
