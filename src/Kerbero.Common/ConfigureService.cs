using Microsoft.Extensions.DependencyInjection;

namespace Kerbero.Common;

public static 
	class ConfigureService
{
	public static IServiceCollection AddDomainServices(this IServiceCollection services)
	{
		return services;
	}
}
