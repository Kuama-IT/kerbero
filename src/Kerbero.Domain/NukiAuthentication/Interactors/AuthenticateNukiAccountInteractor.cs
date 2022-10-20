using FluentResults;
using Kerbero.Domain.Common.Errors;
using Kerbero.Domain.NukiActions.Repositories;
using Kerbero.Domain.NukiAuthentication.Interfaces;
using Kerbero.Domain.NukiAuthentication.Mappers;
using Kerbero.Domain.NukiAuthentication.Models.ExternalRequests;
using Kerbero.Domain.NukiAuthentication.Models.PresentationRequests;
using Kerbero.Domain.NukiAuthentication.Models.PresentationResponses;
using Kerbero.Domain.NukiAuthentication.Repositories;

namespace Kerbero.Domain.NukiAuthentication.Interactors;

public class AuthenticateNukiAccountInteractor: IAuthenticateNukiAccountInteractor
{

    private readonly INukiAccountPersistentRepository _nukiAccountPersistentRepository;
    private readonly INukiAccountExternalRepository _nukiAccountExternalRepository;
    private readonly INukiSmartLockExternalRepository _nukiSmartLockExternalRepository;

    public AuthenticateNukiAccountInteractor(INukiAccountPersistentRepository nukiAccountPersistentRepository,
        INukiAccountExternalRepository nukiAccountExternalRepository, INukiSmartLockExternalRepository nukiSmartLockExternalRepository)
    {
        _nukiAccountExternalRepository = nukiAccountExternalRepository;
        _nukiAccountPersistentRepository = nukiAccountPersistentRepository;
        _nukiSmartLockExternalRepository = nukiSmartLockExternalRepository;
    }

    /// <summary>
    /// Get an account from kerbero persistent repository
    /// If the account has expired token, it calls the refresh token
    /// Update the persistent entry and return the authenticated account
    /// </summary>
    /// <param name="repositoryPresentationRequest"></param>
    /// <returns></returns>
    public async Task<Result<AuthenticateRepositoryPresentationResponse>> Handle(AuthenticateRepositoryPresentationRequest repositoryPresentationRequest)
    {
        var account = _nukiAccountPersistentRepository.GetAccount(repositoryPresentationRequest.NukiAccountId);
        if (account.IsFailed || account.Value == null)
        {
            return Result.Fail(new UnauthorizedAccessError());
        }

        // Refresh token
        if (account.Value.ExpiryDate < DateTime.Now)
        {
            var extRes = await _nukiAccountExternalRepository.RefreshToken(new NukiAccountExternalRequest
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
        _nukiSmartLockExternalRepository.Token = account.Value.Token;
        return NukiAccountMapper.MapToAuthenticatedResponse(account.Value);

    }
}