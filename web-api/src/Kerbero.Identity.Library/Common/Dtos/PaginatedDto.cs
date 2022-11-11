namespace Kerbero.Identity.Library.Common.Dtos;

public class PaginatedDto<T>
{
  public List<T> Items { get; set; } = null!;
  public int TotalItems { get; set; }
}
