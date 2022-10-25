using FluentResults;
using Kerbero.Domain.NukiActions.Interfaces;
using Kerbero.Domain.NukiActions.Models.ExternalRequests;
using Kerbero.Domain.NukiActions.Models.Mapper;
using Kerbero.Domain.NukiActions.Models.PresentationRequest;
using Kerbero.Domain.NukiActions.Models.PresentationResponse;
using Kerbero.Domain.NukiActions.Repositories;

namespace Kerbero.Domain.NukiActions.Interactors;

public class CreateNukiSmartLockInteractor: ICreateNukiSmartLockInteractor
{
    private readonly INukiSmartLockExternalRepository _nukiSmartLockExternalRepository;
    private readonly INukiSmartLockPersistentRepository _nukiSmartLockPersistentRepository;

    public CreateNukiSmartLockInteractor(INukiSmartLockExternalRepository nukiSmartLockExternalRepository,
        INukiSmartLockPersistentRepository nukiSmartLockPersistentRepository)
    {
        _nukiSmartLockExternalRepository = nukiSmartLockExternalRepository;
        _nukiSmartLockPersistentRepository = nukiSmartLockPersistentRepository;
    }

    public async Task<Result<KerberoSmartLockPresentationResponse>> Handle(CreateNukiSmartLockPresentationRequest request)
    {
        var extResponse = await _nukiSmartLockExternalRepository.GetNukiSmartLock(new NukiSmartLockExternalRequest(request.AccessToken, request.NukiSmartLockId));
        if (extResponse.IsFailed)
        {
            return extResponse.ToResult();
        }
        
        var smartLockEntity = await _nukiSmartLockPersistentRepository.Create(NukiSmartLockMapper.MapToEntity(extResponse.Value));
        if (smartLockEntity.IsFailed)
        {
            return smartLockEntity.ToResult();
        }

        return NukiSmartLockMapper.MapToPresentation(smartLockEntity.Value);    
    }
}