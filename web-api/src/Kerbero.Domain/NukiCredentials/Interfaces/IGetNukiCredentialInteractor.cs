using FluentResults;
using Kerbero.Domain.NukiCredentials.Dtos;

namespace Kerbero.Domain.NukiCredentials.Interfaces;

public interface IGetNukiCredentialInteractor
{
  Task<Result<NukiCredentialDto>> Handle(int nukiCredentialId);
}