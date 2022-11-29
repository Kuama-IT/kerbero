using Kerbero.Domain.NukiCredentials.Models;

namespace Kerbero.Domain.SmartLocks.Params;

public record GetSmartLocksInteractorParams(List<NukiCredentialModel> NukiCredentials);