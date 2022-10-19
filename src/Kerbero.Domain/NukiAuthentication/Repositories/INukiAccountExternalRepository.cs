using FluentResults;
using Kerbero.Domain.NukiAuthentication.Models;
using Kerbero.Domain.NukiAuthentication.Models.ExternalRequests;
using Kerbero.Domain.NukiAuthentication.Models.ExternalResponses;
using Kerbero.Domain.NukiAuthentication.Models.PresentationResponses;

namespace Kerbero.Domain.NukiAuthentication.Repositories;

public interface INukiAccountExternalRepository
{
	public Result<NukiRedirectPresentationResponse> BuildUriForCode(NukiRedirectExternalRequest redirectExternalRequest);

	public Task<Result<NukiAccountExternalResponseDto>> GetNukiAccount(NukiAccountExternalRequest accountExternalRequest);
	
	public Task<Result<NukiAccountExternalResponseDto>> RefreshToken(NukiAccountExternalRequest accountExternalRequest);
}
