using FluentResults;
using Kerbero.Domain.Common.Interfaces;
using Kerbero.Domain.Common.Models;
using Kerbero.Domain.NukiActions.Mappers;
using Kerbero.Domain.NukiActions.Repositories;

namespace Kerbero.Domain.NukiActions.Interactors;

public class
	GetNukiSmartLocksListInteractor : InteractorAsyncNoParam<List<KerberoSmartLockPresentationDto>>
{
	private readonly INukiSmartLockExternalRepository _nukiSmartLockClient;

	public GetNukiSmartLocksListInteractor(INukiSmartLockExternalRepository nukiSmartLockClient)
	{
		_nukiSmartLockClient = nukiSmartLockClient;
	}

	public async Task<Result<List<KerberoSmartLockPresentationDto>>> Handle()
	{

		var smartLockList = await _nukiSmartLockClient.GetNukiSmartLockList();
		if (smartLockList.IsFailed)
		{
			return Result.Fail(smartLockList.Errors);
		}

		var mapped = smartLockList.Value.Select(NukiSmartLockMapper.Map).ToList();

		return Result.Ok(mapped);
	}
}
