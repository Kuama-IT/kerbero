using Kerbero.Domain.NukiCredentials.Interactors;
using Kerbero.Domain.NukiCredentials.Interfaces;
using Kerbero.Domain.SmartLocks.Interactors;
using Kerbero.Domain.SmartLocks.Interfaces;

namespace Kerbero.WebApi;

public static
  class ConfigureService
{
  public static void AddWebApiServices(this IServiceCollection services)
  {
    services.AddScoped<ICreateNukiCredentialInteractor, CreateNukiCredentialInteractor>();
    services.AddScoped<IGetNukiCredentialInteractor, GetNukiCredentialInteractor>();
    services.AddScoped<IGetNukiCredentialsByUserInteractor, GetNukiCredentialsByUserInteractor>();
    services.AddScoped<IGetSmartLocksInteractor, GetSmartLocksInteractor>();
    services.AddScoped<IOpenSmartLockInteractor, OpenSmartLockInteractor>();
  }
}
