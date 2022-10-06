using FluentResults;
using Kerbero.Domain.Common.Interfaces;
using Kerbero.Domain.NukiAuthentication.Models;
using Kerbero.Domain.NukiAuthentication.Models.AccountMapper;
using Kerbero.Domain.NukiAuthentication.Repositories;

namespace Kerbero.Domain.NukiAuthentication.Interactors;

public class CreateNukiAccountInteractor: InteractorAsync<NukiAccountExternalRequestDto, NukiAccountPresentationDto>
{
	private readonly INukiAccountPersistentRepository _nukiAccountPersistentRepository;
	private readonly INukiAccountExternalRepository _nukiAccountExternalRepository;

	public CreateNukiAccountInteractor(INukiAccountPersistentRepository nukiAccountPersistentRepository, 
		INukiAccountExternalRepository nukiAccountExternalRepository)
	{
		_nukiAccountExternalRepository = nukiAccountExternalRepository;
		_nukiAccountPersistentRepository = nukiAccountPersistentRepository;
	}

	public async Task<Result<NukiAccountPresentationDto>> Handle(NukiAccountExternalRequestDto externalRequestDto)
	{
		var extRes = await _nukiAccountExternalRepository.GetNukiAccount(externalRequestDto);

		if (!extRes.IsSuccess)
		{
			return Result.Fail(extRes.Errors);
		}
		var nukiAccount = NukiAccountMapper.MapToEntity(extRes.Value);
		var persistentResult = await _nukiAccountPersistentRepository.Create(nukiAccount);

		return persistentResult.IsSuccess ? // isSuccess
			Result.Ok(NukiAccountMapper.MapToPresentation(persistentResult.Value)) : Result.Fail(persistentResult.Errors);

	}
}
