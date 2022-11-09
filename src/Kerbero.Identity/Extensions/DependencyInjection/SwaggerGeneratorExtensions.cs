using Kerbero.Identity.Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Kerbero.Identity.Extensions.DependencyInjection;

public static class SwaggerGeneratorExtensions
{
  internal static string SwaggerSecurityDefinitionName = "Kuama Identity Token";

  public static void AddKerberoIdentitySwaggerGeneratorOptions(this SwaggerGenOptions options)
  {
    options.AddSecurityDefinition(SwaggerSecurityDefinitionName, new OpenApiSecurityScheme
    {
      Name = "Authorization",
      Description = "Kuama Identity Token",
      Type = SecuritySchemeType.Http,
      Scheme = "Bearer",
      In = ParameterLocation.Header,
    });

    options.OperationFilter<SwaggerGeneratorAuthorizeOperationFilter>();
  }
}