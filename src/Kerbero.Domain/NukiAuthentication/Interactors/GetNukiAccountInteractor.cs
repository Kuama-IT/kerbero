using FluentResults;
using Kerbero.Domain.Common.Errors;
using Kerbero.Domain.Common.Interfaces;
using Kerbero.Domain.NukiAuthentication.Models;
using Kerbero.Domain.NukiAuthentication.Models.AccountMapper;
using Kerbero.Domain.NukiAuthentication.Repositories;

namespace Kerbero.Domain.NukiAuthentication.Interactors;

public class GetNukiAccountInteractor: InteractorAsync<NukiAccountAuthenticatedRequestDto, NukiAccountAuthenticatedResponseDto>
{

    private readonly INukiAccountPersistentRepository _nukiAccountPersistentRepository;
    private readonly INukiAccountExternalRepository _nukiAccountExternal;

    public GetNukiAccountInteractor(INukiAccountPersistentRepository nukiAccountPersistentRepository, INukiAccountExternalRepository nukiAccountExternal)
    {
        _nukiAccountExternal = nukiAccountExternal;
        _nukiAccountPersistentRepository = nukiAccountPersistentRepository;
    }

    /// <summary>
    /// Get an account from kerbero persistent repository
    /// If the account has expired token, it calls the refresh token
    /// Update the persistent entry and return the authenticated account
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public async Task<Result<NukiAccountAuthenticatedResponseDto>> Handle(NukiAccountAuthenticatedRequestDto request)
    {
        var account = _nukiAccountPersistentRepository.GetAccount(request.NukiAccountId);
        if (account.IsFailed)
        {
            return Result.Fail(new UnauthorizedAccessError());
        }

        if (account.Value.ExpiryDate > DateTime.Now && account.Value != null)
        {
            return NukiAccountMapper.MapToAuthenticatedResponse(account.Value);
        }
		
        var extRes = await _nukiAccountExternal.RefreshToken(new NukiAccountExternalRequestDto
        {
            ClientId = account.Value!.ClientId,
            RefreshToken = account.Value.RefreshToken
        });
        if (extRes.IsFailed)
        {
            return Result.Fail(extRes.Errors);
        }
        var nukiAccount = NukiAccountMapper.MapToEntity(extRes.Value);
        var persistentResult = await _nukiAccountPersistentRepository.Update(nukiAccount);

        if (persistentResult.IsFailed)
        {
            Result.Fail(new PersistentResourceNotAvailableError());
        }

        if (persistentResult.Value is not null)
        {
            return NukiAccountMapper.MapToAuthenticatedResponse(persistentResult.Value);
        }

        return Result.Fail(new UnauthorizedAccessError());
    }
}