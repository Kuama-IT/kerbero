using FluentResults;
using Kerbero.Domain.Common.Errors;
using Kerbero.Domain.NukiActions.Entities;
using Kerbero.Domain.NukiActions.Repositories;
using Kerbero.Infrastructure.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace Kerbero.Infrastructure.NukiActions.Repositories;

public class NukiSmartLockPersistentRepository: INukiSmartLockPersistentRepository
{
    private readonly IApplicationDbContext _applicationDbContext;
    private readonly ILogger<NukiSmartLockPersistentRepository> _logger;

    public NukiSmartLockPersistentRepository(IApplicationDbContext applicationDbContext,
        ILogger<NukiSmartLockPersistentRepository> logger)
    {
        _logger = logger;
        _applicationDbContext = applicationDbContext;
    }

    public async Task<Result<NukiSmartLock>> Create(NukiSmartLock nukiSmartLock)
    {
        try
        {
            var response = _applicationDbContext.NukiSmartLocks.Add(nukiSmartLock);
            await _applicationDbContext.SaveChangesAsync();
            return Result.Ok(response.Entity);
        }
        catch (NotSupportedException e)
        {
            _logger.LogError(e, "Error while adding a NukiAccount to the database");
            return Result.Fail(new PersistentResourceNotAvailableError());
        }
        catch (DbUpdateException e)
        {
            _logger.LogError(e, "Error while adding a NukiSmartLock to the database");
            if (e.InnerException?.InnerException is NpgsqlException && e.InnerException.InnerException.HResult >
                int.Parse(PostgresErrorCodes.IntegrityConstraintViolation) && e.InnerException.InnerException.HResult <
                int.Parse(PostgresErrorCodes.CheckViolation))
            {
                return Result.Fail(new DuplicateEntryError("Nuki SmartLock"));
            }
            return Result.Fail(new PersistentResourceNotAvailableError());
        }
        catch(Exception e)
        {
            _logger.LogError(e, "Error while adding a NukiAccount to the database");
            return Result.Fail(new KerberoError());
        }
    }

    public Result<NukiSmartLock> GetById(int smartLockId)
    {
        throw new NotImplementedException();
    }
}