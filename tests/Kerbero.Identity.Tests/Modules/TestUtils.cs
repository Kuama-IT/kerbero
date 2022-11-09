using Kerbero.Identity.Modules.Roles.Entities;
using Kerbero.Identity.Modules.Users.Entities;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace Kerbero.Identity.Tests.Modules;

public static class TestUtils
{
  public static Mock<UserManager<User>> GetUserManagerMock(IUserStore<User> store) =>
    new(store, null, null, null, null, null, null, null, null);

  public static Mock<RoleManager<Role>> GetRoleManagerMock => new(null, null, null, null, null);
  public static Mock<SignInManager<User>> GeSignInManagerMock => new(null, null, null, null, null, null, null);
}