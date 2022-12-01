using FluentResults;
using Kerbero.Domain.NukiCredentials.Models;

namespace Kerbero.Domain.Common.Interfaces;

public interface IKerberoConfigurationRepository
{
  Task<Result<NukiApiConfigurationModel>> GetApiDefinition();
}