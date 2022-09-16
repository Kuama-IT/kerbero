using FluentResults;
using Kerbero.Common.Models;

namespace Kerbero.Common.Repositories;

public interface INukiExternalAuthenticationRepository
{
	public Task<Result<NukiAccountExternalResponseDto>> GetNukiAccount(NukiAccountExternalRequestDto externalRequestDto);
	public Result<Uri> BuildUriForCode(string clientId);
}
