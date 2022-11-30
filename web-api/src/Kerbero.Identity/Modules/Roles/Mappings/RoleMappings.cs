using System.Diagnostics;
using Kerbero.Identity.Common.Models;
using Kerbero.Identity.Modules.Roles.Entities;
using Kerbero.Identity.Library.Common.Dtos;
using Kerbero.Identity.Library.Modules.Roles.Dtos;

namespace Kerbero.Identity.Modules.Roles.Mappings;

public static class RoleMappings
{
  public static List<RoleReadDto> Map(List<Role> entities)
  {
    return entities.ConvertAll(Map);
  }

  public static PaginatedDto<RoleReadDto> Map(PaginatedModel<Role> paginatedEntities)
  {
    return new PaginatedDto<RoleReadDto>
    {
      Items = Map(paginatedEntities.Items),
      TotalItems = paginatedEntities.TotalItems,
    };
  }

  public static RoleReadDto Map(Role entity)
  {
    return new RoleReadDto
    {
      Id = entity.Id,
      Name = entity.Name ??
             throw new UnreachableException(
               $"unable to map {nameof(Role)} the property {nameof(entity.Name)} is null"),
    };
  }

  public static Role Map(RoleCreateDto createDto)
  {
    return new Role
    {
      Name = createDto.Name,
    };
  }

  public static void Patch(RoleUpdateDto updateDto, Role entity)
  {
    entity.Name = updateDto.Name;
  }
}