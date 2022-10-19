using Kerbero.Domain.NukiActions.Models;
using Kerbero.Domain.NukiActions.Models.ExternalResponses;
using Kerbero.Domain.NukiActions.Models.PresentationRequest;

namespace Kerbero.Domain.NukiActions.Mappers;

public static class NukiSmartLockMapper
{
    public static KerberoSmartLockPresentationRequest Map(NukiSmartLockExternalResponse nukiSmartLockExternalResponse)
    {
        return new KerberoSmartLockPresentationRequest
        {
            ExternalName = nukiSmartLockExternalResponse.Name,
            ExternalType = nukiSmartLockExternalResponse.Type,
            ExternalAccountId = nukiSmartLockExternalResponse.AccountId,
            ExternalSmartLockId = nukiSmartLockExternalResponse.AuthId
        };
    }
}