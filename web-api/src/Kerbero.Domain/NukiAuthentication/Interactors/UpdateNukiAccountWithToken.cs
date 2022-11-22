using FluentResults;
using Kerbero.Domain.NukiAuthentication.Interfaces;
using Kerbero.Domain.NukiAuthentication.Mappers;
using Kerbero.Domain.NukiAuthentication.Models.ExternalRequests;
using Kerbero.Domain.NukiAuthentication.Models.PresentationRequests;
using Kerbero.Domain.NukiAuthentication.Models.PresentationResponses;
using Kerbero.Domain.NukiAuthentication.Repositories;

namespace Kerbero.Domain.NukiAuthentication.Interactors;

public class UpdateNukiAccountWithToken: IUpdateNukiAccountWithToken
{
	private readonly INukiAccountPersistentRepository _nukiAccountPersistentRepository;
	private readonly INukiAccountExternalRepository _nukiAccountExternalRepository;

	public UpdateNukiAccountWithToken(INukiAccountPersistentRepository nukiAccountPersistentRepository, 
		INukiAccountExternalRepository nukiAccountExternalRepository)
	{
		_nukiAccountExternalRepository = nukiAccountExternalRepository;
		_nukiAccountPersistentRepository = nukiAccountPersistentRepository;
	}

	public async Task<Result<UpdateNukiAccountPresentationResponse>> Handle(UpdateNukiAccountPresentationRequest externalRequestDto)
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
		var persistentResult = await _nukiAccountPersistentRepository.Update(nukiAccount);

		return persistentResult.IsSuccess ? // isSuccess
			Result.Ok(NukiAccountMapper.MapToPresentation(persistentResult.Value)) : Result.Fail(persistentResult.Errors);

	}
}
