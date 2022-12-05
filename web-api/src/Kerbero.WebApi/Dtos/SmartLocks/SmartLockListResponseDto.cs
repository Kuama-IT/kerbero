using FluentResults;
using Kerbero.Domain.NukiCredentials.Models;
using Kerbero.WebApi.Dtos.NukiCredentials;

namespace Kerbero.WebApi.Dtos.SmartLocks;

/// <summary>
/// 
/// </summary>
/// <param name="SmartLocks"></param>
/// <param name="OutdatedCredentials"></param>
public record SmartLockListResponseDto(
  List<SmartLockResponseDto> SmartLocks,
  List<OutdatedNukiCredentialResponseDto> OutdatedCredentials);