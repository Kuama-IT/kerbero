using Kerbero.Domain.Common.Repositories;
using Kerbero.Domain.NukiCredentials.Repositories;
using Kerbero.Domain.SmartLockKeys.Repositories;
using Kerbero.Domain.SmartLocks.Repositories;
using Kerbero.Data.Common.Helpers;
using Kerbero.Data.Common.Repositories;
using Kerbero.Data.NukiCredentials.Repositories;
using Kerbero.Data.SmartLockKeys.Repositories;
using Kerbero.Data.SmartLocks.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Kerbero.Data;

public static class ConfigureService
{
  public static void AddInfrastructureServices(this IServiceCollection services)
  {
    services.AddScoped<INukiCredentialRepository, NukiCredentialRepository>();
    services.AddScoped<INukiSmartLockRepository, NukiSmartLockRepository>();
    services.AddScoped<ISmartLockKeyRepository, SmartLockKeyRepository>();
    services.AddScoped<IKerberoConfigurationRepository, KerberoConfigurationRepository>();
    services.AddScoped<NukiRestApiClient>();
  }
}
