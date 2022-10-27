using Kerbero.Domain.NukiActions.Models;
using Kerbero.Domain.NukiActions.Models.ExternalResponses;
using Kerbero.Domain.NukiActions.Models.PresentationResponse;

namespace Kerbero.Domain.NukiActions.Mappers;

public static class NukiSmartLockMapper
{
    public static KerberoSmartLockPresentationResponse Map(NukiSmartLockExternalResponse nukiSmartLockExternalResponse)
    {
        return new KerberoSmartLockPresentationResponse
        {
            ExternalName = nukiSmartLockExternalResponse.Name,
            ExternalType = nukiSmartLockExternalResponse.Type,
            ExternalAccountId = nukiSmartLockExternalResponse.AccountId,
            ExternalSmartLockId = nukiSmartLockExternalResponse.AuthId
        };
    }
}