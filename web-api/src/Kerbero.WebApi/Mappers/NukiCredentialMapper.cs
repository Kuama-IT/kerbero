using FluentResults;
using Kerbero.Domain.NukiCredentials.Models;
using Kerbero.WebApi.Dtos.NukiCredentials;

namespace Kerbero.WebApi.Mappers;

public static class NukiCredentialMapper
{
  public static NukiCredentialDraftResponseDto Map(Uri uri)
  {
    return new NukiCredentialDraftResponseDto(RedirectUrl: uri.ToString());
  }

  public static List<OutdatedNukiCredentialResponseDto> Map(
    List<(NukiCredentialModel, List<IError>)> outdatedCredentials)
  {
    return outdatedCredentials.ConvertAll(Map);
  }

  public static OutdatedNukiCredentialResponseDto Map((NukiCredentialModel, List<IError>) outdatedCredential)
  {
    return new OutdatedNukiCredentialResponseDto(
      Id: outdatedCredential.Item1.Id,
      NukiEmail: outdatedCredential.Item1.NukiEmail,
      Errors: outdatedCredential.Item2.Select(it => it.Message).ToList()
    );
  }

  public static NukiCredentialResponseDto Map(NukiCredentialModel model)
  {
    return new NukiCredentialResponseDto
    {
      Id = model.Id,
      Token = model.Token
    };
  }

  public static List<NukiCredentialResponseDto> Map(List<NukiCredentialModel> models)
  {
    return models.ConvertAll(Map);
  }

  public static NukiCredentialListResponseDto Map(UserNukiCredentialsModel model)
  {
    return new NukiCredentialListResponseDto(
      Credentials: Map(model.NukiCredentials),
      OutdatedCredentials: Map(model.OutdatedCredentials)
    );
  }
}