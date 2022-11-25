using FluentResults;
using Kerbero.Domain.Common.Errors;
using Kerbero.Domain.NukiActions.Repositories;
using Kerbero.Domain.NukiAuthentication.Dtos;
using Kerbero.Domain.NukiAuthentication.Interfaces;
using Kerbero.Domain.NukiAuthentication.Mappers;
using Kerbero.Domain.NukiAuthentication.Models;
using Kerbero.Domain.NukiAuthentication.Repositories;

namespace Kerbero.Domain.NukiAuthentication.Interactors;

public class GetNukiCredentialInteractor: IGetNukiCredentialInteractor
{

    private readonly INukiCredentialRepository _nukiCredentialRepository;
    private readonly INukiOAuthRepository _nukiOAuthRepository;

    public GetNukiCredentialInteractor(INukiCredentialRepository nukiCredentialRepository,
        INukiOAuthRepository nukiOAuthRepository)
    {
        _nukiOAuthRepository = nukiOAuthRepository;
        _nukiCredentialRepository = nukiCredentialRepository;
    }

    /// <summary>
    /// Gets an account from kerbero local repository.
    /// If the account has expired token executes the following:
    /// - retrieves again the nuki account from Nuki apis with a new token and refresh token
    /// - updates the local entry
    ///
    /// finally returns the authenticated account
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public async Task<Result<NukiCredentialDto>> Handle(GetNukiCredentialParams request)
    {
        var accountResult = await _nukiCredentialRepository.GetById(request.NukiCredentialId);
        
        if (accountResult.IsFailed)
        {
            return Result.Fail(accountResult.Errors);
        }

        // Data on DB are mandatory converted to UTC
        var isAuthenticationTokenExpired = accountResult.Value.ExpireDate < DateTime.Now.ToUniversalTime();

        if (!isAuthenticationTokenExpired)
        {
            return NukiCredentialMapper.Map(accountResult.Value);
        }
        
        // Refresh token
        accountResult = await _nukiOAuthRepository.RefreshNukiOAuth(new NukiOAuthRequest
        {
            ClientId = accountResult.Value.ClientId,
            RefreshToken = accountResult.Value.RefreshToken
        });
            
        if (accountResult.IsFailed)
        {
            return Result.Fail(accountResult.Errors);
        }

        var updateAccountResult = await _nukiCredentialRepository.Update(accountResult.Value);

        if (updateAccountResult.IsFailed)
        {
            return Result.Fail(updateAccountResult.Errors);
        }
        
        return NukiCredentialMapper.Map(updateAccountResult.Value);
    }
}