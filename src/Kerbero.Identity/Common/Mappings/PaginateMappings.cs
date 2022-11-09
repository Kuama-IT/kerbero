using Kerbero.Identity.Common.Models;
using Kerbero.Identity.Library.Common.Dtos;

namespace Kerbero.Identity.Common.Mappings;

public static class PaginateMappings
{
  public static PaginateModel Map(PaginateDto paginateDto)
  {
    return new PaginateModel(paginateDto.Page, paginateDto.PageSize);
  }
}

