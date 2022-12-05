namespace Kerbero.WebApi.Dtos.NukiCredentials;

public record OutdatedNukiCredentialResponseDto
(
  int Id,
  string NukiEmail,
  List<string> Errors
);