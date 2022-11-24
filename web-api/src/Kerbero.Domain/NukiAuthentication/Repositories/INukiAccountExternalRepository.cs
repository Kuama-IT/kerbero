using FluentResults;
using Kerbero.Domain.NukiAuthentication.Models;
using Kerbero.Domain.NukiAuthentication.Models.ExternalRequests;
using Kerbero.Domain.NukiAuthentication.Models.ExternalResponses;
using Kerbero.Domain.NukiAuthentication.Models.PresentationResponses;

namespace Kerbero.Domain.NukiAuthentication.Repositories;

public interface INukiAccountExternalRepository
{
	public Result<NukiAccountBuildUriForCodeExternalResponse> BuildUriForCode(NukiAccountBuildUriForCodeExternalRequest accountBuildUriForCodeExternalRequest);

	public Task<Result<NukiAccountExternalResponse>> GetNukiAccount(NukiAccountExternalRequest accountExternalRequest);
	
	public Task<Result<NukiAccountExternalResponse>> RefreshToken(NukiAccountExternalRequest accountExternalRequest);
}
