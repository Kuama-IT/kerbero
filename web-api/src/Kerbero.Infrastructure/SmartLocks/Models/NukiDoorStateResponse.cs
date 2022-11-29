namespace Kerbero.Infrastructure.SmartLocks.Models;

/// <summary>
/// This is a 1 to 1 mapping from what can be seen inside the Nuki official swagger https://api.nuki.io/#/Smartlock/SmartlockResource_get_get
/// </summary>
public enum NukiDoorStateResponse
{
  UnavailableOrNotPaired = 0,
  Deactivated = 1,
  DoorClosed = 2,
  DoorOpened = 3,
  DoorStateUnknown = 4,
  Calibrating = 5,
  Uncalibrated = 16,
  Removed = 240,
  Unknown = 255,
}