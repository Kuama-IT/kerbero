using FluentResults;
using Kerbero.Domain.Common.Repositories;
using Kerbero.Domain.NukiCredentials.Models;
using Microsoft.Extensions.Configuration;

namespace Kerbero.Data.Common.Repositories;

public class KerberoConfigurationRepository : IKerberoConfigurationRepository
{
  private readonly IConfiguration _configuration;

  public KerberoConfigurationRepository(IConfiguration configuration)
  {
    _configuration = configuration;
  }

  public Task<Result<NukiApiConfigurationModel>> GetNukiApiDefinition()
  {
    return Task.FromResult<Result<NukiApiConfigurationModel>>(new NukiApiConfigurationModel(
      ApiEndpoint: _configuration["NUKI_DOMAIN"]!,
      ClientId: _configuration["NUKI_CLIENT_ID"]!,
      Scopes: _configuration["NUKI_SCOPES"]!,
      ApplicationRedirectEndpoint: _configuration["NUKI_REDIRECT_FOR_TOKEN"]!,
      ApplicationDomain: _configuration["ALIAS_DOMAIN"]!
    ));
  }
}