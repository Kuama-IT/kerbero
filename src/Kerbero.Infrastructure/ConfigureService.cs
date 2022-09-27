using Kerbero.Common.Repositories;
using Kerbero.Infrastructure.Context;
using Kerbero.Infrastructure.Interfaces;
using Kerbero.Infrastructure.Options;
using Kerbero.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace Kerbero.Infrastructure;

public static 
	class ConfigureService
{
	public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
	{
		services.Configure<NukiExternalOptions>(
			configuration.GetSection(key: nameof(NukiExternalOptions))); 
		services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(configuration.GetConnectionString("KerberoDatabase")!));
		services.AddScoped<IApplicationDbContext, ApplicationDbContext>();
		services.AddScoped<INukiPersistentAccountRepository, NukiPersistentAccountRepository>();
		services.AddScoped<INukiExternalAuthenticationRepository, NukiExternalAuthenticationRepository>();
		return services;
	}
}
