using Kerbero.Identity.Modules.Claims.Utils;
using Kerbero.Identity.Modules.Roles.Services;
using Kerbero.Identity.Library.Common.Dtos;
using Kerbero.Identity.Library.Modules.Claims.Dtos;
using Kerbero.Identity.Library.Modules.Roles.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kerbero.Identity.Modules.Roles.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RolesController : ControllerBase
{
  private readonly IRoleService _roleService;

  public RolesController(IRoleService roleService)
  {
    _roleService = roleService;
  }

  [HttpGet]
  [Authorize(Policy = PoliciesDefinition.Roles_GetAll)]
  public async Task<ActionResult<List<RoleReadDto>>> GetAll()
  {
    var dtos = await _roleService.GetAll();
    return dtos;
  }

  [HttpPost("paginate")]
  [Authorize(Policy = PoliciesDefinition.Roles_GetAll)]
  public async Task<ActionResult<PaginatedDto<RoleReadDto>>> GetPaginated(PaginateDto paginateDto)
  {
    return await _roleService.GetPaginated(paginateDto);
  }

  [HttpGet("{id:guid}")]
  [Authorize(Policy = PoliciesDefinition.Roles_GetById)]
  public async Task<ActionResult<RoleReadDto>> GetById(Guid id)
  {
    var dto = await _roleService.GetById(id);
    return dto;
  }

  [HttpGet("{id:guid}/claims")]
  [Authorize(Policy = PoliciesDefinition.Roles_GetClaimsById)]
  public async Task<ActionResult<List<ClaimReadDto>>> GetClaimsById(Guid id)
  {
    var dto = await _roleService.GetClaimsById(id);
    return dto;
  }

  [HttpPost]
  [Authorize(Policy = PoliciesDefinition.Roles_Create)]
  public async Task<ActionResult<RoleReadDto>> Create(RoleCreateDto createDto)
  {
    var dto = await _roleService.Create(createDto);
    return dto;
  }

  [HttpPut("{id:guid}")]
  [Authorize(Policy = PoliciesDefinition.Roles_Update)]
  public async Task<ActionResult<RoleReadDto>> Update(Guid id, RoleUpdateDto updateDto)
  {
    var dto = await _roleService.Update(id, updateDto);
    return dto;
  }

  [HttpDelete("{id:guid}")]
  [Authorize(Policy = PoliciesDefinition.Roles_Delete)]
  public async Task<ActionResult<RoleReadDto>> Delete(Guid id)
  {
    var dto = await _roleService.Delete(id);
    return dto;
  }


  [HttpPut("{id:guid}/claims")]
  [Authorize(Policy = PoliciesDefinition.Roles_SetClaims)]
  public async Task<ActionResult> SetClaims(Guid id, RoleSetClaimsDto setClaimsDto)
  {
    await _roleService.SetClaims(id, setClaimsDto);
    return Ok();
  }
}