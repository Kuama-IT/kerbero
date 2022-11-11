namespace Kerbero.Identity.Common.Models;

public record PaginateModel(int Page, int PageSize)
{
  public int Take => (Page - 1) * PageSize + PageSize;
  public int Skip => (Page - 1) * PageSize;
}