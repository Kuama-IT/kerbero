using Microsoft.Extensions.DependencyInjection;

namespace Kerbero.Infrastructure;

public static 
	class ConfigureService
{
	public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
	{
		return services;
	}
}
