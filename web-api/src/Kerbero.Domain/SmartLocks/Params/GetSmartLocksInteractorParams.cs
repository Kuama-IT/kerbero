using Kerbero.Domain.NukiAuthentication.Models;

namespace Kerbero.Domain.SmartLocks.Params;

public record GetSmartLocksInteractorParams(List<NukiCredential> NukiCredentials);