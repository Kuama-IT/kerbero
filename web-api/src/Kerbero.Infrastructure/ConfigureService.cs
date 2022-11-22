using Kerbero.Domain.NukiActions.Repositories;
using Kerbero.Domain.NukiAuthentication.Repositories;
using Kerbero.Infrastructure.Common.Context;
using Kerbero.Infrastructure.Common.Helpers;
using Kerbero.Infrastructure.Common.Interfaces;
using Kerbero.Infrastructure.NukiActions.Repositories;
using Kerbero.Infrastructure.NukiAuthentication.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace Kerbero.Infrastructure;

public static class ConfigureService
{
  public static void AddInfrastructureServices(this IServiceCollection services)
  {
    services.AddScoped<INukiCredentialRepository, NukiCredentialRepository>();
    services.AddScoped<INukiOAuthRepository, NukiOAuthRepository>();
    services.AddScoped<INukiSmartLockExternalRepository, NukiSmartLockExternalRepository>();
    services.AddScoped<INukiSmartLockPersistentRepository, NukiSmartLockPersistentRepository>();
    services.AddScoped<INukiCredentialDraftRepository, NukiCredentialDraftRepository>();

    services.AddScoped<NukiSafeHttpCallHelper>();
  }
}