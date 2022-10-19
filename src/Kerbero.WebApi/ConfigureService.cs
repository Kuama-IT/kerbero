using Kerbero.Domain.Common.Interfaces;
using Kerbero.Domain.NukiActions.Interactors;
using Kerbero.Domain.NukiActions.Interfaces;
using Kerbero.Domain.NukiActions.Models;
using Kerbero.Domain.NukiActions.Repositories;
using Kerbero.Domain.NukiAuthentication.Interactors;
using Kerbero.Domain.NukiAuthentication.Interfaces;
using Kerbero.Domain.NukiAuthentication.Models;
using Kerbero.Infrastructure.NukiActions;

namespace Kerbero.WebApi;

public static 
	class ConfigureService
{
	public static IServiceCollection AddWebApiServices(this IServiceCollection services)
	{
		services.AddScoped<IProvideNukiAuthRedirectUrlInteractor, ProvideNukiAuthRedirectUrlInteractor>();
		services.AddScoped<ICreateNukiAccountInteractor, CreateNukiAccountInteractor>();
		services.AddScoped<IAuthenticateNukiAccountInteractor, AuthenticateNukiAccountInteractor>();
		services.AddScoped<IGetNukiSmartLockListInteractor, GetNukiSmartLockListInteractor>();

		return services;
	}
}
