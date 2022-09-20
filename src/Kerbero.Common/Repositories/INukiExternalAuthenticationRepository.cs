using FluentResults;
using Kerbero.Common.Models;

namespace Kerbero.Common.Repositories;

public interface INukiExternalAuthenticationRepository
{
	public Result<NukiRedirectPresentationDto> BuildUriForCode(NukiRedirectExternalRequestDto clientId);

	public Task<Result<NukiAccountExternalResponseDto>> GetNukiAccount(NukiAccountExternalRequestDto externalRequestDto);
}
