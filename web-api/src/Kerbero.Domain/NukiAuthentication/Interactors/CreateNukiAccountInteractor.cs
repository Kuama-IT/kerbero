using FluentResults;
using Kerbero.Domain.Common.Interfaces;
using Kerbero.Domain.NukiAuthentication.Interfaces;
using Kerbero.Domain.NukiAuthentication.Mappers;
using Kerbero.Domain.NukiAuthentication.Models;
using Kerbero.Domain.NukiAuthentication.Models.ExternalRequests;
using Kerbero.Domain.NukiAuthentication.Models.PresentationRequests;
using Kerbero.Domain.NukiAuthentication.Models.PresentationResponses;
using Kerbero.Domain.NukiAuthentication.Repositories;

namespace Kerbero.Domain.NukiAuthentication.Interactors;

public class CreateNukiAccountInteractor: ICreateNukiAccountInteractor
{
	private readonly INukiAccountPersistentRepository _nukiAccountPersistentRepository;
	private readonly INukiAccountExternalRepository _nukiAccountExternalRepository;

	public CreateNukiAccountInteractor(INukiAccountPersistentRepository nukiAccountPersistentRepository, 
		INukiAccountExternalRepository nukiAccountExternalRepository)
	{
		_nukiAccountExternalRepository = nukiAccountExternalRepository;
		_nukiAccountPersistentRepository = nukiAccountPersistentRepository;
	}

	public async Task<Result<NukiAccountPresentationResponse>> Handle(NukiAccountPresentationRequest externalRequestDto)
	{
		var extRes = await _nukiAccountExternalRepository.GetNukiAccount(new NukiAccountExternalRequest
		{
			Code = externalRequestDto.Code,
			ClientId = externalRequestDto.ClientId,
			RefreshToken = externalRequestDto.RefreshToken
		});

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
