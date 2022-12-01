using System.Text.Encodings.Web;
using System.Web;

namespace Kerbero.Domain.NukiCredentials.Models;

/// <summary>
/// Represents a nuki credential that will be confirmed later on after a Nuki Authentication process
/// </summary>
public record NukiCredentialDraftModel(string RedirectUrl, Guid UserId, int? Id = null)
{
  /// <summary>
  /// Builds a correct URI to redirect our users to the Nuki Web platform in order to initiate a Nuki Authentication flow
  /// </summary>
  /// <param name="model"></param>
  /// <param name="identifier">Optional, if not provided it will be a Guid.NewGuid()</param>
  /// <returns></returns>
  public static Uri GetOAuthRedirectUri(NukiApiConfigurationModel model, Guid? identifier = null)
  {
    var baseUri = $"{model.apiEndpoint}/oauth/authorize";
    var applicationRedirectUri = $"{model.applicationDomain}/{model.applicationRedirectEndpoint}/{identifier ?? Guid.NewGuid()}";
    
    var queryParams = new List<string>()
    {
      "response_type=code",
      $"client_id={model.clientId}",
      $"scope={HttpUtility.UrlEncode(model.scopes)}",
      $"redirect_uri={HttpUtility.UrlEncode(applicationRedirectUri)}",
    };

    return new UriBuilder(baseUri) { Query = string.Join("&", queryParams) }.Uri;
  }
}