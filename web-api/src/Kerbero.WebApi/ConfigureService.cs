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
		services.AddScoped<ICreateNukiAccountDraftInteractor, CreateNukiAccountDraftInteractor>();
		services.AddScoped<ICreateNukiAccountInteractor, CreateNukiAccountInteractor>();
		services.AddScoped<IGetNukiAccountInteractor, GetNukiAccountInteractor>();
		services.AddScoped<IGetNukiSmartLocksInteractor, GetNukiSmartLocksInteractor>();
		services.AddScoped<ICloseNukiSmartLockInteractor, CloseNukiSmartLockInteractor>();
		services.AddScoped<ICreateNukiSmartLockInteractor, CreateNukiSmartLockInteractor>();
		services.AddScoped<IOpenNukiSmartLockInteractor, OpenNukiSmartLockInteractor>();

		return services;
	}
}
