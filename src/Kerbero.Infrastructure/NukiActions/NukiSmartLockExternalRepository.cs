using FluentResults;
using Kerbero.Domain.Common.Errors;
using Kerbero.Domain.NukiActions.Models;
using Kerbero.Domain.NukiActions.Repositories;
using Kerbero.Domain.NukiAuthentication.Entities;
using Kerbero.Infrastructure.NukiAuthentication.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Kerbero.Infrastructure.NukiActions;

public class NukiSmartLockExternalRepository: INukiSmartLockExternalRepository
{
    private readonly NukiExternalOptions _options;
    private readonly ILogger<NukiSmartLockExternalRepository> _logger;
    private NukiAccount? NukiAccount { get; set; }

    public bool IsAuthenticated => NukiAccount is not null;

    public NukiSmartLockExternalRepository(IOptions<NukiExternalOptions> options, 
        ILogger<NukiSmartLockExternalRepository> logger)
    {
        _options = options.Value;
        _logger = logger;
    }
    
    public Task<Result<List<NukiSmartLockExternalResponseDto>>> GetNukiSmartLockList()
    {
        throw new NotImplementedException();
    }

    public void Authenticate(NukiAccount nukiAccount)
    {
        NukiAccount = nukiAccount;
    }
}