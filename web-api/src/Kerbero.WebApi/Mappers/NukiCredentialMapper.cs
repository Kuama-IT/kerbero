using FluentResults;
using Kerbero.Domain.NukiCredentials.Models;
using Kerbero.WebApi.Dtos;

namespace Kerbero.WebApi.Mappers;

public static class NukiCredentialMapper
{
  public static NukiCredentialDraftResponseDto Map(Uri uri)
  {
    return new NukiCredentialDraftResponseDto(RedirectUrl: uri.ToString());
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
}