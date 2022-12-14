using Kerbero.Domain.NukiCredentials.Interactors;
using Kerbero.Domain.NukiCredentials.Interfaces;
using Kerbero.Domain.SmartLockKeys.Interactors;
using Kerbero.Domain.SmartLockKeys.Interfaces;
using Kerbero.Domain.SmartLocks.Interactors;
using Kerbero.Domain.SmartLocks.Interfaces;

namespace Kerbero.WebApi;

public static class ConfigureService
{
  public static void AddWebApiServices(this IServiceCollection services)
  {
    services.AddScoped<ICreateNukiCredentialInteractor, CreateNukiCredentialInteractor>();
    services.AddScoped<IGetNukiCredentialInteractor, GetNukiCredentialInteractor>();
    services.AddScoped<IGetNukiCredentialsByUserInteractor, GetNukiCredentialsByUserInteractor>();
    services.AddScoped<IGetSmartLocksInteractor, GetSmartLocksInteractor>();
    services.AddScoped<IOpenSmartLockInteractor, OpenSmartLockInteractor>();
    services.AddScoped<ICreateSmartLockKeyInteractor, CreateSmartLockKeyInteractor>();
    services.AddScoped<IGetSmartLockKeysInteractor, GetSmartLockKeysInteractor>();
    services.AddScoped<IOpenSmartLockWithKeyInteractor, OpenSmartLockWithKeyInteractor>();
    services.AddScoped<ICloseSmartLockWithKeyInteractor, CloseSmartLockWithKeyInteractor>();
    services.AddScoped<ICreateNukiCredentialDraftInteractor, CreateNukiCredentialDraftInteractor>();
    services.AddScoped<IConfirmNukiDraftCredentialsInteractor, ConfirmNukiDraftCredentialsInteractor>();
    services.AddScoped<IBuildNukiRedirectUriInteractor, BuildNukiRedirectUriInteractor>();
    services.AddScoped<ICloseSmartLockInteractor, CloseSmartLockInteractor>();
    services.AddScoped<IDeleteSmartLockKeyInteractor, DeleteSmartLockKeyInteractor>();
    services.AddScoped<IDeleteNukiCredentialInteractor, DeleteNukiCredentialInteractor>();
    services.AddScoped<IUpdateSmartLockKeyValidityInteractor, UpdateSmartLockKeyValidityInteractor>();
    services.AddScoped<IEnsureNukiCredentialBelongsToUserInteractor, EnsureNukiCredentialBelongsToUserInteractor>();
    services.AddScoped<IBuildWebAppRedirectUriInteractor, BuildWebAppRedirectUriInteractor>();
  }
}
