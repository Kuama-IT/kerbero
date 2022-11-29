namespace Kerbero.Domain.SmartLocks.Models;

public enum SmartLockState
{
  /// <summary>
  /// The smart lock is currently closed, a.k.a. it can be opened
  /// </summary>
  Closed,

  /// <summary>
  /// The smart lock is currently opened, a.k.a. it can be closed
  /// </summary>
  Open,

  /// <summary>
  /// The external provider did not return a meaningful state for this smartlock
  /// </summary>
  Unknown,

  /// <summary>
  /// The external provider returned an error state for this smartlock
  /// </summary>
  Error,

  /// <summary>
  /// Kerbero did not map the external provider status
  /// </summary>
  Unmapped,
  // Other interesting states to add some granularity can be "Unlocking", "Opening", etc.
}