using FluentResults;
using Kerbero.Domain.Common.Errors;
using Kerbero.Domain.Common.Models;
using Kerbero.Domain.NukiCredentials.Repositories;
using Kerbero.Domain.SmartLockKeys.Errors;
using Kerbero.Domain.SmartLockKeys.Interfaces;
using Kerbero.Domain.SmartLockKeys.Repositories;
using Kerbero.Domain.SmartLocks.Interfaces;

namespace Kerbero.Domain.SmartLockKeys.Interactors;

public class CloseSmartLockWithKeyInteractor : ICloseSmartLockWithKeyInteractor
{
	private readonly ISmartLockKeyRepository _smartLockKeyRepository;
	private readonly INukiCredentialRepository _nukiCredentialRepository;
	private readonly ICloseSmartLockInteractor _closeSmartLockInteractor;

	public CloseSmartLockWithKeyInteractor(
		ISmartLockKeyRepository smartLockKeyRepository,
		INukiCredentialRepository nukiCredentialRepository,
		ICloseSmartLockInteractor closeSmartLockInteractor
	)
	{
		_smartLockKeyRepository = smartLockKeyRepository;
		_nukiCredentialRepository = nukiCredentialRepository;
		_closeSmartLockInteractor = closeSmartLockInteractor;
	}

	public async Task<Result> Handle(Guid smartLockKeyId, string smartLockKeyPassword)
	{
		var smartLockKeyResult = await _smartLockKeyRepository.GetById(smartLockKeyId);
		if (smartLockKeyResult.IsFailed)
		{
			return Result.Fail(smartLockKeyResult.Errors);
		}

		var smartLockKey = smartLockKeyResult.Value;
		var validationResult = _ValidateSmartLockKey(smartLockKey.Password, smartLockKeyPassword, smartLockKey.ValidUntil);
		if (validationResult.IsFailed)
		{
			return Result.Fail(validationResult.Errors);
		}
		
		var smartLockProvider = SmartLockProvider.TryParse(smartLockKey.SmartLockProvider);
		if (smartLockProvider != SmartLockProvider.Nuki)
		{
			return Result.Fail(new UnsupportedSmartLockProviderError());
		}

		var nukiCredentialResult = await _nukiCredentialRepository.GetById(smartLockKey.CredentialId);
		if (nukiCredentialResult.IsFailed)
		{
			return Result.Fail(nukiCredentialResult.Errors);
		}

		var closeResult = await _closeSmartLockInteractor.Handle(
			nukiCredentialResult.Value.UserId,
			smartLockProvider,
			smartLockKeyResult.Value.SmartLockId,
			nukiCredentialResult.Value.Id
		);
		
		if (closeResult.IsFailed)
		{
			return Result.Fail(closeResult.Errors);
		}

		// update usage on smartlock key
		smartLockKey.UsageCounter++;
		var updateSmartLockKeyResult = await _smartLockKeyRepository.Update(smartLockKey);
		if (updateSmartLockKeyResult.IsFailed)
		{
			return Result.Fail(updateSmartLockKeyResult.Errors);
		}	
		
		return Result.Ok();
	}

	private Result _ValidateSmartLockKey(
		string expectedPassword, 
		string actualPassword, 
		DateTime expiryDate)
	{
		if (expectedPassword != actualPassword)
		{
			return Result.Fail(new UnauthorizedAccessError());
		}

		if (expiryDate < DateTime.Now)
		{
			return Result.Fail(new SmartLockKeyExpiredError());
		}
		return Result.Ok();
	}
}
