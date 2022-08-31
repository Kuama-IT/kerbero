namespace KerberoWebApi.Models.Rent;
// RentProvider contains the information of a Rent provider service
public abstract class RentProvider
{
  public string Name { get; }
  public string? Logo { get; set;}
  public RentProvider(string name) { Name = name;}
}