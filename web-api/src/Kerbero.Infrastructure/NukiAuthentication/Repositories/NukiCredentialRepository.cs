using FluentResults;
using Kerbero.Domain.Common.Errors;
using Kerbero.Domain.NukiAuthentication.Errors;
using Kerbero.Domain.NukiAuthentication.Models;
using Kerbero.Domain.NukiAuthentication.Repositories;
using Kerbero.Infrastructure.Common.Interfaces;
using Kerbero.Infrastructure.NukiAuthentication.Entities;
using Kerbero.Infrastructure.NukiAuthentication.Mappers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace Kerbero.Infrastructure.NukiAuthentication.Repositories;

public class NukiCredentialRepository : INukiCredentialRepository
{
  private readonly IApplicationDbContext _dbContext;
  private readonly ILogger<NukiCredentialRepository> _logger;

  public NukiCredentialRepository(IApplicationDbContext dbContext, ILogger<NukiCredentialRepository> logger)
  {
    _logger = logger;
    _dbContext = dbContext;
  }

  public async Task<Result<NukiCredential>> Create(NukiCredential model)
  {
    try
    {
      var entity = NukiCredentialMapper.Map(model);

      _dbContext.NukiCredentials.Add(entity);
      await _dbContext.SaveChangesAsync();

      return NukiCredentialMapper.Map(entity);
    }
    catch (NotSupportedException exception)
    {
      _logger.LogError(exception, "Error while adding a NukiCredential to the database");
      return Result.Fail(new PersistentResourceNotAvailableError());
    }
    catch (DbUpdateException exception)
    {
      _logger.LogError(exception, "Error while adding a NukiCredential to the database");
      if (exception.InnerException?.InnerException is NpgsqlException &&
          exception.InnerException.InnerException.HResult >
          int.Parse(PostgresErrorCodes.IntegrityConstraintViolation) &&
          exception.InnerException.InnerException.HResult <
          int.Parse(PostgresErrorCodes.CheckViolation))
      {
        return Result.Fail(new DuplicateEntryError("Nuki account"));
      }

      return Result.Fail(new PersistentResourceNotAvailableError());
    }
    catch (Exception exception)
    {
      _logger.LogError(exception, "Error while adding a NukiCredential to the database");
      return Result.Fail(new KerberoError());
    }
  }

  public async Task<Result<NukiCredential>> GetById(int id)
  {
    try
    {
      var entity = await _dbContext.NukiCredentials.AsNoTracking().SingleAsync(it => it.Id == id);

      return Result.Ok(NukiCredentialMapper.Map(entity));
    }
    catch (NotSupportedException exception)
    {
      _logger.LogError(exception, "Error while adding a NukiCredential to the database");
      return Result.Fail(new PersistentResourceNotAvailableError());
    }
    catch (InvalidOperationException exception)
    {
      _logger.LogError(exception, "Error while adding a NukiCredential to the database");
      return Result.Fail(new UnauthorizedAccessError());
    }
    catch (Exception exception)
    {
      _logger.LogError(exception, "Error while adding a NukiCredential to the database");
      return Result.Fail(new KerberoError());
    }
  }

  public async Task<Result<NukiCredential>> Update(NukiCredential model)
  {
    if (model.Id is null)
    {
      // TODO evaluate to throw (we should not have this case, except for bad development errors)
      return Result.Fail(new InvalidParametersError(nameof(model.Id)));
    }

    try
    {
      var entity = await _dbContext.NukiCredentials.SingleAsync(it => it.Id == model.Id);

      NukiCredentialMapper.Map(entity, model: model);
      await _dbContext.SaveChangesAsync();

      return NukiCredentialMapper.Map(entity);
    }
    catch (NotSupportedException exception)
    {
      _logger.LogError(exception, "Error while adding a NukiCredential to the database");
      return Result.Fail(new PersistentResourceNotAvailableError());
    }
    catch (DbUpdateException exception)
    {
      _logger.LogError(exception, "Error while adding a NukiCredential to the database");
      if (exception.InnerException?.InnerException is NpgsqlException &&
          exception.InnerException.InnerException.HResult >
          int.Parse(PostgresErrorCodes.IntegrityConstraintViolation) &&
          exception.InnerException.InnerException.HResult <
          int.Parse(PostgresErrorCodes.CheckViolation))
      {
        return Result.Fail(new DuplicateEntryError("Nuki account"));
      }

      return Result.Fail(new PersistentResourceNotAvailableError());
    }
    catch (Exception exception)
    {
      _logger.LogError(exception, "Error while adding a NukiCredential to the database");
      return Result.Fail(new KerberoError());
    }
  }
}