using System.Web;
using FluentResults;
using Kerbero.Domain.Common.Repositories;
using Kerbero.Domain.NukiCredentials.Interfaces;
using Kerbero.Domain.NukiCredentials.Utils;

namespace Kerbero.Domain.NukiCredentials.Interactors;

/// <summary>
/// Builds a correct URI to redirect our users to the Nuki Web platform in order to initiate a Nuki Authentication flow
/// </summary>
public class BuildNukiRedirectUriInteractor : IBuildNukiRedirectUriInteractor
{
  private readonly IKerberoConfigurationRepository _kerberoConfigurationRepository;

  public BuildNukiRedirectUriInteractor(IKerberoConfigurationRepository kerberoConfigurationRepository)
  {
    _kerberoConfigurationRepository = kerberoConfigurationRepository;
  }

  public async Task<Result<Uri>> Handle()
  {
    var nukiApiDefinitionResult = await _kerberoConfigurationRepository.GetNukiApiDefinition();

    if (nukiApiDefinitionResult.IsFailed)
    {
      return Result.Fail(nukiApiDefinitionResult.Errors);
    }

    var nukiApiDefinition = nukiApiDefinitionResult.Value;

    var baseUri = $"{nukiApiDefinition.ApiEndpoint}/oauth/authorize";
    var applicationRedirectUri = BuildRedirectToKerberoUriHelper.Handle(nukiApiDefinition);

    var queryParams = new List<string>()
    {
      "response_type=code",
      $"client_id={nukiApiDefinition.ClientId}",
      $"scope={HttpUtility.UrlEncode(nukiApiDefinition.Scopes)}",
      $"redirect_uri={HttpUtility.UrlEncode(applicationRedirectUri)}",
    };

    return new UriBuilder(baseUri) { Query = string.Join("&", queryParams) }.Uri;
  }
}
