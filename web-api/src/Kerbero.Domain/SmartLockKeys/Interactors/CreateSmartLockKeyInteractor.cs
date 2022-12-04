using FluentResults;
using Kerbero.Domain.Common.Errors;
using Kerbero.Domain.Common.Models;
using Kerbero.Domain.NukiCredentials.Repositories;
using Kerbero.Domain.SmartLockKeys.Interfaces;
using Kerbero.Domain.SmartLockKeys.Models;
using Kerbero.Domain.SmartLockKeys.Repositories;
using Kerbero.Domain.SmartLocks.Repositories;

namespace Kerbero.Domain.SmartLockKeys.Interactors;

public class CreateSmartLockKeyInteractor: ICreateSmartLockKeyInteractor
{
	private readonly ISmartLockKeyRepository _smartLockKeyRepository;
	private readonly INukiSmartLockRepository _nukiSmartLockRepository;
	private readonly INukiCredentialRepository _nukiCredentialRepository;

	public CreateSmartLockKeyInteractor(
		ISmartLockKeyRepository smartLockKeyRepository,
		INukiSmartLockRepository nukiSmartLockRepository,
		INukiCredentialRepository nukiCredentialRepository)
	{
		_smartLockKeyRepository = smartLockKeyRepository;
		_nukiSmartLockRepository = nukiSmartLockRepository;
		_nukiCredentialRepository = nukiCredentialRepository;
	}

	public async Task<Result<SmartLockKeyModel>> Handle(string smartLockId, DateTime expiryDate, int credentialId, SmartLockProvider smartLockProvider)
	{
		if (smartLockProvider != SmartLockProvider.Nuki)
		{
			return Result.Fail(new UnsupportedSmartLockProviderError());
		}
		var nukiCredentialResult = await _nukiCredentialRepository.GetById(credentialId);
		if (nukiCredentialResult.IsFailed)
		{
			return Result.Fail(nukiCredentialResult.Errors);
		}

		var nukiSmartLockResult = await _nukiSmartLockRepository.Get(nukiCredentialResult.Value, smartLockId);
		
		if (nukiSmartLockResult.IsFailed)
		{
			return Result.Fail(nukiCredentialResult.Errors);
		}
		
		var generatedKey = SmartLockKeyModel.CreateKey(smartLockId, expiryDate, credentialId, smartLockProvider);
		var createSmartLockKeyResult = await _smartLockKeyRepository.Create(generatedKey);
		
		if (createSmartLockKeyResult.IsFailed)
		{
			return Result.Fail(createSmartLockKeyResult.Errors);
		}
		return createSmartLockKeyResult.Value;
	}
}
