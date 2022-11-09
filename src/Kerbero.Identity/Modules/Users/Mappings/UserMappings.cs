using Kerbero.Identity.Common.Models;
using Kerbero.Identity.Modules.Users.Entities;
using Kerbero.Identity.Library.Common.Dtos;
using Kerbero.Identity.Library.Modules.Users.Dtos;

namespace Kerbero.Identity.Modules.Users.Mappings;

public static class UserMappings
{
  public static List<UserReadDto> Map(List<User> entities)
  {
    return entities.ConvertAll(Map);
  }

  public static UserReadDto Map(User entity)
  {
    return new UserReadDto
    {
      Id = entity.Id,
      UserName = entity.UserName,
      Email = entity.Email,
      EmailConfirmed = entity.EmailConfirmed
    };
  }

  public static User Map(UserCreateDto createDto)
  {
    return new User
    {
      UserName = createDto.UserName,
      Email = createDto.Email,
    };
  }
  
  public static void Patch(UserUpdateDto updateDto, User entity)
  {
    entity.Email = updateDto.Email;
    entity.UserName = updateDto.UserName;
  }


  public static PaginatedDto<UserReadDto> Map(PaginatedModel<User> paginatedEntities)
  {
    return new PaginatedDto<UserReadDto>
    {
      Items =  Map(paginatedEntities.Items),
      TotalItems = paginatedEntities.TotalItems,
    };
  }
}