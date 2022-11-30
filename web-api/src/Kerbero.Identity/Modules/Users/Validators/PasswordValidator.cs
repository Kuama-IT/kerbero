using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Kerbero.Identity.Library.Modules.Users.Models;


namespace Kerbero.Identity.Modules.Users.Validators;

public class PasswordValidator : AbstractValidator<IHavePassword>
{
  // check -> Microsoft.AspNetCore.Identity -> PasswordValidator
  public PasswordValidator(IOptions<IdentityOptions> identityOptions)
  {
    var passwordOptions = identityOptions.Value.Password;

    RuleFor(e => e.Password)
      .NotEmpty()
      .MinimumLength(passwordOptions.RequiredLength);

    var describer = new IdentityErrorDescriber();

    if (passwordOptions.RequireNonAlphanumeric)
    {
      RuleFor(e => e.Password)
        .Matches(@"[^a-zA-Z\d]")
        .WithMessage(describer.PasswordRequiresNonAlphanumeric().Description);
    }

    if (passwordOptions.RequireDigit)
    {
      RuleFor(e => e.Password)
        .Matches(@"\d")
        .WithMessage(describer.PasswordRequiresDigit().Description);
    }

    if (passwordOptions.RequireLowercase)
    {
      RuleFor(e => e.Password)
        .Matches(@"[a-z]")
        .WithMessage(describer.PasswordRequiresLower().Description);
    }

    if (passwordOptions.RequireUppercase)
    {
      RuleFor(e => e.Password)
        .Matches(@"[A-Z]")
        .WithMessage(describer.PasswordRequiresUpper().Description);
    }

    if (passwordOptions.RequiredUniqueChars > 0)
    {
      RuleFor(e => e.Password)
        .Must(password => password.Distinct().Count() >= passwordOptions.RequiredUniqueChars)
        .WithMessage(describer.PasswordRequiresUniqueChars(passwordOptions.RequiredUniqueChars).Description);
    }
  }
}