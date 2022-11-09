namespace Kerbero.Identity.Common.Models;

public record PaginatedModel<T>(List<T> Items, int TotalItems);