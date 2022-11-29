namespace Kerbero.Domain.SmartLocks.Models;

/// <summary>
/// Providers of smartlocks supported from Kerbero platform
/// </summary>
public class SmartlockProvider
{
  public static readonly SmartlockProvider Nuki = new SmartlockProvider("nuki");
  
  public readonly string Name;

  private SmartlockProvider(string name)
  {
    this.Name = name;
  }
}