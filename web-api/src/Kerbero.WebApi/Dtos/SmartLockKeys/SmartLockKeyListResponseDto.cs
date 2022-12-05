using Kerbero.WebApi.Dtos.NukiCredentials;

namespace Kerbero.WebApi.Dtos.SmartLockKeys;

public record SmartLockKeyListResponseDto(
  List<SmartLockKeyResponseDto> SmartLockKeys,
  List<OutdatedNukiCredentialResponseDto> OutdatedCredentials
);