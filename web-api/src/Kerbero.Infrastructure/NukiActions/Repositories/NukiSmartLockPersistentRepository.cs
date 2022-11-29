using FluentResults;
using Kerbero.Domain.Common.Errors;
using Kerbero.Domain.NukiActions.Entities;
using Kerbero.Domain.NukiActions.Errors;
using Kerbero.Domain.NukiActions.Repositories;
using Kerbero.Domain.SmartLocks.Errors;
using Kerbero.Infrastructure.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace Kerbero.Infrastructure.NukiActions.Repositories;

public class NukiSmartLockPersistentRepository : INukiSmartLockPersistentRepository
{
  private readonly IApplicationDbContext _applicationDbContext;
  private readonly ILogger<NukiSmartLockPersistentRepository> _logger;

  public NukiSmartLockPersistentRepository(IApplicationDbContext applicationDbContext,
    ILogger<NukiSmartLockPersistentRepository> logger)
  {
    _logger = logger;
    _applicationDbContext = applicationDbContext;
  }

  public async Task<Result<NukiSmartLockEntity>> Create(NukiSmartLockEntity nukiSmartLockEntity)
  {
    try
    {
      var response = _applicationDbContext.NukiSmartLocks.Add(nukiSmartLockEntity);
      await _applicationDbContext.SaveChangesAsync();
      return Result.Ok(response.Entity);
    }
    catch (NotSupportedException exception)
    {
      _logger.LogError(exception, "Error while adding a NukiAccount to the database");
      return Result.Fail(new PersistentResourceNotAvailableError());
    }
    catch (DbUpdateException exception)
    {
      _logger.LogError(exception, "Error while adding a NukiSmartLock to the database");
      if (exception.InnerException?.InnerException is NpgsqlException &&
          exception.InnerException.InnerException.HResult >
          int.Parse(PostgresErrorCodes.IntegrityConstraintViolation) &&
          exception.InnerException.InnerException.HResult <
          int.Parse(PostgresErrorCodes.CheckViolation))
      {
        return Result.Fail(new DuplicateEntryError("Nuki SmartLock"));
      }

      return Result.Fail(new PersistentResourceNotAvailableError());
    }
    catch (Exception exception)
    {
      _logger.LogError(exception, "Error while adding a NukiAccount to the database");
      return Result.Fail(new KerberoError());
    }
  }

  public async Task<Result<NukiSmartLockEntity>> GetById(int smartLockId)
  {
    try
    {
      var smartLock = await _applicationDbContext.NukiSmartLocks
        .FindAsync(smartLockId);
      return smartLock is null
        ? Result.Fail(new SmartLockNotFoundError(smartLockId.ToString()))
        : Result.Ok(smartLock);
    }
    catch (Exception exception)
    {
      _logger.LogError(exception, "Error while retrieving a NukiAccount to the database");
      return Result.Fail(new UnknownExternalError());
    }
  }
}