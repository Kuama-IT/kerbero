using Kerbero.Identity.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Kerbero.Identity.Common;

public class SwaggerGeneratorAuthorizeOperationFilter : IOperationFilter
{
  public void Apply(OpenApiOperation operation, OperationFilterContext context)
  {
    // Policy names map to scopes
    var requiredScopes = context.MethodInfo
      .GetCustomAttributes(true)
      .OfType<AuthorizeAttribute>()
      .Select(attr => attr.Policy)
      .Distinct()
      .ToList();

    if (!requiredScopes.Any()) return;

    operation.Responses.Add("401", new OpenApiResponse { Description = "Unauthorized" });
    operation.Responses.Add("403", new OpenApiResponse { Description = "Forbidden" });

    var openApiSecurityScheme = new OpenApiSecurityScheme
    {
      Reference = new OpenApiReference
        { Type = ReferenceType.SecurityScheme, Id = SwaggerGeneratorExtensions.SwaggerSecurityDefinitionName }
    };

    operation.Security = new List<OpenApiSecurityRequirement>
    {
      new()
      {
        [openApiSecurityScheme] = requiredScopes
      }
    };
  }
}