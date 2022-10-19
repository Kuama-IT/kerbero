using Kerbero.Domain.Common.Interfaces;
using Kerbero.Domain.Common.Models;
using Kerbero.Domain.NukiActions.Interactors;
using Kerbero.Domain.NukiActions.Models;
using Kerbero.Domain.NukiActions.Repositories;
using Kerbero.Domain.NukiAuthentication.Interactors;
using Kerbero.Domain.NukiAuthentication.Models;
using Kerbero.Infrastructure.NukiActions;

namespace Kerbero.WebApi;

public static 
	class ConfigureService
{
	public static IServiceCollection AddWebApiServices(this IServiceCollection services)
	{
		services.AddScoped<Interactor<NukiRedirectExternalRequestDto, NukiRedirectPresentationDto>, ProvideNukiAuthRedirectUrlInteractor>();
		services.AddScoped<InteractorAsync<NukiAccountExternalRequestDto, NukiAccountPresentationDto>, CreateNukiAccountInteractor>();
		services.AddScoped<InteractorAsync<NukiAccountAuthenticatedRequestDto, NukiAccountAuthenticatedResponseDto>, AuthenticateNukiAccountInteractor>();
		services.AddScoped<InteractorAsyncNoParam<List<KerberoSmartLockPresentationDto>>, GetNukiSmartLocksListInteractor>();
		services.AddScoped<INukiSmartLockExternalRepository, NukiSmartLockExternalRepository>();

		return services;
	}
}
