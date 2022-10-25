using FluentResults;
using Kerbero.Domain.NukiActions.Interfaces;
using Kerbero.Domain.NukiActions.Models.ExternalRequests;
using Kerbero.Domain.NukiActions.Models.PresentationRequest;
using Kerbero.Domain.NukiActions.Repositories;

namespace Kerbero.Domain.NukiActions.Interactors;

public class CloseNukiSmartLockInteractor: ICloseNukiSmartLockInteractor
{
    private readonly INukiSmartLockPersistentRepository _persistentRepository;
    private readonly INukiSmartLockExternalRepository _externalRepository;

    public CloseNukiSmartLockInteractor(INukiSmartLockPersistentRepository persistent, INukiSmartLockExternalRepository external)
    {
        _persistentRepository = persistent;
        _externalRepository = external;
    }

    public async Task<Result> Handle(CloseNukiSmartLockPresentationRequest request)
    {
        var smartLock = _persistentRepository.GetById(request.SmartLockId);

        if (smartLock.IsFailed)
        {
            return smartLock.ToResult();
        }

        return await _externalRepository.CloseNukiSmartLock(new NukiSmartLockExternalRequest(request.AccessToken, smartLock.Value.ExternalSmartLockId));
    }
}