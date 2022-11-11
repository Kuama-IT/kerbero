using FluentValidation;
using Kerbero.Identity.Library.Modules.Users.Dtos;
using Kerbero.Identity.Library.Modules.Users.Models;

namespace Kerbero.Identity.Modules.Users.Validators;

public class UserCreateDtoValidator : AbstractValidator<UserCreateDto>
{
  public UserCreateDtoValidator(IValidator<IHavePassword> passwordValidator, IValidator<UserUpdateDto> updateValidator)
  {
    Include(updateValidator);

    Include(passwordValidator);
  }
}