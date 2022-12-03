using Kerbero.Domain.Common.Repositories;
using Kerbero.Domain.NukiActions.Repositories;
using Kerbero.Domain.NukiCredentials.Repositories;
using Kerbero.Domain.SmartLockKeys.Repositories;
using Kerbero.Domain.SmartLocks.Repositories;
using Kerbero.Infrastructure.Common.Helpers;
using Kerbero.Infrastructure.Common.Repositories;
using Kerbero.Infrastructure.NukiActions.Repositories;
using Kerbero.Infrastructure.NukiCredentials.Repositories;
using Kerbero.Infrastructure.SmartLockKeys.Repositories;
using Kerbero.Infrastructure.SmartLocks.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Kerbero.Infrastructure;

public static class ConfigureService
{
  public static void AddInfrastructureServices(this IServiceCollection services)
  {
    services.AddScoped<INukiCredentialRepository, NukiCredentialRepository>();
    services.AddScoped<INukiSmartLockExternalRepository, NukiSmartLockExternalRepository>();
    services.AddScoped<INukiSmartLockRepository, NukiSmartLockRepository>();
    services.AddScoped<ISmartLockKeyRepository, SmartLockKeyRepository>();
    services.AddScoped<IKerberoConfigurationRepository, KerberoConfigurationRepository>();
    services.AddScoped<NukiSafeHttpCallHelper>();
  }
}
