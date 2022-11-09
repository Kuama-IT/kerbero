using Kerbero.Identity.Modules.Authentication.Controllers;
using Kerbero.Identity.Modules.Roles.Controllers;
using Kerbero.Identity.Modules.Users.Controllers;
using Microsoft.Extensions.DependencyInjection;

namespace Kerbero.Identity.Extensions.DependencyInjection;

public static class MvcBuilderExtensions
{
  public static void AddKerberoIdentityControllers(this IMvcBuilder builder)
  {
    builder
      .AddApplicationPart(typeof(AuthenticationController).Assembly)
      .AddApplicationPart(typeof(UsersController).Assembly)
      .AddApplicationPart(typeof(RolesController).Assembly)
      ;
  }
}