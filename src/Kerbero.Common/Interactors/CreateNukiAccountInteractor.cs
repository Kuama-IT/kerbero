using Kerbero.Common.Entities;
using Kerbero.Common.Exceptions;
using Kerbero.Common.Interfaces;
using Kerbero.Common.Models;
using Kerbero.Common.Models.AccountMapper;
using Kerbero.Common.Repositories;

namespace Kerbero.Common.Interactors;

public class CreateNukiAccountInteractor: Interactor<NukiAccountExternalRequestDto, Task<NukiAccountPresentationDto>>
{
	private readonly INukiPersistentAccountRepository _nukiPersistentAccountRepository;
	private readonly INukiExternalAuthenticationRepository _nukiExternalAuthenticationRepository;

	public CreateNukiAccountInteractor(INukiPersistentAccountRepository nukiPersistentAccountRepository, 
		INukiExternalAuthenticationRepository nukiExternalAuthenticationRepository)
	{
		_nukiExternalAuthenticationRepository = nukiExternalAuthenticationRepository;
		_nukiPersistentAccountRepository = nukiPersistentAccountRepository;
	}

	public async Task<NukiAccountPresentationDto> Handle(NukiAccountExternalRequestDto externalRequestDto)
	{
		var nukiAccountDto = await _nukiExternalAuthenticationRepository.GetNukiAccount(externalRequestDto);
		
		var nukiAccount = NukiAccountMapper.MapToEntity(nukiAccountDto);
		
		EnsureNukiAccountIsValid(nukiAccount);
		
		nukiAccount = await _nukiPersistentAccountRepository.Create(nukiAccount);

		// return presentation object
		return NukiAccountMapper.MapToPresentation(nukiAccount);
	}

	/// <summary>
	/// Verify if a Nuki account is a valid account
	/// </summary>
	/// <param name="nukiAccount"></param>
	/// <returns></returns>
	private static void EnsureNukiAccountIsValid(NukiAccount nukiAccount)
	{
		if (nukiAccount.ExpiryDate.Date <= DateTime.Now.Date || nukiAccount.TokenExpiringTimeInSeconds == 0)
		{
			throw new TokenExpiredException();
		}
		if (string.IsNullOrWhiteSpace(nukiAccount.Token) ||
		    string.IsNullOrWhiteSpace(nukiAccount.RefreshToken))
		{
			throw new InvalidTokenException();
		};
	}
}
