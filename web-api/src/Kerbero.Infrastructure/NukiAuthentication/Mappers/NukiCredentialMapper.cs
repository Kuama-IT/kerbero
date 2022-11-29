using Kerbero.Domain.NukiAuthentication.Models;
using Kerbero.Infrastructure.NukiAuthentication.Entities;

namespace Kerbero.Infrastructure.NukiAuthentication.Mappers;

public static class NukiCredentialMapper
{
  public static NukiCredentialModel Map(NukiCredentialEntity entity)
  {
    return new NukiCredentialModel()
    {
      Id = entity.Id,
      Token = entity.Token,
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

  public static void Map(NukiCredentialEntity entity, NukiCredentialModel model)
  {
    entity.Token = model.Token;
  }
}
