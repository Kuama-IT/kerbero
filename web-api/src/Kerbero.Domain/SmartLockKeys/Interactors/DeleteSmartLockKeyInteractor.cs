using FluentResults;
using Kerbero.Domain.Common.Errors;
using Kerbero.Domain.NukiCredentials.Repositories;
using Kerbero.Domain.SmartLockKeys.Interfaces;
using Kerbero.Domain.SmartLockKeys.Models;
using Kerbero.Domain.SmartLockKeys.Repositories;

namespace Kerbero.Domain.SmartLockKeys.Interactors;

public class DeleteSmartLockKeyInteractor : IDeleteSmartLockKeyInteractor
{
    private readonly INukiCredentialRepository _nukiCredentialRepository;
    private readonly ISmartLockKeyRepository _smartLockKeyRepository;

    public DeleteSmartLockKeyInteractor(
        INukiCredentialRepository nukiCredentialRepository,
        ISmartLockKeyRepository smartLockKeyRepository)
    {
        _nukiCredentialRepository = nukiCredentialRepository;
        _smartLockKeyRepository = smartLockKeyRepository;
    }

    public async Task<Result<SmartLockKeyModel>> Handle(Guid userId, Guid smartLockId)
    {
        var smartLockKeyResult = await _smartLockKeyRepository.GetById(smartLockId);
        if (smartLockKeyResult.IsFailed)
        {
            return Result.Fail(smartLockKeyResult.Errors);
        }

        var nukiCredentialsResult = await _nukiCredentialRepository.GetAllByUserId(userId);
        if (nukiCredentialsResult.IsFailed)
        {
            return Result.Fail(nukiCredentialsResult.Errors);
        }

        var nukiCredentialModel = nukiCredentialsResult.Value.Find(credential => credential.Id == smartLockKeyResult.Value.CredentialId);
        if (nukiCredentialModel is null)
        {
            return Result.Fail(new UnauthorizedAccessError());
        }

        return await _smartLockKeyRepository.Delete(smartLockId);
    }
}