using System.Text.Json;
using FluentAssertions;
using Kerbero.Identity.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Xunit;

namespace Kerbero.Identity.Tests.Common;

public class SwaggerGeneratorAuthorizeOperationFilterTest
{
  private class TestController
  {
    [Authorize]
    public void MethodWithAuthorize()
    {
    }
    
    public void MethodWithoutAuthorize()
    {
    }
  }

  [Fact]
  public void Apply_MutateOperation()
  {
    var schemaGenerator = new SchemaGenerator(
      new SchemaGeneratorOptions(),
      new JsonSerializerDataContractResolver(new JsonSerializerOptions())
    );

    var operationFilterContext = new OperationFilterContext(
      new ApiDescription(),
      schemaGenerator,
      new SchemaRepository(),
      typeof(TestController).GetMethod(nameof(TestController.MethodWithAuthorize))
    );

    var openApiOperation = new OpenApiOperation();

    new SwaggerGeneratorAuthorizeOperationFilter().Apply(openApiOperation, operationFilterContext);

    openApiOperation.Responses["401"]
      .Should().BeEquivalentTo(new OpenApiResponse { Description = "Unauthorized" });

    openApiOperation.Responses["403"]
      .Should().BeEquivalentTo(new OpenApiResponse { Description = "Forbidden" });

    openApiOperation.Security.Should().NotBeEmpty();
  }
  
  [Fact]
  public void Apply_DoNot_MutateOperation()
  {
    var schemaGenerator = new SchemaGenerator(
      new SchemaGeneratorOptions(),
      new JsonSerializerDataContractResolver(new JsonSerializerOptions())
    );

    var operationFilterContext = new OperationFilterContext(
      new ApiDescription(),
      schemaGenerator,
      new SchemaRepository(),
      typeof(TestController).GetMethod(nameof(TestController.MethodWithoutAuthorize))
    );

    var openApiOperation = new OpenApiOperation();

    new SwaggerGeneratorAuthorizeOperationFilter().Apply(openApiOperation, operationFilterContext);

    openApiOperation.Responses.ContainsKey("401").Should().BeFalse();
    
    openApiOperation.Responses.ContainsKey("403").Should().BeFalse();

    openApiOperation.Security.Should().BeEmpty();
  }
}