using Kerbero.Domain.Common.Models;
using Kerbero.Domain.SmartLocks.Models;
using Kerbero.Infrastructure.SmartLocks.Models;

namespace Kerbero.Infrastructure.SmartLocks.Mappers;

public static class SmartLockMapper
{
  public static List<SmartLockModel> Map(List<NukiSmartLockResponse> responses)
  {
    return responses.ConvertAll(Map);
  }

  public static SmartLockModel Map(NukiSmartLockResponse response)
  {
    return new SmartLockModel
    {
      Id = response.NukiAccountId.ToString(),
      Name = response.Name,
      SmartLockProvider = SmartLockProvider.Nuki,
      State = Map(response.Type, response.State) // TODO I'm not sure whether we should use state or doorState
    };
  }

  public static ESmartLockState Map(ENukiSmartLockType type, NukiSmartlockStateResponse nukiSmartlockState)
  {
    switch (type)
    {
      case ENukiSmartLockType.Keyturner:
      case ENukiSmartLockType.Box:
      case ENukiSmartLockType.Smartlock3:
      case ENukiSmartLockType.Smartdoor:
        if (!Enum.IsDefined(typeof(ENukiSmartLockState), nukiSmartlockState.State))
        {
          throw new ArgumentOutOfRangeException(nameof(nukiSmartlockState),
            $"Trying to map an unknown state for a Nuki Smartlock State type: ${nukiSmartlockState.State}");
        }

        var state = (ENukiSmartLockState)nukiSmartlockState.State;
        switch (state)
        {
          case ENukiSmartLockState.Locked:
            return ESmartLockState.Closed;
          case ENukiSmartLockState.Unlocked:
          case ENukiSmartLockState.UnlockedLockNGo:
            return ESmartLockState.Open;
          case ENukiSmartLockState.Uncalibrated:
          case ENukiSmartLockState.Unlocking:
          case ENukiSmartLockState.Locking:
          case ENukiSmartLockState.Unlatched:
          case ENukiSmartLockState.Unlatching:
            return ESmartLockState.Unmapped;
          case ENukiSmartLockState.MotorBlocked:
            return ESmartLockState.Error;
          case ENukiSmartLockState.Undefined:
            return ESmartLockState.Unknown;
          default:
            throw new ArgumentOutOfRangeException(nameof(state),
              $"Unmapped case for Nuki Smartlock State type: ${nukiSmartlockState.State}");
        }

      case ENukiSmartLockType.Opener:
        if (!Enum.IsDefined(typeof(ENukiSmartLockOpenerState), nukiSmartlockState.State))
        {
          throw new ArgumentOutOfRangeException(nameof(nukiSmartlockState),
            $"Trying to map an unknown state for a Nuki Smartlock Opener type: ${nukiSmartlockState.State}");
        }

        var openerState = (ENukiSmartLockOpenerState)nukiSmartlockState.State;

        switch (openerState)
        {
          case ENukiSmartLockOpenerState.BootRun:
          case ENukiSmartLockOpenerState.Opening:
          case ENukiSmartLockOpenerState.RingToOpenActive:
          case ENukiSmartLockOpenerState.Untrained:
            return ESmartLockState.Unmapped;
          case ENukiSmartLockOpenerState.Open:
            return ESmartLockState.Open;
          case ENukiSmartLockOpenerState.Online:
            return ESmartLockState.Closed; // TODO is this correct?
          case ENukiSmartLockOpenerState.Undefined:
            return ESmartLockState.Unknown;
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
