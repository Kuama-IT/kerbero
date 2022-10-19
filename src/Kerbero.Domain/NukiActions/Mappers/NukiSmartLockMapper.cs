using Kerbero.Domain.Common.Models;
using Kerbero.Domain.NukiActions.Models;

namespace Kerbero.Domain.NukiActions.Mappers;

public static class NukiSmartLockMapper
{
    public static KerberoSmartLockPresentationDto Map(NukiSmartLockExternalResponseDto nukiSmartLockExternalResponseDto)
    {
        return new KerberoSmartLockPresentationDto
        {
            ExternalName = nukiSmartLockExternalResponseDto.Name,
            ExternalType = nukiSmartLockExternalResponseDto.Type,
            ExternalAccountId = nukiSmartLockExternalResponseDto.AccountId,
            ExternalSmartLockId = nukiSmartLockExternalResponseDto.AuthId
        };
    }
}