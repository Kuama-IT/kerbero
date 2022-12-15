using FluentResults;
using Kerbero.Domain.Common.Repositories;
using Kerbero.Domain.NukiCredentials.Interfaces;

namespace Kerbero.Domain.NukiCredentials.Interactors;

public class BuildWebAppRedirectUriInteractor: IBuildWebAppRedirectUriInteractor
{
	private readonly IKerberoConfigurationRepository _kerberoConfigurationRepository;

	public BuildWebAppRedirectUriInteractor(IKerberoConfigurationRepository kerberoConfigurationRepository)
	{
		_kerberoConfigurationRepository = kerberoConfigurationRepository;
	}

	public async Task<Result<Uri>> Handle(bool isSuccessUri)
	{
		var nukiApiDefinitionResult = await _kerberoConfigurationRepository.GetNukiApiDefinition();
		if (nukiApiDefinitionResult.IsFailed)
		{
			return Result.Fail(nukiApiDefinitionResult.Errors);
		}

		var nukiApiDefinition = nukiApiDefinitionResult.Value;
		var redirectToWebApp = isSuccessUri ? $"{nukiApiDefinition.WebAppDomain}/{nukiApiDefinition.WebAppSuccessRedirectEndpoint}"
			: $"{nukiApiDefinition.WebAppDomain}/{nukiApiDefinition.WebAppFailureRedirectEndpoint}";
		return new Uri(redirectToWebApp);
	}
}
