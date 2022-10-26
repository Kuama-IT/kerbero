using Kerbero.Domain.NukiActions.Entities;
using Kerbero.Domain.NukiActions.Models;
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
            ExternalAccountId = nukiSmartLockExternalResponse.AccountId,
            ExternalSmartLockId = nukiSmartLockExternalResponse.AuthId
        };
    }
    
    // entity -> presentation
    public static KerberoSmartLockPresentationResponse MapToPresentation(
        NukiSmartLock entity)
    {
        return new KerberoSmartLockPresentationResponse
        {
            ExternalName = entity.Name,
            ExternalType = entity.Type,
            ExternalAccountId = entity.NukiAccountId,
            ExternalSmartLockId = entity.ExternalSmartLockId
        };
    }

    // external -> entity
    public static NukiSmartLock MapToEntity(NukiSmartLockExternalResponse externalResponseDto, int accountId)
    {
        return new NukiSmartLock
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
    private static NukiSmartLockState MapToEntity(NukiSmartLockStateExternalResponse externalDto)
    {
        return new NukiSmartLockState
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