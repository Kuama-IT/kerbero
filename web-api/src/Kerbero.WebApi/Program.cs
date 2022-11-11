using System.Security.Claims;
using Kerbero.Identity.Common;
using Kerbero.Identity.Extensions.DependencyInjection;
using Kerbero.Infrastructure;
using Kerbero.Infrastructure.Common.Context;
using Kerbero.WebApi;

var builder = WebApplication.CreateBuilder(args);

builder.Host.ConfigureLogging(logging =>
{
	logging.ClearProviders();
	logging.AddConsole();
});

// Add services to the container.
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddWebApiServices();
builder.Services.AddKerberoIdentity<ApplicationDbContext>(
	new KerberoIdentityConfiguration()
	{
		AccessTokenExpirationInMinutes = 48 * 60,
	},
	new KerberoIdentityServicesOptions()
	{
		Claims = new List<Claim>
		{
			// keep for reference
		},
		AuthorizationOptionsConfigure = options =>
		{
			// keep for reference
		},
	});


builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option => option.AddKerberoIdentitySwaggerGeneratorOptions());

builder.Services.Configure<RouteOptions>(options => options.LowercaseUrls = true);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

// We need to be on ssl in order to call Nuki Apis
app.UseHttpsRedirection();

app.MapControllers();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.Run();

namespace Kerbero.WebApi
{
	public abstract class Program { }
}