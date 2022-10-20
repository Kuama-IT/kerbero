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
		services.AddScoped<IProvideNukiAuthRedirectUrlInteractor, ProvideNukiAuthRedirectUrlInteractor>();
		services.AddScoped<ICreateNukiAccountInteractor, CreateNukiAccountInteractor>();
		services.AddScoped<IAuthenticateNukiAccountInteractor, AuthenticateNukiAccountInteractor>();
		services.AddScoped<IGetNukiSmartLockListInteractor, GetNukiSmartLocksInteractor>();

		return services;
	}
}
