using Kerbero.Identity.Modules.Claims.Utils;
using Kerbero.Identity.Modules.Roles.Services;
using Kerbero.Identity.Modules.Users.Services;
using Kerbero.Identity.Library.Common.Dtos;
using Kerbero.Identity.Library.Modules.Claims.Dtos;
using Kerbero.Identity.Library.Modules.Roles.Dtos;
using Kerbero.Identity.Library.Modules.Users.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kerbero.Identity.Modules.Users.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
  private readonly IUserService _userService;
  private readonly IRoleService _roleService;

  public UsersController(IUserService userService, IRoleService roleService)
  {
    _userService = userService;
    _roleService = roleService;
  }

  [HttpGet]
  [Authorize(Policy = PoliciesDefinition.Users_GetAll)]
  public async Task<ActionResult<List<UserReadDto>>> GetAll()
  {
    return await _userService.GetAll();
  }

  [HttpPost("paginate")]
  [Authorize(Policy = PoliciesDefinition.Users_GetAll)]
  public async Task<ActionResult<PaginatedDto<UserReadDto>>> GetPaginated(PaginateDto paginateDto)
  {
    return await _userService.GetPaginated(paginateDto);
  }

  [HttpGet("{id:guid}")]
  [Authorize(Policy = PoliciesDefinition.Users_GetById)]
  public async Task<ActionResult<UserReadDto>> GetById(Guid id)
  {
    return await _userService.GetById(id);
  }

  [HttpGet("{id:guid}/roles")]
  [Authorize(Policy = PoliciesDefinition.Users_GetById)]
  public async Task<ActionResult<List<RoleReadDto>>> GetRolesById(Guid id)
  {
    return await _roleService.GetByUserId(id);
  }

  [HttpPost]
  [Authorize(Policy = PoliciesDefinition.Users_Create)]
  public async Task<ActionResult<UserReadDto>> Create(UserCreateDto createDto)
  {
    return await _userService.Create(createDto);
  }

  [HttpGet("confirm-email")]
  [AllowAnonymous]
  public async Task<ActionResult> ConfirmEmailByUserIdAndCode(Guid userId, string code)
  {
    await _userService.ConfirmEmailAsync(userId, code);
    return new OkResult();
  }


#if DEBUG

  [HttpPost("admin")]
  [AllowAnonymous]
  public async Task<ActionResult<UserReadDto>> CreateAdmin(UserCreateDto createDto)
  {
    return await _userService.CreateAdmin(createDto);
  }

#endif

  [HttpPut("{id:guid}")]
  [Authorize(Policy = PoliciesDefinition.Users_Update)]
  public async Task<ActionResult<UserReadDto>> Update(Guid id, UserUpdateDto updateDto)
  {
    return await _userService.Update(id, updateDto);
  }


  [HttpDelete("{id:guid}")]
  [Authorize(Policy = PoliciesDefinition.Users_Delete)]
  public async Task<ActionResult<UserReadDto>> Delete(Guid id)
  {
    return await _userService.Delete(id);
  }

  [HttpPut("{id:guid}/roles")]
  [Authorize(Policy = PoliciesDefinition.Users_SetRoles)]
  public async Task<ActionResult> SetRoles(Guid id, UserSetRolesDto setRolesDto)
  {
    await _userService.SetRoles(id, setRolesDto);
    return Ok();
  }

  [HttpGet("{id:guid}/claims")]
  [Authorize(Policy = PoliciesDefinition.Users_GetClaimsById)]
  public async Task<ActionResult<List<ClaimReadDto>>> GetClaims(Guid id)
  {
    var claims = await _userService.GetClaimsById(id);
    return claims;
  }

  [HttpPut("{id:guid}/claims")]
  [Authorize(Policy = PoliciesDefinition.Users_SetClaims)]
  public async Task<ActionResult<List<ClaimReadDto>>> SetClaims(Guid id, UserSetClaimsDto setClaimsDto)
  {
    await _userService.SetClaims(id, setClaimsDto);
    return Ok();
  }
}