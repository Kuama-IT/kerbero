using Kerbero.Domain.Common.Models;

namespace Kerbero.Domain.NukiActions.Models.Mapper;

public static class NukiSmartLockListMapper
{
	public static NukiSmartLocksListPresentationDto Map(NukiSmartLocksListExternalResponseDto nukiSmartLocksListExternal)
	{
		var nukiSmartLocksListPresentation = new NukiSmartLocksListPresentationDto();
		foreach (var nukiSmartLock in nukiSmartLocksListExternal.NukiSmartLockList)
		{
			nukiSmartLocksListPresentation.NukiSmartLocksList.Add(new KerberoSmartLockPresentationDto<NukiSmartLockState>
			{
				ExternalName = nukiSmartLock.Name,
				ExternalState = nukiSmartLock.State,
				ExternalType = nukiSmartLock.Type,
				ExternalAccountId = nukiSmartLock.AccountId,
				ExternalSmartLockId = nukiSmartLock.SmartLockId
			} );
		}

		return nukiSmartLocksListPresentation;
	}
}
