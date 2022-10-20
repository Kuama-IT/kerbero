using FluentResults;
using Kerbero.Domain.NukiActions.Interfaces;
using Kerbero.Domain.NukiActions.Mappers;
using Kerbero.Domain.NukiActions.Models.PresentationRequest;
using Kerbero.Domain.NukiActions.Repositories;

namespace Kerbero.Domain.NukiActions.Interactors;

public class
	GetNukiSmartLockListInteractor : IGetNukiSmartLockListInteractor
{
	private readonly INukiSmartLockExternalRepository _nukiSmartLockClient;

	public GetNukiSmartLockListInteractor(INukiSmartLockExternalRepository nukiSmartLockClient)
	{
		_nukiSmartLockClient = nukiSmartLockClient;
	}

	public async Task<Result<List<KerberoSmartLockPresentationRequest>>> Handle()
	{

		var smartLockList = await _nukiSmartLockClient.GetNukiSmartLocks();
		if (smartLockList.IsFailed)
		{
			return Result.Fail(smartLockList.Errors);
		}

		var mapped = smartLockList.Value.Select(NukiSmartLockMapper.Map).ToList();

		return Result.Ok(mapped);
	}
}
