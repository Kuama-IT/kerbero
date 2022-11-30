using FluentResults;
using Kerbero.Domain.NukiCredentials.Dtos;

namespace Kerbero.Domain.NukiCredentials.Interfaces;

public interface ICreateNukiCredentialInteractor
{
  Task<Result<NukiCredentialDto>> Handle(Guid userId, string token);
}