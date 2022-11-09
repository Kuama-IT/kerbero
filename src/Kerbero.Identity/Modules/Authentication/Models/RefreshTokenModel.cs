namespace Kerbero.Identity.Modules.Authentication.Models;

public record RefreshTokenModel(string Token, DateTime ExpireDate);