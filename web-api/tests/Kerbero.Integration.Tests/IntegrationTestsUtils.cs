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

  public static NukiSmartLockEntity GetSeedingNukiSmartLock()
  {
    return new NukiSmartLockEntity
    {
      Favourite = true,
      Name = "kquarter",
      Type = 0,
      NukiAccountId = 1,
      AuthId = 0,
      ExternalSmartLockId = 0,
      State = new NukiSmartLockStateEntity
      {
        Mode = 4,
        State = 255,
        LastAction = 5,
        BatteryCritical = true,
        BatteryCharging = true,
        BatteryCharge = 100,
        DoorState = 255,
        OperationId = "string"
      }
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