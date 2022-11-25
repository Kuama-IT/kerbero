using FluentResults;
using Kerbero.Domain.NukiAuthentication.Dtos;
using Kerbero.Domain.NukiAuthentication.Interfaces;
using Kerbero.Domain.NukiAuthentication.Mappers;
using Kerbero.Domain.NukiAuthentication.Models;
using Kerbero.Domain.NukiAuthentication.Repositories;

namespace Kerbero.Domain.NukiAuthentication.Interactors;

public class GetNukiCredentialsByUserInteractor : IGetNukiCredentialsByUserInteractor
{
  private readonly INukiCredentialRepository _nukiCredentialRepository;
  private readonly INukiOAuthRepository _nukiOAuthRepository;

  public GetNukiCredentialsByUserInteractor(INukiCredentialRepository nukiCredentialRepository,
    INukiOAuthRepository nukiOAuthRepository)
  {
    _nukiCredentialRepository = nukiCredentialRepository;
    _nukiOAuthRepository = nukiOAuthRepository;
  }

  public async Task<Result<List<NukiCredentialDto>>> Handle(GetNukiCredentialsByUserInteractorParams request)
  {
    var nukiCredentialsResult = await _nukiCredentialRepository.GetAllByUserId(request.UserId);

    if (nukiCredentialsResult.IsFailed)
    {
      return Result.Fail(nukiCredentialsResult.Errors);
    }

    Func<NukiCredential, Task<Result<NukiCredentialDto>>> action = async (nukiCredential) =>
    {
      var isAuthenticationTokenExpired = nukiCredential.ExpireDate < DateTime.Now.ToUniversalTime();

      if (!isAuthenticationTokenExpired)
      {
        return NukiCredentialMapper.Map(nukiCredential);
      }

      // Refresh token
      var accountResult = await _nukiOAuthRepository.RefreshNukiOAuth(new NukiOAuthRequest
      {
        ClientId = nukiCredential.ClientId,
        RefreshToken = nukiCredential.RefreshToken
      });

      if (accountResult.IsFailed)
      {
        return Result.Fail(accountResult.Errors);
      }

      var updateAccountResult = await _nukiCredentialRepository.Update(accountResult.Value);

      if (updateAccountResult.IsFailed)
      {
        return Result.Fail(updateAccountResult.Errors);
      }

      return NukiCredentialMapper.Map(updateAccountResult.Value);
    };

    var nukiCredentialsDtos = new List<NukiCredentialDto>();
    foreach (var nukiCredential in nukiCredentialsResult.Value)
    {
      var nukiCredentialResult = await action.Invoke(nukiCredential);

      if (nukiCredentialResult.IsFailed)
      {
        return Result.Fail(nukiCredentialResult.Errors);
      }
      
      nukiCredentialsDtos.Add(nukiCredentialResult.Value);
    }

    return nukiCredentialsDtos;
  }
}