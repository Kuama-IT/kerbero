namespace Kerbero.Infrastructure.SmartLocks.Models;

/// <summary>
/// Type of smartlocks supported by Nuki
/// This is a 1 to 1 mapping from what can be seen inside the Nuki official swagger https://api.nuki.io/#/Smartlock/SmartlockResource_get_get
/// </summary>
public enum NukiSmartlockType
{
  Keyturner = 0,
  Box = 1,
  Opener = 2,
  Smartdoor = 3,
  Smartlock3 = 4,
}