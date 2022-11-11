using FluentValidation;
using Kerbero.Identity.Library.Modules.Users.Dtos;

namespace Kerbero.Identity.Modules.Users.Validators;

public class UserUpdateDtoValidator : AbstractValidator<UserUpdateDto>
{
  public UserUpdateDtoValidator()
  {
    RuleFor(e => e.Email)
      .NotEmpty()
      .EmailAddress();
    
    RuleFor(e => e.UserName)
      .NotEmpty()
      .MinimumLength(3);
  }
}