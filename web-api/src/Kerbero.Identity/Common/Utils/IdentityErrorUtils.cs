using Microsoft.AspNetCore.Identity;

namespace Kerbero.Identity.Common.Utils;

public static class IdentityErrorUtils
{
  public static IdentityError? FindDuplicateRoleName(IEnumerable<IdentityError> errors)
  {
    return errors.FirstOrDefault(e => e.Code == nameof(IdentityErrorDescriber.DuplicateRoleName));
  }
  
  public static IdentityError? FindDuplicateEmail(IEnumerable<IdentityError> errors)
  {
    return errors.FirstOrDefault(e => e.Code == nameof(IdentityErrorDescriber.DuplicateEmail));
  }
  
  public static IdentityError? FindDuplicateUserName(IEnumerable<IdentityError> errors)
  {
    return errors.FirstOrDefault(e => e.Code == nameof(IdentityErrorDescriber.DuplicateUserName));
  }
  
  public static IdentityError? FindInvalidUserName(IEnumerable<IdentityError> errors)
  {
    return errors.FirstOrDefault(e => e.Code == nameof(IdentityErrorDescriber.InvalidUserName));
  }
  
  public static IdentityError? FindInvalidEmail(IEnumerable<IdentityError> errors)
  {
    return errors.FirstOrDefault(e => e.Code == nameof(IdentityErrorDescriber.InvalidEmail));
  }
}