using Kerbero.Domain.NukiCredentials.Models;
using Kerbero.Infrastructure.NukiCredentials.Entities;

namespace Kerbero.Infrastructure.NukiCredentials.Mappers;

public static class NukiCredentialMapper
{
  public static NukiCredentialModel Map(NukiCredentialEntity entity)
  {
    return new NukiCredentialModel()
    {
      Id = entity.Id,
      Token = entity.Token,
      UserId = entity.UserId
    };
  }


  public static List<NukiCredentialModel> Map(List<NukiCredentialEntity> entities)
  {
    return entities.ConvertAll(Map);
  }

  public static NukiCredentialEntity Map(NukiCredentialModel model)
  {
    return new NukiCredentialEntity()
    {
      Token = model.Token,
    };
  }
}
