using Kerbero.Domain.Common.Models;

namespace Kerbero.Domain.SmartLocks.Models;

public class SmartLockModel
{
  public required string Id { get; set; }
  public required string Name { get; set; }

  /// <summary>
  /// Which external service has returned this Smartlock, useful to group the Smartlocks inside the UI or discern which
  /// external service should be invoked upon further interactions with this smartlock
  /// </summary>
  public required SmartLockProvider SmartLockProvider { get; set; }

  /// <summary>
  /// Current state of this Smartlock, this should be a driver information for any further interactions
  /// (Eg. you cannot open a smartlock if it is already opened)
  /// </summary>
  public required ESmartLockState State { get; set; }
}
