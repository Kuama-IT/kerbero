using FluentResults;
using Kerbero.Domain.NukiAuthentication.Models;

namespace Kerbero.Domain.NukiAuthentication.Repositories;

public interface INukiAccountExternalRepository
{
	public Result<NukiRedirectPresentationDto> BuildUriForCode(NukiRedirectExternalRequestDto clientId);

	public Task<Result<NukiAccountExternalResponseDto>> GetNukiAccount(NukiAccountExternalRequestDto externalRequestDto);
	
	public Task<Result<NukiAccountExternalResponseDto>> RefreshToken(NukiAccountExternalRequestDto nukiAccountExternalRequestDto);
}
