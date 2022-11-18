using System.Security.Claims;
using DotNetEnv;
using Kerbero.Identity.Common;
using Kerbero.Identity.Extensions;
using Kerbero.Identity.Extensions.DependencyInjection;
using Kerbero.Infrastructure;
using Kerbero.Infrastructure.Common.Context;
using Kerbero.WebApi;
using Kerbero.WebApi.Exceptions;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables(_ => Env.Load());

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
		CookieExpirationInTimeSpan = TimeSpan.FromDays(1),
		SendGridKey = builder.Configuration["SENDGRID_SECRET"] ?? 
		              throw new DevException("SendGrid secrets keys not set")
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
		}
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
