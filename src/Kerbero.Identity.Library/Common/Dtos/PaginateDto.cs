namespace Kerbero.Identity.Library.Common.Dtos;

public class PaginateDto
{
  public int Page { get; set; }
  public int PageSize { get; set; }

  public const int DefaultPage = 1;
  public const int DefaultPageSize = 10;
}