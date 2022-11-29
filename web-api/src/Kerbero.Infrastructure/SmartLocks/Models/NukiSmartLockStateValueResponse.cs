using System.Text.Json.Serialization;

namespace Kerbero.Infrastructure.SmartLocks.Models;

/// <summary>
/// This is a 1 to 1 mapping from what can be seen inside the Nuki official swagger https://api.nuki.io/#/Smartlock/SmartlockResource_get_get
/// Note that they have different states based on the <see cref="NukiSmartLockResponse.Type"/>
/// </summary>
public enum NukiSmartlockState
{
  Uncalibrated = 0,
  Locked = 1,
  Unlocking = 2,
  Unlocked = 3,
  Locking = 4,
  Unlatched = 5,
  UnlockedLockNGo = 6,
  Unlatching = 7,
  MotorBlocked = 254,
  Undefined = 255
}

public enum NukiSmartlockOpenerState
{
  Untrained = 0,
  Online = 1,
  RingToOpenActive = 3,
  Open = 5,
  Opening = 7,
  BootRun = 253,
  Undefined = 255
}