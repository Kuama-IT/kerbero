using System;
using Kerbero.Domain.SmartLocks.Models;

namespace Kerbero.Domain.SmartLocks.Dtos;

public class OpenSmartLockParams
{
  public required Guid UserId { get; set; }

  public required SmartLockProvider SmartLockProvider { get; set; }

  public required string SmartLockId { get; set; }

  public int CredentialId { get; set; }
}