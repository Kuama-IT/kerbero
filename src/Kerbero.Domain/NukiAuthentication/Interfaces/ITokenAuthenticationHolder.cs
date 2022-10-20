namespace Kerbero.Domain.NukiAuthentication.Interfaces;

public interface ITokenAuthenticationHolder
{
    public string? Token { get; set; }
}