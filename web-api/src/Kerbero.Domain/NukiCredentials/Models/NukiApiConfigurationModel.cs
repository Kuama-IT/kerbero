namespace Kerbero.Domain.NukiCredentials.Models;

/// <summary>
/// Represents information about nuki params configuration
/// </summary>
/// <param name="apiEndpoint">Complete uri for the base nuki api</param>
/// <param name="clientId">Client id of our app registered inside nuki web account</param>
/// <param name="scopes">Which scopes should be requested for the user nuki account</param>
/// <param name="applicationRedirectEndpoint">The endpoint that will handle the user redirection after he grants us the requested scopes</param>
/// <param name="applicationDomain">The domain where the Kerbero application is served</param>
public record NukiApiConfigurationModel(
  string apiEndpoint,
  string clientId,
  string scopes,
  string applicationRedirectEndpoint,
  string applicationDomain
);