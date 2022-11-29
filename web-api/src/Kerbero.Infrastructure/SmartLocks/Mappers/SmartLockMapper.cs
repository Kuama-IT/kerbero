using Kerbero.Domain.SmartLocks.Models;
using Kerbero.Infrastructure.SmartLocks.Models;

namespace Kerbero.Infrastructure.SmartLocks.Mappers;

public static class SmartLockMapper
{
  public static List<SmartLock> Map(List<NukiSmartLockResponse> responses)
  {
    return responses.ConvertAll(Map);
  }

  public static SmartLock Map(NukiSmartLockResponse response)
  {
    return new SmartLock
    {
      Id = response.NukiAccountId.ToString(),
      Name = response.Name,
      Provider = SmartlockProvider.Nuki,
      State = Map(response.Type, response.State) // TODO I'm not sure whether we should use state or doorState
    };
  }

  public static SmartLockState Map(NukiSmartlockType type, NukiSmartlockStateResponse nukiSmartlockState)
  {
    switch (type)
    {
      case NukiSmartlockType.Keyturner:
      case NukiSmartlockType.Box:
      case NukiSmartlockType.Smartlock3:
      case NukiSmartlockType.Smartdoor:
        if (!Enum.IsDefined(typeof(NukiSmartlockState), nukiSmartlockState.State))
        {
          throw new ArgumentOutOfRangeException(nameof(nukiSmartlockState),
            $"Trying to map an unknown state for a Nuki Smartlock State type: ${nukiSmartlockState.State}");
        }

        var state = (NukiSmartlockState)nukiSmartlockState.State;
        switch (state)
        {
          case NukiSmartlockState.Locked:
            return SmartLockState.Closed;
          case NukiSmartlockState.Unlocked:
          case NukiSmartlockState.UnlockedLockNGo:
            return SmartLockState.Open;
          case NukiSmartlockState.Uncalibrated:
          case NukiSmartlockState.Unlocking:
          case NukiSmartlockState.Locking:
          case NukiSmartlockState.Unlatched:
          case NukiSmartlockState.Unlatching:
            return SmartLockState.Unmapped;
          case NukiSmartlockState.MotorBlocked:
            return SmartLockState.Error;
          case NukiSmartlockState.Undefined:
            return SmartLockState.Unknown;
          default:
            throw new ArgumentOutOfRangeException(nameof(state),
              $"Unmapped case for Nuki Smartlock State type: ${nukiSmartlockState.State}");
        }

      case NukiSmartlockType.Opener:
        if (!Enum.IsDefined(typeof(NukiSmartlockOpenerState), nukiSmartlockState.State))
        {
          throw new ArgumentOutOfRangeException(nameof(nukiSmartlockState),
            $"Trying to map an unknown state for a Nuki Smartlock Opener type: ${nukiSmartlockState.State}");
        }

        var openerState = (NukiSmartlockOpenerState)nukiSmartlockState.State;

        switch (openerState)
        {
          case NukiSmartlockOpenerState.BootRun:
          case NukiSmartlockOpenerState.Opening:
          case NukiSmartlockOpenerState.RingToOpenActive:
          case NukiSmartlockOpenerState.Untrained:
            return SmartLockState.Unmapped;
          case NukiSmartlockOpenerState.Open:
            return SmartLockState.Open;
          case NukiSmartlockOpenerState.Online:
            return SmartLockState.Closed; // TODO is this correct?
          case NukiSmartlockOpenerState.Undefined:
            return SmartLockState.Unknown;
          default:
            throw new ArgumentOutOfRangeException(nameof(nukiSmartlockState),
              $"Unmapped case for Nuki Smartlock Opener type: ${nukiSmartlockState.State}");
        }
      default:
        throw new ArgumentOutOfRangeException(nameof(type),
          $"Unmapped case for Nuki Smartlock type: ${type}");
    }
  }
}