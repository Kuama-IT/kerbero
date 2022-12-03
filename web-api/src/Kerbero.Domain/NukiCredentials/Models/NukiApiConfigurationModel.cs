namespace Kerbero.Domain.NukiCredentials.Models;

/// <summary>
/// Represents information about nuki params configuration
/// </summary>
/// <param name="ApiEndpoint">Complete uri for the base nuki api</param>
/// <param name="ClientId">Client id of our app registered inside nuki web account</param>
/// <param name="Scopes">Which scopes should be requested for the user nuki account</param>
/// <param name="ApplicationRedirectEndpoint">The endpoint that will handle the user redirection after he grants us the requested scopes</param>
/// <param name="ApplicationDomain">The domain where the Kerbero application is served</param>
public record NukiApiConfigurationModel(
  string ApiEndpoint,
  string ClientId,
  string Scopes,
  string ApplicationRedirectEndpoint,
  string ApplicationDomain
);