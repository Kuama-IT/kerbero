using FluentResults;
using Kerbero.Domain.NukiActions.Interfaces;
using Kerbero.Domain.NukiActions.Models.ExternalRequests;
using Kerbero.Domain.NukiActions.Models.PresentationRequest;
using Kerbero.Domain.NukiActions.Repositories;

namespace Kerbero.Domain.NukiActions.Interactors;

public class OpenNukiSmartLockInteractor: IOpenNukiSmartLockInteractor
{
    private readonly INukiSmartLockPersistentRepository _nukiSmartLockPersistentRepository;
    private readonly INukiSmartLockExternalRepository _nukiSmartLockExternalRepository;

    public OpenNukiSmartLockInteractor(INukiSmartLockPersistentRepository nukiSmartLockPersistentRepository,
        INukiSmartLockExternalRepository nukiSmartLockExternalRepository)
    {
        _nukiSmartLockPersistentRepository = nukiSmartLockPersistentRepository;
        _nukiSmartLockExternalRepository = nukiSmartLockExternalRepository;
    }

    public async Task<Result> Handle(OpenNukiSmartLockPresentationRequest presentationRequest)
    {
        var dbResult = _nukiSmartLockPersistentRepository.GetById(presentationRequest.NukiSmartLockId);

        if (dbResult.IsFailed)
        {
            return dbResult.ToResult();
        }

        return await _nukiSmartLockExternalRepository.OpenNukiSmartLock(
            new NukiSmartLockExternalRequest(presentationRequest.AccessToken, dbResult.Value.ExternalSmartLockId));
    }
}