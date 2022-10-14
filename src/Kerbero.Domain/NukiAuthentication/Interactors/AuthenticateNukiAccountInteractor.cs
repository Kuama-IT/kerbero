using FluentResults;
using Kerbero.Domain.Common.Errors;
using Kerbero.Domain.Common.Interfaces;
using Kerbero.Domain.NukiActions.Repositories;
using Kerbero.Domain.NukiAuthentication.Errors.CreateNukiAccountErrors;
using Kerbero.Domain.NukiAuthentication.Models;
using Kerbero.Domain.NukiAuthentication.Models.AccountMapper;
using Kerbero.Domain.NukiAuthentication.Repositories;

namespace Kerbero.Domain.NukiAuthentication.Interactors;

public class AuthenticateNukiAccountInteractor: InteractorAsync<NukiAccountAuthenticatedRequestDto, NukiAccountAuthenticatedResponseDto>
{

    private readonly INukiAccountPersistentRepository _nukiAccountPersistentRepository;
    private readonly INukiAccountExternalRepository _nukiAccountExternalRepository;
    private readonly INukiSmartLockExternalRepository _nukiSmartLockExternalRepository;

    public AuthenticateNukiAccountInteractor(INukiAccountPersistentRepository nukiAccountPersistentRepository,
        INukiAccountExternalRepository nukiAccountExternalRepositoryRepository, INukiSmartLockExternalRepository nukiSmartLockExternalRepository)
    {
        _nukiAccountExternalRepository = nukiAccountExternalRepositoryRepository;
        _nukiAccountPersistentRepository = nukiAccountPersistentRepository;
        _nukiSmartLockExternalRepository = nukiSmartLockExternalRepository;
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
        if (account.IsFailed || account.Value == null)
        {
            return Result.Fail(new UnauthorizedAccessError());
        }

        // Refresh token
        if (account.Value.ExpiryDate < DateTime.Now)
        {
            var extRes = await _nukiAccountExternalRepository.RefreshToken(new NukiAccountExternalRequestDto
            {
                ClientId = account.Value.ClientId,
                RefreshToken = account.Value.RefreshToken
            });
            if (extRes.IsFailed)
            {
                return Result.Fail(extRes.Errors);
            }

            var nukiAccount = NukiAccountMapper.MapToEntity(extRes.Value);
            account = await _nukiAccountPersistentRepository.Update(nukiAccount);

            if (account.IsFailed)
            {
                Result.Fail(new PersistentResourceNotAvailableError());
            }
        }

        if (account.Value is null)
        {
            return Result.Fail(new UnauthorizedAccessError());
        }
        
        // authenticating external repo
        _nukiSmartLockExternalRepository.Authenticate(account.Value);
        return NukiAccountMapper.MapToAuthenticatedResponse(account.Value);

    }
}