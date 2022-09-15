using Kerbero.Common.Models;

namespace Kerbero.Common.Repositories;

public interface INukiExternalAuthenticationRepository
{
	public Task<NukiAccountExternalResponseDto> GetNukiAccount(NukiAccountExternalRequestDto externalRequestDto);
}
