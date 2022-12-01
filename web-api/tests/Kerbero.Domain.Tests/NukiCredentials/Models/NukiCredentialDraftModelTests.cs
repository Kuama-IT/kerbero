using System.Web;
using FluentAssertions;
using Kerbero.Domain.NukiCredentials.Models;

namespace Kerbero.Domain.Tests.NukiCredentials.Models;

public class NukiCredentialDraftModelTests
{
  [Fact]
  void It_Creates_A_Correct_Uri()
  {
    var uri = NukiCredentialDraftModel.GetOAuthRedirectUri(new NukiApiConfigurationModel(
      apiEndpoint: "https://api.nuki.io",
      clientId: "0",
      scopes: "account notification",
      applicationDomain: "https://test.domain",
      applicationRedirectEndpoint: "api/nuki-credentials/confirm-draft"
    ));

    var partialUri =
      $"https://api.nuki.io/oauth/authorize?response_type=code&client_id=0&scope={HttpUtility.UrlEncode("account notification")}&redirect_uri={HttpUtility.UrlEncode("https://test.domain/api/nuki-credentials/confirm-draft")}";
    uri.ToString().Should().Contain(partialUri);

    var rawGuid = uri.ToString().Replace(partialUri, "");
    var validGuid = HttpUtility.UrlDecode(rawGuid).Remove(0, 1); // Drop the "/" between kerbero api endpoint and the guid
    Guid.TryParse(validGuid, out var parsedGuid);
    parsedGuid.Should().NotBeEmpty();
  }
}