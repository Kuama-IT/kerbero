namespace KerberoWebApi.Models
{
    public class RentProviderAccount: RentProvider
    {
        public string Token {get; set;}
        public string RefreshToken {get; set;}
        public string ClientId {get; set;}
        public string? ClientSecret {get; set;}
        public string? ApiKey {get; set;}
        public RentProviderAccount(string name, string token, string refreshToken, string clientId): base(name) { Token = token; RefreshToken = refreshToken; ClientId = clientId; }
    }
}