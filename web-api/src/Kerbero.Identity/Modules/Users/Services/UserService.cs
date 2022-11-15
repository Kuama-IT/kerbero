using System.Text;
using System.Text.Encodings.Web;
using FluentValidation;
using FluentValidation.Results;
using Kerbero.Identity.Common.Exceptions;
using Kerbero.Identity.Common.Mappings;
using Kerbero.Identity.Common.Utils;
using Kerbero.Identity.Modules.Claims.Mappings;
using Kerbero.Identity.Modules.Claims.Utils;
using Kerbero.Identity.Modules.Notifier.Events;
using Kerbero.Identity.Modules.Notifier.Services;
using Kerbero.Identity.Modules.Roles.Entities;
using Kerbero.Identity.Modules.Roles.Services;
using Kerbero.Identity.Modules.Users.Mappings;
using Kerbero.Identity.Library.Common.Dtos;
using Kerbero.Identity.Library.Modules.Claims.Dtos;
using Kerbero.Identity.Library.Modules.Users.Dtos;
using Kerbero.Identity.Modules.Email.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;

namespace Kerbero.Identity.Modules.Users.Services;

public class UserService : IUserService
{
  private readonly IKerberoIdentityNotifier _kerberoIdentityNotifier;

  private readonly IUserManager _userManager;
  private readonly IRoleManager _roleManager;

  private readonly IValidator<UserCreateDto> _userCreateDtoValidator;
  private readonly IValidator<UserUpdateDto> _userUpdateDtoValidator;
  private readonly IEmailSenderService _emailSenderService;

  public UserService(
    IKerberoIdentityNotifier kerberoIdentityNotifier,
    IUserManager userManager,
    IRoleManager roleManager,
    IValidator<UserCreateDto> userCreateDtoValidator,
    IValidator<UserUpdateDto> userUpdateDtoValidator,
    IEmailSenderService emailSenderService
  )
  {
    _kerberoIdentityNotifier = kerberoIdentityNotifier;
    _userManager = userManager;
    _roleManager = roleManager;
    _userCreateDtoValidator = userCreateDtoValidator;
    _userUpdateDtoValidator = userUpdateDtoValidator;
    _emailSenderService = emailSenderService;
  }

  public async Task<List<UserReadDto>> GetAll()
  {
    var users = await _userManager.GetAll();
    return UserMappings.Map(users);
  }

  public async Task<PaginatedDto<UserReadDto>> GetPaginated(PaginateDto paginateDto)
  {
    var paginate = PaginateMappings.Map(paginateDto);
    var paginated = await _userManager.GetPaginated(paginate);
    return UserMappings.Map(paginated);
  }

  public async Task<UserReadDto> GetById(Guid id)
  {
    try
    {
      var user = await _userManager.GetById(id);
      return UserMappings.Map(user);
    }
    catch (InvalidOperationException)
    {
      throw new NotFoundException();
    }
  }

  public async Task<List<ClaimReadDto>> GetClaimsById(Guid id)
  {
    try
    {
      var user = await _userManager.GetById(id);
      var claims = await _userManager.GetClaimsByUser(user);
      return ClaimMappings.Map(claims);
    }
    catch (InvalidOperationException)
    {
      throw new NotFoundException();
    }
  }

  public async Task<UserReadDto> Create(UserCreateDto createDto, HostString serviceDomain)
  {
    _userCreateDtoValidator.ValidateAndThrow(createDto);

    var user = UserMappings.Map(createDto);

    var result = await _userManager.Create(user, createDto.Password);
    HandleCreateResult(result);

    _kerberoIdentityNotifier.EmitUserCreateEvent(new UserCreateEvent(user.Id));
    
    var userId = await _userManager.GetUserIdAsync(user);
    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

    var callbackUrl = new UriBuilder
    {
      Host = serviceDomain.Host,
      Scheme = "https",
      Path = "/api/users/confirm",
      Query = $"userId={userId}&code={code}" 
    };
    
    #if DEBUG
    if (serviceDomain.Port is not null)
      callbackUrl.Port = (int)serviceDomain.Port;
    #endif
    
    await _emailSenderService.SendEmailAsSystem(createDto.Email, "Confirm your email",
      $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl.ToString())}'>clicking here</a>.");

