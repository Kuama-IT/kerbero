using FluentResults;
using Kerbero.Domain.Common.Interfaces;
using Kerbero.Domain.NukiAuthentication.Models;
using Kerbero.Domain.NukiAuthentication.Models.AccountMapper;
using Kerbero.Domain.NukiAuthentication.Repositories;

namespace Kerbero.Domain.NukiAuthentication.Interactors;

public class CreateNukiAccountInteractor: InteractorAsync<NukiAccountExternalRequestDto, NukiAccountPresentationDto>
{
	private readonly INukiPersistentAccountRepository _nukiPersistentAccountRepository;
	private readonly INukiExternalAuthenticationRepository _nukiExternalAuthenticationRepository;

	public CreateNukiAccountInteractor(INukiPersistentAccountRepository nukiPersistentAccountRepository, 
		INukiExternalAuthenticationRepository nukiExternalAuthenticationRepository)
	{
		_nukiExternalAuthenticationRepository = nukiExternalAuthenticationRepository;
		_nukiPersistentAccountRepository = nukiPersistentAccountRepository;
	}

	public async Task<Result<NukiAccountPresentationDto>> Handle(NukiAccountExternalRequestDto externalRequestDto)
	{
		var extRes = await _nukiExternalAuthenticationRepository.GetNukiAccount(externalRequestDto);

		if (!extRes.IsSuccess)
		{
			return Result.Fail(extRes.Errors);
		}
		var nukiAccount = NukiAccountMapper.MapToEntity(extRes.Value);
		var persistentResult = await _nukiPersistentAccountRepository.Create(nukiAccount);

		return persistentResult.IsSuccess ? // isSuccess
			Result.Ok(NukiAccountMapper.MapToPresentation(persistentResult.Value)) : Result.Fail(persistentResult.Errors);

	}
}
