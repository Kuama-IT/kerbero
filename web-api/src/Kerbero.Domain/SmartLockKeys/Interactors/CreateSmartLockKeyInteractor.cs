using FluentResults;
using Kerbero.Domain.SmartLockKeys.Managers;
using Kerbero.Domain.SmartLockKeys.Mappers;
using Kerbero.Domain.SmartLockKeys.Models.PresentationRequests;
using Kerbero.Domain.SmartLockKeys.Models.PresentationResponses;
using Kerbero.Domain.SmartLockKeys.Repositories;

namespace Kerbero.Domain.SmartLockKeys.Interactors;

public class CreateSmartLockKeyInteractor
{
	private readonly ISmartLockKeyGeneratorManager _smartLockKeyGeneratorManager;
	private readonly ISmartLockKeyPersistentRepository _smartLockKeyPersistentRepository;

	public CreateSmartLockKeyInteractor(ISmartLockKeyGeneratorManager smartLockKeyGeneratorManager, ISmartLockKeyPersistentRepository smartLockKeyPersistentRepository)
	{
		_smartLockKeyGeneratorManager = smartLockKeyGeneratorManager;
		_smartLockKeyPersistentRepository = smartLockKeyPersistentRepository;
	}

	public async Task<Result<CreateSmartLockKeyPresentationResponse>> Handle(CreateSmartLockKeyPresentationRequest presentationRequest)
	{
		var generatedKeyResult = _smartLockKeyGeneratorManager.GenerateSmartLockKey(presentationRequest.SmartLockId, presentationRequest.ExpiryDate);
		if (generatedKeyResult.IsFailed)
		{
			return generatedKeyResult.ToResult();
		}
		var createResult = await _smartLockKeyPersistentRepository.Create(generatedKeyResult.Value);
		if (createResult.IsFailed)
		{
			return createResult.ToResult();
		}
		return SmartLockKeyMapper.Map(createResult.Value);
	}
}
