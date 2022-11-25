using FluentResults;
using Kerbero.Domain.NukiAuthentication.Dtos;
using Kerbero.Domain.NukiAuthentication.Models;

namespace Kerbero.Domain.NukiAuthentication.Mappers;

public static class NukiCredentialMapper
{
  public static NukiCredentialDto Map(NukiCredential model)
  {
    return new NukiCredentialDto
    {
      Id = model.Id ??
           throw new ArgumentNullException(nameof(model), "The value of 'accountResultValue.Id' should not be null"),
      ClientId = model.ClientId,
      Token = model.Token
    };
  }


  public static List<NukiCredential> Map(List<NukiCredentialDto> dtos)
  {
    return dtos.ConvertAll(Map);
  }

  public static NukiCredential Map(NukiCredentialDto dto)
  {
    return new NukiCredential
    {
      Id = dto.Id,
      Token = dto.Token,
      ClientId = dto.ClientId
    };
  }
}