using FluentResults;
using Kerbero.Domain.NukiAuthentication.Models;

namespace Kerbero.Domain.NukiAuthentication.Repositories;

public interface INukiExternalAuthenticationRepository
{
	public Result<NukiRedirectPresentationDto> BuildUriForCode(NukiRedirectExternalRequestDto clientId);

	public Task<Result<NukiAccountExternalResponseDto>> GetNukiAccount(NukiAccountExternalRequestDto externalRequestDto);
}
