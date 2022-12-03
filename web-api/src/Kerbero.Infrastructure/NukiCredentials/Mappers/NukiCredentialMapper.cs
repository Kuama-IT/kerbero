using Kerbero.Domain.NukiCredentials.Models;
using Kerbero.Infrastructure.NukiCredentials.Dtos;
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
  
  public static NukiCredentialEntity Map(NukiCredentialDraftModel model)
  {
    return new NukiCredentialEntity()
    {
      IsRefreshable = true,
      IsDraft = true,
      UserId = model.UserId,
    };
  }

  public static void Map(NukiCredentialEntity entity, NukiCredentialModel model)
  {
    entity.Token = model.Token;
  }

  public static void Map(NukiCredentialEntity entity, NukiRefreshableCredentialModel model)
  {
    entity.Token = model.Token;
    entity.RefreshToken = model.RefreshToken;
    entity.CreatedAt = model.CreatedAt;
    entity.ExpiresIn = model.TokenExpiresIn;
    entity.IsRefreshable = true;
    entity.IsDraft = false;
  }

  public static NukiCredentialDraftModel MapAsDraft(NukiCredentialEntity entity)
  {
    return new NukiCredentialDraftModel(UserId: entity.UserId, Id: entity.Id);
  }

  public static NukiRefreshableCredentialModel Map(NukiOAuthResponseDto entity, DateTime createdAt)
  {
    return new NukiRefreshableCredentialModel(Token: entity.Token, Error: entity.Error, RefreshToken: entity.RefreshToken,
      TokenExpiresIn: entity.TokenExpiresIn, CreatedAt: createdAt);
  }
}
