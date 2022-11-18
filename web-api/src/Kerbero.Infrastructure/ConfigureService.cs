using Kerbero.Domain.NukiActions.Repositories;
using Kerbero.Domain.NukiAuthentication.Repositories;
using Kerbero.Infrastructure.Common.Context;
using Kerbero.Infrastructure.Common.Helpers;
using Kerbero.Infrastructure.Common.Interfaces;
using Kerbero.Infrastructure.NukiActions.Repositories;
using Kerbero.Infrastructure.NukiAuthentication.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace Kerbero.Infrastructure;

public static class ConfigureService
{
	public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
	{
		services.AddDbContext<ApplicationDbContext>(
			options => options.UseNpgsql(configuration["POSTGRESQL_CONNECTION_STRING"]!, 
			x => x.MigrationsAssembly("Kerbero.Infrastructure")));
		
		services.AddScoped<IApplicationDbContext, ApplicationDbContext>();
		services.AddScoped<INukiAccountPersistentRepository, NukiAccountPersistentRepository>();
		services.AddScoped<INukiAccountExternalRepository, NukiAccountExternalRepository>();
		services.AddScoped<INukiSmartLockExternalRepository, NukiSmartLockExternalRepository>();
		services.AddScoped<INukiSmartLockPersistentRepository, NukiSmartLockPersistentRepository>();
		
		services.AddScoped<NukiSafeHttpCallHelper>();
		
		return services;
	}
}
