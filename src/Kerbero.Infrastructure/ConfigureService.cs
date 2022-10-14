using Kerbero.Domain.Common.Interfaces;
using Kerbero.Domain.NukiAuthentication.Interactors;
using Kerbero.Domain.NukiAuthentication.Models;
using Kerbero.Domain.NukiAuthentication.Repositories;
using Kerbero.Infrastructure.Common.Context;
using Kerbero.Infrastructure.Common.Interfaces;
using Kerbero.Infrastructure.NukiAuthentication.Options;
using Kerbero.Infrastructure.NukiAuthentication.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace Kerbero.Infrastructure;

public static class ConfigureService
{
	public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
	{
		services.Configure<NukiExternalOptions>(
			configuration.GetSection(key: nameof(NukiExternalOptions))); 
		services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(configuration.GetConnectionString("Database")!));
		services.AddScoped<IApplicationDbContext, ApplicationDbContext>();
		services.AddScoped<INukiAccountPersistentRepository, NukiAccountPersistentRepository>();
		services.AddScoped<INukiAccountExternalRepository, NukiAccountExternalRepository>();
		return services;
	}
}
