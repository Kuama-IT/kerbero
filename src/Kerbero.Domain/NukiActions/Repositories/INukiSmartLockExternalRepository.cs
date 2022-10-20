using FluentResults;
using Kerbero.Domain.NukiActions.Models;
using Kerbero.Domain.NukiActions.Models.ExternalResponses;
using Kerbero.Domain.NukiAuthentication.Entities;
using Kerbero.Domain.NukiAuthentication.Interfaces;

namespace Kerbero.Domain.NukiActions.Repositories;

public interface INukiSmartLockExternalRepository: ITokenAuthenticationHolder
{
	Task<Result<List<NukiSmartLockExternalResponse>>> GetNukiSmartLocks();
}
