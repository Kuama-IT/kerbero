using FluentValidation;
using FluentValidation.Results;
using Kerbero.Identity.Common.Exceptions;
using Kerbero.Identity.Common.Mappings;
using Kerbero.Identity.Common.Utils;
using Kerbero.Identity.Modules.Claims.Mappings;
using Kerbero.Identity.Modules.Notifier.Events;
using Kerbero.Identity.Modules.Notifier.Services;
using Kerbero.Identity.Modules.Roles.Mappings;
using Kerbero.Identity.Modules.Users.Services;
using Kerbero.Identity.Extensions;
using Kerbero.Identity.Modules.Roles.Entities;
using Kerbero.Identity.Modules.Users.Entities;
using Kerbero.Identity.Modules.Users.Mappings;
using Kerbero.Identity.Library.Common.Dtos;
using Kerbero.Identity.Library.Modules.Claims.Dtos;
using Kerbero.Identity.Library.Modules.Roles.Dtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Kerbero.Identity.Modules.Roles.Services;

public class RoleService : IRoleService
{
  private readonly IRoleManager _roleManager;
  private readonly IUserManager _userManager;
  private readonly IKerberoIdentityNotifier _kerberoIdentityNotifier;


  public RoleService(
    IRoleManager roleManager,
    IUserManager userManager,
    IKerberoIdentityNotifier kerberoIdentityNotifier
  )
  {
    _roleManager = roleManager;
    _userManager = userManager;
    _kerberoIdentityNotifier = kerberoIdentityNotifier;
  }

  public async Task<List<RoleReadDto>> GetAll()
  {
    var entities = await _roleManager.GetAll();
    return RoleMappings.Map(entities);
  }

  public async Task<PaginatedDto<RoleReadDto>> GetPaginated(PaginateDto paginateDto)
  {
    var paginate = PaginateMappings.Map(paginateDto);
    var paginated = await _roleManager.GetPaginated(paginate);
    return RoleMappings.Map(paginated);
  }

  public async Task<RoleReadDto> GetById(Guid id)
  {
    try
    {
      var entity = await _roleManager.GetById(id);
      return RoleMappings.Map(entity);
    }
    catch (InvalidOperationException)
    {
      throw new NotFoundException();
    }
  }

  public async Task<List<RoleReadDto>> GetByUserId(Guid userId)
  {
    try
    {
      var user = await _userManager.GetById(userId);

      var rolesNames = await _userManager.GetRolesNamesByUser(user);
      var roles = await _roleManager.GetByNames(rolesNames);

      return RoleMappings.Map(roles);
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
      var role = await _roleManager.GetById(id);
      var claims = await _roleManager.GetClaimsByRole(role);

      return ClaimMappings.Map(claims);
    }
    catch (InvalidOperationException)
    {
      throw new NotFoundException();
    }
  }

  public async Task<RoleReadDto> Create(RoleCreateDto createDto)
  {
    var entity = RoleMappings.Map(createDto);

    var result = await _roleManager.Create(entity);
    if (!result.Succeeded)
    {
      _ThrowKnownErrorsAsValidationException(result.Errors);

      // should handle failure errors
      throw new BadRequestException();
    }

    _kerberoIdentityNotifier.EmitRoleCreateEvent(new RoleCreateEvent(entity.Id));

    return RoleMappings.Map(entity);
  }

  public async Task<RoleReadDto> Update(Guid id, RoleUpdateDto updateDto)
  {
    try
    {
      var entity = await _roleManager.GetById(id);

      RoleMappings.Patch(updateDto, entity);

      var result = await _roleManager.Update(entity);
      if (!result.Succeeded)
      {
        _ThrowKnownErrorsAsValidationException(result.Errors);
        
        // should handle failure errors
        throw new BadRequestException();
      }

      _kerberoIdentityNotifier.EmitRoleUpdateEvent(new RoleUpdateEvent(entity.Id));

      return RoleMappings.Map(entity);
    }
    catch (InvalidOperationException)
    {
      throw new NotFoundException();
    }
  }

  public async Task<RoleReadDto> Delete(Guid id)
  {
    try
    {
      var entity = await _roleManager.GetById(id);

      var result = await _roleManager.Delete(entity);
      if (!result.Succeeded)
      {
        // should handle failure errors
        throw new BadRequestException();
      }

      _kerberoIdentityNotifier.EmitRoleDeleteEvent(new RoleDeleteEvent(entity.Id));

      return RoleMappings.Map(entity);
    }
    catch (InvalidOperationException)
    {
      throw new NotFoundException();
    }
  }

  public async Task SetClaims(Guid id, RoleSetClaimsDto setClaimsDto)
  {
    try
    {
      var role = await _roleManager.GetById(id);

      // remove all (clear)
      var roleClaims = await _roleManager.GetClaimsByRole(role);
      foreach (var roleClaim in roleClaims)
      {
        var result = await _roleManager.RemoveClaimFromRole(role, roleClaim);
        if (!result.Succeeded)
        {
          // should handle failure errors
          throw new BadRequestException();
        }
      }

      // add all
      var claims = ClaimMappings.Map(setClaimsDto.Claims);
      foreach (var claim in claims)
      {
        var result = await _roleManager.AddClaimToRole(role, claim);
        if (!result.Succeeded)
        {
          // should handle failure errors
          throw new BadRequestException();
        }
      }
    }
    catch (InvalidOperationException)
    {
      throw new NotFoundException();
    }
  }
  
  // use for create and update, not sure if we should do it before in another class (like inspectors)
  private void _ThrowKnownErrorsAsValidationException(IEnumerable<IdentityError> errors)
  {
    var validationResults = new List<ValidationFailure>();

    var duplicateRoleNameError = IdentityErrorUtils.FindDuplicateRoleName(errors);
    if (duplicateRoleNameError is not null)
    {
      validationResults.Add(new ValidationFailure(nameof(RoleUpdateDto.Name), duplicateRoleNameError.Description));
    }

    if (validationResults.Any())
    {
      throw new ValidationException(validationResults);
    }
  }
}