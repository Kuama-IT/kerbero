using System.Text.Json;
using Kerbero.Domain.NukiActions.Entities;
using Kerbero.Domain.NukiAuthentication.Models;
using Kerbero.Identity.Modules.Users.Entities;
using Kerbero.Infrastructure.Common.Context;
using Kerbero.Infrastructure.NukiAuthentication.Entities;

namespace Kerbero.Integration.Tests;

public static class IntegrationTestsUtils
{
  #region Seed DB

  public static NukiCredential GetNukiCredential()
  {
    return new NukiCredential()
    {
      Token = "VALID_TOKEN",
      RefreshToken = "VALID_REFRESH_TOKEN",
      TokenExpiringTimeInSeconds = 2592000,
      ClientId = "VALID_CLIENT_ID",
      TokenType = "bearer",
      CreatedAt = DateTime.Now,
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