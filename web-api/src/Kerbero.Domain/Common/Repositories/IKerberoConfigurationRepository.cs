using Kerbero.Domain.NukiCredentials.Models;
using FluentResults;

namespace Kerbero.Domain.Common.Repositories;

/// <summary>
/// The role of this repository is bridge any env / json / ... configuration over to domain 
/// </summary>
public interface IKerberoConfigurationRepository
{
  Task<Result<NukiApiConfigurationModel>> GetNukiApiDefinition();
}