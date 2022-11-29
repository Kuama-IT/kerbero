using FluentResults;
using Kerbero.Domain.NukiCredentials.Dtos;
using Kerbero.Domain.NukiCredentials.Interfaces;
using Kerbero.Domain.NukiCredentials.Mappers;
using Kerbero.Domain.NukiCredentials.Repositories;

namespace Kerbero.Domain.NukiCredentials.Interactors;

public class GetNukiCredentialInteractor: IGetNukiCredentialInteractor
{

    private readonly INukiCredentialRepository _nukiCredentialRepository;

    public GetNukiCredentialInteractor(INukiCredentialRepository nukiCredentialRepository)
    {
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
        var nukiCredentialsResult = await _nukiCredentialRepository.GetById(request.NukiCredentialId);
        
        if (nukiCredentialsResult.IsFailed)
        {
            return Result.Fail(nukiCredentialsResult.Errors);
        }

        return NukiCredentialMapper.Map(nukiCredentialsResult.Value);
    }
}