    return UserMappings.Map(user);
  }

  public async Task ConfirmEmailAsync(Guid userId, string code)
  {
    var user = await _userManager.GetById(userId);
    if (user == null)
    {
      throw new UnauthorizedException();
    }
    
    code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));

    var result = await _userManager.ConfirmEmailAsync(user, code);
    if (!result.Succeeded)
    {
      throw new ApplicationException($"Error confirming email for user with ID '{userId}':");
    }
  }

  private void HandleCreateResult(IdentityResult result)
  {
    if (result.Succeeded) return;

    var failures = new List<ValidationFailure>();

    var invalidEmailError = IdentityErrorUtils.FindInvalidEmail(result.Errors);
    if (invalidEmailError is not null)
    {
      failures.Add(new ValidationFailure(nameof(UserCreateDto.Email), invalidEmailError.Description));
    }
    
    var duplicateEmailError = IdentityErrorUtils.FindDuplicateEmail(result.Errors);
    if (duplicateEmailError is not null)
    {
      failures.Add(new ValidationFailure(nameof(UserCreateDto.Email), duplicateEmailError.Description));
    }
    
    var invalidUserNameError = IdentityErrorUtils.FindInvalidUserName(result.Errors);
    if (invalidUserNameError is not null)
    {
      failures.Add(new ValidationFailure(nameof(UserCreateDto.UserName), invalidUserNameError.Description));
    }
    
    var duplicateUserNameError = IdentityErrorUtils.FindDuplicateUserName(result.Errors);
    if (duplicateUserNameError is not null)
    {
      failures.Add(new ValidationFailure(nameof(UserCreateDto.UserName), duplicateUserNameError.Description));
    }


    if (failures.Any())
    {
      throw new ValidationException(failures);
    }

    // fallback
    throw new BadRequestException();
  }

  public async Task<UserReadDto> Update(Guid id, UserUpdateDto updateDto)
  {
    _userUpdateDtoValidator.ValidateAndThrow(updateDto);

    try
    {
      var user = await _userManager.GetById(id);

      UserMappings.Patch(updateDto, user);

      var result = await _userManager.Update(user);
      HandleUpdateResult(result);

      _kerberoIdentityNotifier.EmitUserUpdateEvent(new UserUpdateEvent(user.Id));

      return UserMappings.Map(user);
    }
    catch (InvalidOperationException)
    {
      throw new NotFoundException();
    }
  }

  public void HandleUpdateResult(IdentityResult result)
  {
    if (result.Succeeded) return;

    var failures = new List<ValidationFailure>();
    
    var invalidEmailError = IdentityErrorUtils.FindInvalidEmail(result.Errors);
    if (invalidEmailError is not null)
    {
      failures.Add(new ValidationFailure(nameof(UserUpdateDto.Email), invalidEmailError.Description));
    }
    
    var duplicateEmailError = IdentityErrorUtils.FindDuplicateEmail(result.Errors);
    if (duplicateEmailError is not null)
    {
      failures.Add(new ValidationFailure(nameof(UserUpdateDto.Email), duplicateEmailError.Description));
    }
    
    var invalidUserNameError = IdentityErrorUtils.FindInvalidUserName(result.Errors);
    if (invalidUserNameError is not null)
    {
      failures.Add(new ValidationFailure(nameof(UserUpdateDto.UserName), invalidUserNameError.Description));
    }
    
    var duplicateUserNameError = IdentityErrorUtils.FindDuplicateUserName(result.Errors);
    if (duplicateUserNameError is not null)
    {
      failures.Add(new ValidationFailure(nameof(UserUpdateDto.UserName), duplicateUserNameError.Description));
    }
    
    if (failures.Any())
    {
      throw new ValidationException(failures);
    }

    // fallback
    throw new BadRequestException();
  }

  public async Task<UserReadDto> Delete(Guid id)
  {
    try
    {
      var user = await _userManager.GetById(id);

      var result = await _userManager.Delete(user);
      if (!result.Succeeded)
      {
        throw new BadRequestException();
      }

      _kerberoIdentityNotifier.EmitUserDeleteEvent(new UserDeleteEvent(user.Id));

      return UserMappings.Map(user);
    }
    catch (InvalidOperationException)
    {
      throw new NotFoundException();
    }
  }

  public async Task SetRoles(Guid id, UserSetRolesDto setRolesDto)
  {
    try
    {
      var user = await _userManager.GetById(id);

      var rolesNamesToRemove = await _userManager.GetRolesNamesByUser(user);

      var resultRemove = await _userManager.RemoveRolesByNamesFromUser(user, rolesNamesToRemove);
      if (!resultRemove.Succeeded)
      {
        // should handle failure errors
        throw new BadRequestException();
      }

      var resultAdd = await _userManager.AddRolesByNamesToUser(user, setRolesDto.RoleNames);
      if (!resultAdd.Succeeded)
      {
        // should handle failure errors
        throw new BadRequestException();
      }
    }
    catch (InvalidOperationException)
    {
      throw new NotFoundException();
    }
  }

  public async Task SetClaims(Guid id, UserSetClaimsDto setClaimsDto)
  {
    try
    {
      var user = await _userManager.GetById(id);

      var claimsToRemove = await _userManager.GetClaimsByUser(user);
      var claimsRemoveResult = await _userManager.RemoveClaimsToUser(user, claimsToRemove);
      if (!claimsRemoveResult.Succeeded)
      {
        throw new BadRequestException();
      }

      var claimsToAdd = ClaimMappings.Map(setClaimsDto.Claims);
      var claimsAddResult = await _userManager.AddClaimsToUser(user, claimsToAdd);
      if (!claimsAddResult.Succeeded)
      {
        throw new BadRequestException();
      }
    }
    catch (InvalidOperationException)
    {
      throw new NotFoundException();
    }
  }

  public async Task<UserReadDto> CreateAdmin(UserCreateDto createDto)
  {
    var user = UserMappings.Map(createDto);
    var userCreateResult = await _userManager.Create(user, createDto.Password);
    if (!userCreateResult.Succeeded)
    {
      throw new BadRequestException();
    }

    const string adminRoleName = "Admin";
    var role = await _roleManager.FindByNameAsync(adminRoleName);
    if (role is null)
    {
      role = new Role { Name = adminRoleName };
      var roleCreateResult = await _roleManager.Create(role);
      if (!roleCreateResult.Succeeded)
      {
        throw new BadRequestException();
      }

      foreach (var claim in ClaimsDefinition.KuIdentityClaims)
      {
        var roleAddClaimResult = await _roleManager.AddClaimToRole(role, claim);
        if (!roleAddClaimResult.Succeeded)
        {
          throw new BadRequestException();
        }
      }
    }

    var userAddRoleResult = await _userManager.AddRoleByNameToUser(user, role.Name);
    if (!userAddRoleResult.Succeeded)
    {
      throw new BadRequestException();
    }

    return UserMappings.Map(user);
  }
}
