using FluentResults;
using Kerbero.Domain.NukiActions.Interfaces;
using Kerbero.Domain.NukiActions.Mappers;
using Kerbero.Domain.NukiActions.Models.PresentationRequest;
using Kerbero.Domain.NukiActions.Models.PresentationResponse;
using Kerbero.Domain.NukiActions.Repositories;

namespace Kerbero.Domain.NukiActions.Interactors;

public class GetNukiSmartLocksInteractor : IGetNukiSmartLocksInteractor
{
	private readonly INukiSmartLockExternalRepository _nukiSmartLockClient;

	public GetNukiSmartLocksInteractor(INukiSmartLockExternalRepository nukiSmartLockClient)
	{
		_nukiSmartLockClient = nukiSmartLockClient;
	}

	public async Task<Result<List<KerberoSmartLockPresentationResponse>>> Handle(NukiSmartLocksPresentationRequest request)
	{

		var smartLockList = await _nukiSmartLockClient.GetNukiSmartLocks(request.Token);
		if (smartLockList.IsFailed)
		{
			return Result.Fail(smartLockList.Errors);
		}

		var mapped = smartLockList.Value.Select(NukiSmartLockMapper.MapToPresentation).ToList();

		return Result.Ok(mapped);
	}
}
