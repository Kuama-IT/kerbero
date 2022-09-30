using Kerbero.Domain.Common.Interfaces;
using Kerbero.Domain.NukiAuthentication.Interactors;
using Kerbero.Domain.NukiAuthentication.Models;

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
