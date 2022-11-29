using Kerbero.Domain.NukiActions.Interactors;
using Kerbero.Domain.NukiActions.Interfaces;
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
    services.AddScoped<ICloseNukiSmartLockInteractor, CloseNukiSmartLockInteractor>();
    services.AddScoped<ICreateNukiSmartLockInteractor, CreateNukiSmartLockInteractor>();
    services.AddScoped<IOpenNukiSmartLockInteractor, OpenNukiSmartLockInteractor>();
    services.AddScoped<IGetNukiCredentialsByUserInteractor, GetNukiCredentialsByUserInteractor>();
    services.AddScoped<IGetSmartLocksInteractor, GetSmartLocksInteractor>();
  }
}
