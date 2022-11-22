using Kerbero.Domain.NukiActions.Interactors;
using Kerbero.Domain.NukiActions.Interfaces;
using Kerbero.Domain.NukiAuthentication.Interactors;
using Kerbero.Domain.NukiAuthentication.Interfaces;

namespace Kerbero.WebApi;

public static 
	class ConfigureService
{
	public static IServiceCollection AddWebApiServices(this IServiceCollection services)
	{
		services.AddScoped<ICreateNukiAccountAndRedirectToNukiInteractor, CreateNukiAccountAndRedirectToNukiInteractor>();
		services.AddScoped<IUpdateNukiAccountWithToken, UpdateNukiAccountWithToken>();
		services.AddScoped<IAuthenticateNukiAccountInteractor, AuthenticateNukiAccountInteractor>();
		services.AddScoped<IGetNukiSmartLocksInteractor, GetNukiSmartLocksInteractor>();
		services.AddScoped<ICloseNukiSmartLockInteractor, CloseNukiSmartLockInteractor>();
		services.AddScoped<ICreateNukiSmartLockInteractor, CreateNukiSmartLockInteractor>();
		services.AddScoped<IOpenNukiSmartLockInteractor, OpenNukiSmartLockInteractor>();
		

		return services;
	}
}
