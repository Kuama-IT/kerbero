using FluentResults;
using Kerbero.Domain.NukiCredentials.Models;

namespace Kerbero.Domain.SmartLockKeys.Models;

public record UserSmartLockKeysModel(
  List<SmartLockKeyModel> SmartLockKeys,
  List<(NukiCredentialModel, List<IError>)> OutdatedCredentials
);