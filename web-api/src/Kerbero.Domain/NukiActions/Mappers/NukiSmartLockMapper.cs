using Kerbero.Domain.NukiActions.Entities;
using Kerbero.Domain.NukiActions.Models.ExternalResponses;
using Kerbero.Domain.NukiActions.Models.PresentationResponse;

namespace Kerbero.Domain.NukiActions.Mappers;

public static class NukiSmartLockMapper
{
    public static KerberoSmartLockPresentationResponse MapToPresentation(NukiSmartLockExternalResponse nukiSmartLockExternalResponse)
    {
        return new KerberoSmartLockPresentationResponse
        {
            ExternalName = nukiSmartLockExternalResponse.Name,
            ExternalType = nukiSmartLockExternalResponse.Type,
            AccountId = nukiSmartLockExternalResponse.AccountId,
            ExternalSmartLockId = nukiSmartLockExternalResponse.SmartLockId
        };
    }
    
    // entity -> presentation
    public static KerberoSmartLockPresentationResponse MapToPresentation(
        NukiSmartLockEntity entity)
    {
        return new KerberoSmartLockPresentationResponse
        {
            ExternalName = entity.Name,
            ExternalType = entity.Type,
            AccountId = entity.NukiAccountId,
            ExternalSmartLockId = entity.ExternalSmartLockId,
            SmartLockId = entity.Id
        };
    }

    // external -> entity
    public static NukiSmartLockEntity MapToEntity(NukiSmartLockExternalResponse externalResponseDto, int accountId)
    {
        return new NukiSmartLockEntity
        {
            Favourite = externalResponseDto.Favourite,
            Name = externalResponseDto.Name,
            State = externalResponseDto.State is null ? null : MapToEntity(externalResponseDto.State),
            Type = externalResponseDto.Type,
            AuthId = externalResponseDto.AuthId,
            ExternalSmartLockId = externalResponseDto.SmartLockId,
            NukiAccountId = accountId
        };
    }

    // external -> entity
    private static NukiSmartLockStateEntity MapToEntity(NukiSmartLockStateExternalResponse externalDto)
    {
        return new NukiSmartLockStateEntity
        {
            Mode = externalDto.Mode,
            State = externalDto.State,
            BatteryCharge = externalDto.BatteryCharge,
            BatteryCharging = externalDto.BatteryCharging,
            BatteryCritical = externalDto.BatteryCharging,
            DoorState = externalDto.DoorState,
            LastAction = externalDto.LastAction,
            OperationId = externalDto.OperationId
        };
    }
}
