using Kerbero.Domain.NukiCredentials.Models;
using Kerbero.Domain.SmartLockKeys.Models;
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

  public static SmartLockKeyModel GetSmartLockKey()
  {
    return new SmartLockKeyModel()
    {
      Password = "TOKEN",
      CreationDate = DateTime.Now,
      ExpiryDate = DateTime.Now.AddDays(7),
      UsageCounter = 0,
      IsDisabled = false,
      SmartLockId = "VALID_ID",
      CredentialId = 1
    };
  }
}
