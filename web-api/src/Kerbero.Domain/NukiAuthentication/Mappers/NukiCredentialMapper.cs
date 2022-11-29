using Kerbero.Domain.NukiAuthentication.Dtos;
using Kerbero.Domain.NukiAuthentication.Models;

namespace Kerbero.Domain.NukiAuthentication.Mappers;

public static class NukiCredentialMapper
{
  public static NukiCredentialDto Map(NukiCredentialModel model)
  {
    return new NukiCredentialDto
    {
      Id = model.Id,
      Token = model.Token
    };
  }

  public static List<NukiCredentialDto> Map(List<NukiCredentialModel> models)
  {
    return models.ConvertAll(Map);
  }

  public static List<NukiCredentialModel> Map(List<NukiCredentialDto> dtos)
  {
    return dtos.ConvertAll(Map);
  }

  public static NukiCredentialModel Map(NukiCredentialDto dto)
  {
    return new NukiCredentialModel
    {
      Id = dto.Id,
      Token = dto.Token,
    };
  }
}
