using Kerbero.Common.Interactors;
using Kerbero.Common.Interfaces;
using Kerbero.Common.Models;

namespace Kerbero.WebApi;

public static 
	class ConfigureService
{
	public static IServiceCollection AddWebApiServices(this IServiceCollection services)
	{
		services.AddScoped<Interactor<NukiRedirectExternalRequestDto, NukiRedirectPresentationDto>, ProvideNukiAuthRedirectUrlInteractor>();
		services.AddScoped<InteractorAsync<NukiAccountExternalRequestDto, NukiAccountPresentationDto>, CreateNukiAccountInteractor>();

		return services;
	}
}
