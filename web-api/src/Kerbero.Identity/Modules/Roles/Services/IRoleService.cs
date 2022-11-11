using Kerbero.Identity.Library.Common.Dtos;
using Kerbero.Identity.Library.Modules.Claims.Dtos;
using Kerbero.Identity.Library.Modules.Roles.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Kerbero.Identity.Modules.Roles.Services;

public interface IRoleService
{
  Task<List<RoleReadDto>> GetAll();
  Task<PaginatedDto<RoleReadDto>> GetPaginated(PaginateDto paginateDto);
  Task<RoleReadDto> GetById(Guid id);
  Task<List<RoleReadDto>> GetByUserId(Guid userId);
  Task<List<ClaimReadDto>> GetClaimsById(Guid id);
  Task<RoleReadDto> Create(RoleCreateDto createDto);
  Task<RoleReadDto> Update(Guid id, RoleUpdateDto updateDto);
  Task<RoleReadDto> Delete(Guid id);
  Task SetClaims(Guid id, RoleSetClaimsDto setClaimsDto);
}