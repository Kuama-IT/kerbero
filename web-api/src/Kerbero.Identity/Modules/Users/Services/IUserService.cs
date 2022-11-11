using Kerbero.Identity.Library.Common.Dtos;
using Kerbero.Identity.Library.Modules.Authentication.Dtos;
using Kerbero.Identity.Library.Modules.Claims.Dtos;
using Kerbero.Identity.Library.Modules.Users.Dtos;

namespace Kerbero.Identity.Modules.Users.Services;

public interface IUserService
{
  Task<List<UserReadDto>> GetAll();
  Task<PaginatedDto<UserReadDto>> GetPaginated(PaginateDto paginateDto);

  Task<UserReadDto> GetById(Guid id);
  Task<List<ClaimReadDto>> GetClaimsById(Guid id);

  Task<UserReadDto> Create(UserCreateDto createDto);
  Task<UserReadDto> CreateAdmin(UserCreateDto createDto);
  Task<UserReadDto> Update(Guid id, UserUpdateDto updateDto);
  Task<UserReadDto> Delete(Guid id);
  Task SetRoles(Guid id, UserSetRolesDto roleNames);

  Task SetClaims(Guid id, UserSetClaimsDto setClaimsDto);
}