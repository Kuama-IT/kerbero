using FluentResults;
using Kerbero.Domain.Common.Errors;
using Kerbero.Domain.NukiCredentials.Repositories;
using Kerbero.Domain.SmartLockKeys.Dtos;
using Kerbero.Domain.SmartLockKeys.Interfaces;
using Kerbero.Domain.SmartLockKeys.Mappers;
using Kerbero.Domain.SmartLockKeys.Models;
using Kerbero.Domain.SmartLockKeys.Repositories;
using Kerbero.Domain.SmartLocks.Models;
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

	public async Task<Result<SmartLockKeyDto>> Handle(CreateSmartLockKeyParams @params)
	{
		if (@params.SmartLockProvider != SmartLockProvider.Nuki)
		{
			return Result.Fail(new UnsupportedSmartLockProviderError());
		}
		var nukiCredentialResult = await _nukiCredentialRepository.GetById(@params.CredentialId);
		if (nukiCredentialResult.IsFailed)
		{
			return Result.Fail(nukiCredentialResult.Errors);
		}

		var nukiSmartLockResult = await _nukiSmartLockRepository.Get(nukiCredentialResult.Value, @params.SmartLockId);
		
		if (nukiSmartLockResult.IsFailed)
		{
			return Result.Fail(nukiCredentialResult.Errors);
		}
		
		var generatedKey = SmartLockKeyModel.CreateKey(@params.SmartLockId, @params.ExpiryDate, @params.CredentialId);
		var createSmartLockKeyResult = await _smartLockKeyRepository.Create(generatedKey);
		
		if (createSmartLockKeyResult.IsFailed)
		{
			return Result.Fail(createSmartLockKeyResult.Errors);
		}
		return SmartLockKeyMapper.Map(createSmartLockKeyResult.Value);
	}
}
