namespace Kerbero.WebApi.Dtos.NukiCredentials;

public record NukiCredentialListResponseDto(
  List<NukiCredentialResponseDto> Credentials,
  List<OutdatedNukiCredentialResponseDto> OutdatedCredentials
  );