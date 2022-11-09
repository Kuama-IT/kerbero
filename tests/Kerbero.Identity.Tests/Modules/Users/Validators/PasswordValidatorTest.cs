using FluentValidation.TestHelper;
using Kerbero.Identity.Library.Modules.Users.Dtos;
using Kerbero.Identity.Modules.Users.Validators;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Xunit;

namespace Kerbero.Identity.Tests.Modules.Users.Validators;

public class PasswordValidatorTest
{
  [Fact]
  public void Validator_IdentityOptions_Password_RequiredLength()
  {
    var options = new IdentityOptions
    {
      Password = new PasswordOptions
      {
        RequiredLength = 20,
        // off
        RequireDigit = false,
        RequireLowercase = false,
        RequiredUniqueChars = 0,
        RequireUppercase = false,
        RequireNonAlphanumeric = false,
      }
    };
    
    var validator = new PasswordValidator(Options.Create(options));

    var dto = new UserCreateDto
    {
      UserName = "TestedUser",
      Email = "random@email.com",
      Password = "noRequiredLength",
    };

    var result = validator.TestValidate(dto);
    result.ShouldHaveValidationErrorFor(e => e.Password);
  }
  
  [Fact]
  public void Validator_IdentityOptions_Password_RequireNonAlphanumeric()
  {
    var options = new IdentityOptions
    {
      Password = new PasswordOptions
      {
        RequireNonAlphanumeric = true,
        // off
        RequireUppercase = false,
        RequiredLength = 0,
        RequireDigit = false,
        RequireLowercase = false,
        RequiredUniqueChars = 0,
      }
    };
    
    var validator = new PasswordValidator(Options.Create(options));

    var dto = new UserCreateDto
    {
      UserName = "TestedUser",
      Email = "random@email.com",
      Password = "alpha123",
    };


    var result = validator.TestValidate(dto);
    result.ShouldHaveValidationErrorFor(e => e.Password);
  }
  
  
  [Fact]
  public void Validator_IdentityOptions_Password_RequireDigit()
  {
    var options = new IdentityOptions
    {
      Password = new PasswordOptions
      {
        RequireDigit = true,
        // off
        RequiredLength = 0,
        RequireUppercase = false,
        RequireLowercase = false,
        RequiredUniqueChars = 0,
        RequireNonAlphanumeric = false,
      }
    };
    
    var validator = new PasswordValidator(Options.Create(options));

    var dto = new UserCreateDto
    {
      UserName = "TestedUser",
      Email = "random@email.com",
      Password = "nodigit",
    };


    var result = validator.TestValidate(dto);
    result.ShouldHaveValidationErrorFor(e => e.Password);
  }
  
  [Fact]
  public void Validator_IdentityOptions_Password_RequireLowercase()
  {
    var options = new IdentityOptions
    {
      Password = new PasswordOptions
      {
        RequireLowercase = true,
        // off
        RequiredLength = 0,
        RequireUppercase = false,
        RequireDigit = false,
        RequiredUniqueChars = 0,
        RequireNonAlphanumeric = false,
      }
    };
    
    var validator = new PasswordValidator(Options.Create(options));

    var dto = new UserCreateDto
    {
      UserName = "TestedUser",
      Email = "random@email.com",
      Password = "UPPERCASE",
    };


    var result = validator.TestValidate(dto);
    result.ShouldHaveValidationErrorFor(e => e.Password);
  }
  
  [Fact]
  public void Validator_IdentityOptions_Password_RequireUppercase()
  {
    var options = new IdentityOptions
    {
      Password = new PasswordOptions
      {
        RequireUppercase = true,
        // off
        RequiredLength = 0,
        RequireDigit = false,
        RequireLowercase = false,
        RequiredUniqueChars = 0,
        RequireNonAlphanumeric = false,
      }
    };
    
    var validator = new PasswordValidator(Options.Create(options));

    var dto = new UserCreateDto
    {
      UserName = "TestedUser",
      Email = "random@email.com",
      Password = "lowercase",
    };


    var result = validator.TestValidate(dto);
    result.ShouldHaveValidationErrorFor(e => e.Password);
  }
  
  [Fact]
  public void Validator_IdentityOptions_Password_RequiredUniqueChars()
  {
    var options = new IdentityOptions
    {
      Password = new PasswordOptions
      {
        RequiredUniqueChars = 2,
        // off
        RequiredLength = 0,
        RequireUppercase = false,
        RequireDigit = false,
        RequireLowercase = false,
        RequireNonAlphanumeric = false,
      }
    };
    
    var validator = new PasswordValidator(Options.Create(options));

    var dto = new UserCreateDto
    {
      UserName = "TestedUser",
      Email = "random@email.com",
      Password = "aaaaaaaaaaaaaaa", // one unique character
    };


    var result = validator.TestValidate(dto);
    result.ShouldHaveValidationErrorFor(e => e.Password);
    
  }
  
}