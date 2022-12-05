using FluentResults;
using Kerbero.Domain.NukiCredentials.Interfaces;
using Kerbero.Domain.NukiCredentials.Models;
using Kerbero.Domain.NukiCredentials.Repositories;

namespace Kerbero.Domain.NukiCredentials.Interactors;

public class GetNukiCredentialsByUserInteractor : IGetNukiCredentialsByUserInteractor
{
  private readonly INukiCredentialRepository _nukiCredentialRepository;

  public GetNukiCredentialsByUserInteractor(INukiCredentialRepository nukiCredentialRepository)
  {
    _nukiCredentialRepository = nukiCredentialRepository;
  }

  public async Task<Result<UserNukiCredentialsModel>> Handle(Guid userId)
  {
    var nukiCredentialsResult = await _nukiCredentialRepository.GetAllByUserId(userId);

    if (nukiCredentialsResult.IsFailed)
    {
      return Result.Fail(nukiCredentialsResult.Errors);
    }

    var refreshedNukiCredentials = new List<NukiCredentialModel>();
    var failedNukiCredentials = new List<(NukiCredentialModel, List<IError>)>();

    foreach (var nukiCredentialModel in nukiCredentialsResult.Value)
    {
      if (nukiCredentialModel.IsRefreshable)
      {
        var refreshedNukiCredentialResult = await _nukiCredentialRepository.GetRefreshedCredential(nukiCredentialModel);
        if (refreshedNukiCredentialResult.IsSuccess)
        {
          refreshedNukiCredentials.Add(refreshedNukiCredentialResult.Value);
        }
        else
        {
          failedNukiCredentials.Add((nukiCredentialModel, refreshedNukiCredentialResult.Errors));
        }
      }
      else
      {
        refreshedNukiCredentials.Add(nukiCredentialModel);
      }
    }

    return new UserNukiCredentialsModel(
      NukiCredentials: refreshedNukiCredentials,
      OutdatedCredentials: failedNukiCredentials
    );
  }
}