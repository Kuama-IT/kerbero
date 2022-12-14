using System.Security.Claims;
using DotNetEnv;
using Kerbero.Identity.Common;
using Kerbero.Identity.Extensions.DependencyInjection;
using Kerbero.Data;
using Kerbero.Data.Common.Context;
using Kerbero.Data.Common.Interfaces;
using Kerbero.WebApi;
using Kerbero.WebApi.Exceptions;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables(_ => Env.Load());

var connectionString = builder.Configuration["POSTGRESQL_CONNECTION_STRING"];
if (connectionString is null)
{
  throw new DevException("[DEV] missing env POSTGRESQL_CONNECTION_STRING ");
}

builder.Services.AddDbContext<IApplicationDbContext, ApplicationDbContext>(
  options => options.UseNpgsql(connectionString, b => b.MigrationsAssembly("Kerbero.Data")));

// Add services to the container.
builder.Services.AddInfrastructureServices();
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
builder.Services.AddCors(options =>
{
	options.AddPolicy("Kerbero", policyBuilder =>
	{
		policyBuilder
			.AllowCredentials()
			.AllowAnyHeader()
			.AllowAnyMethod()
			.AllowAnyOrigin()
			.WithOrigins("http://127.0.0.1:5173", "http://localhost:5173", "https://127.0.0.1:5173", "https://localhost:5173", "https://test.com:5173");
	});
});

builder.Services.AddControllers(options =>
{
  options.Conventions.Add(new RouteTokenTransformerConvention(new SlugifyOutboundParameterTransformer()));
});

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

app.UseCors("Kerbero");
app.Run();

namespace Kerbero.WebApi
{
  public abstract class Program
  {
  }
}
