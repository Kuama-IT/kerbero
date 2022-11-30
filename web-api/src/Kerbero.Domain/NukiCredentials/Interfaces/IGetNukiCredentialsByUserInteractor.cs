using FluentResults;
using Kerbero.Domain.NukiCredentials.Dtos;

namespace Kerbero.Domain.NukiCredentials.Interfaces;

public interface IGetNukiCredentialsByUserInteractor
{
  Task<Result<List<NukiCredentialDto>>> Handle(Guid userId);
}