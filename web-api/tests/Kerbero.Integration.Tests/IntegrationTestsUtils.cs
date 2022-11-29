using Kerbero.Domain.NukiAuthentication.Models;
using Kerbero.Identity.Modules.Users.Entities;

namespace Kerbero.Integration.Tests;

public static class IntegrationTestsUtils
{
  #region Seed DB

  public static NukiCredentialModel GetNukiCredential()
  {
    return new NukiCredentialModel()
    {
      Token = "VALID_TOKEN",
    };
  }
  #endregion
  
  public static User GetSeedingUser()
  {
    return new User
    {
      Email = "test@test.com",
      EmailConfirmed = true,
      UserName = "test",
    };
  }
}
