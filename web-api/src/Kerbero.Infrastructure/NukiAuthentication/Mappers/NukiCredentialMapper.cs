using Kerbero.Domain.NukiAuthentication.Dtos;
using Kerbero.Domain.NukiAuthentication.Models;
using Kerbero.Infrastructure.NukiAuthentication.Entities;
using Kerbero.Infrastructure.NukiAuthentication.Models;

namespace Kerbero.Infrastructure.NukiAuthentication.Mappers;

public static class NukiCredentialMapper
{
  public static NukiCredential Map(NukiCredentialEntity entity)
  {
    return new NukiCredential()
    {
      Id = entity.Id,
      ClientId = entity.ClientId,
      CreatedAt = entity.CreatedAt,
      Token = entity.Token,
      RefreshToken = entity.RefreshToken,
      TokenExpiringTimeInSeconds = entity.TokenExpiringTimeInSeconds, // TODO can be extension / computed?
      TokenType = entity.TokenType,
    };
  }


  public static List<NukiCredential> Map(List<NukiCredentialEntity> entities)
  {
    return entities.ConvertAll(Map);
  }

  public static NukiCredentialEntity Map(NukiCredential model)
  {
    return new NukiCredentialEntity()
    {
      ClientId = model.ClientId,
      CreatedAt = model.CreatedAt,
      Token = model.Token,
      RefreshToken = model.RefreshToken,
      TokenExpiringTimeInSeconds = model.TokenExpiringTimeInSeconds,
      TokenType = model.TokenType,
    };
  }

  public static void Map(NukiCredentialEntity entity, NukiCredential model)
  {
    entity.Token = model.Token;
    entity.CreatedAt = model.CreatedAt;
    entity.RefreshToken = model.RefreshToken;
    entity.TokenType = model.TokenType;
    entity.TokenExpiringTimeInSeconds = model.TokenExpiringTimeInSeconds;
  }

  
  
  public static NukiCredential Map(NukiOAuthResponse entity, string clientId, DateTime createdAt)
  {
    return new()
    {
      ClientId = clientId,
      CreatedAt = createdAt,
      Token = entity.Token,
      RefreshToken = entity.RefreshToken,
      TokenExpiringTimeInSeconds = entity.TokenExpiresIn,
      TokenType = entity.TokenType,
    };
  }
}