using Kerbero.Domain.Common.Models;
using Kerbero.Domain.NukiActions.Entities;
using Kerbero.Domain.NukiActions.Models.ExternalResponses;
using Kerbero.Domain.NukiActions.Models.PresentationResponse;

namespace Kerbero.Domain.NukiActions.Models.Mapper;

public static class NukiSmartLockMapper
{
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
    public static NukiSmartLock MapToEntity(NukiSmartLockExternalResponse externalResponseDto)
    {
        return new NukiSmartLock
        {
            Favourite = externalResponseDto.Favourite,
            Name = externalResponseDto.Name,
            State = externalResponseDto.State is null ? null : MapToEntity(externalResponseDto.State),
            Type = externalResponseDto.Type,
            AuthId = externalResponseDto.AuthId,
            ExternalSmartLockId = externalResponseDto.SmartLockId,
            NukiAccountId = externalResponseDto.AccountId
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