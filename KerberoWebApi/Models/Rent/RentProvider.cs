namespace KerberoWebApi.Models
{
    public abstract class RentProvider
    {
        public string Name { get; }
        public string? Logo { get; set;}
        public RentProvider(string name) { Name = name;}
    }
}