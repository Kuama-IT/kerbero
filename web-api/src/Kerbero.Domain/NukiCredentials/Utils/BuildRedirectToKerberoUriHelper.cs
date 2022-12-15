using Kerbero.Domain.NukiCredentials.Models;

namespace Kerbero.Domain.NukiCredentials.Utils;

public static class BuildRedirectToKerberoUriHelper
{
	public static string Handle(NukiApiConfigurationModel nukiApiConfigurationModel)
	{
		return $"{nukiApiConfigurationModel.ApplicationDomain}/{nukiApiConfigurationModel.ApplicationRedirectEndpoint}";
	}
}
