using FluentResults;
using Kerbero.Domain.NukiCredentials.Repositories;
using Kerbero.Domain.SmartLockKeys.Interfaces;
using Kerbero.Domain.SmartLockKeys.Models;
using Kerbero.Domain.SmartLockKeys.Repositories;

namespace Kerbero.Domain.SmartLockKeys.Interactors;

public class GetSmartLockKeysInteractor : IGetSmartLockKeysInteractor
{
	private readonly ISmartLockKeyRepository _smartLockKeyRepository;
	private readonly INukiCredentialRepository _nukiCredentialRepository;

	public GetSmartLockKeysInteractor(
		ISmartLockKeyRepository smartLockKeyRepository,
		INukiCredentialRepository nukiCredentialRepository
		)
	{
		_smartLockKeyRepository = smartLockKeyRepository;
		_nukiCredentialRepository = nukiCredentialRepository;
	}

	public async Task<Result<List<SmartLockKeyModel>>> Handle(Guid userId)
	{
		var credentialsResult = await _nukiCredentialRepository.GetAllByUserId(userId);
		if (credentialsResult.IsFailed)
		{
			return Result.Fail(credentialsResult.Errors);
		}
		
		var smartLockKeysResult = await _smartLockKeyRepository.GetAllByCredentials(credentialsResult.Value);
		if (smartLockKeysResult.IsFailed)
		{
			return Result.Fail(smartLockKeysResult.Errors);
		}

		return smartLockKeysResult.Value;
	}
}
