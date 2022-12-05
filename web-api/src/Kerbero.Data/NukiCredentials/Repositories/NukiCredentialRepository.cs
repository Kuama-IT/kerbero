using FluentResults;
using Kerbero.Data.Common.Helpers;
using Kerbero.Data.Common.Interfaces;
using Kerbero.Data.NukiCredentials.Entities;
using Kerbero.Data.NukiCredentials.Mappers;
using Kerbero.Domain.Common.Errors;
using Kerbero.Domain.NukiCredentials.Errors;
using Kerbero.Domain.NukiCredentials.Models;
using Kerbero.Domain.NukiCredentials.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace Kerbero.Data.NukiCredentials.Repositories;

public class NukiCredentialRepository : INukiCredentialRepository
{
  private readonly IApplicationDbContext _dbContext;
  private readonly ILogger<NukiCredentialRepository> _logger;
  private readonly NukiRestApiClient _nukiRestApiClient;

  public NukiCredentialRepository(IApplicationDbContext dbContext, ILogger<NukiCredentialRepository> logger,
    NukiRestApiClient nukiRestApiClient)
  {
    _logger = logger;
    _nukiRestApiClient = nukiRestApiClient;
    _dbContext = dbContext;
  }

  public async Task<Result<string>> GetNukiAccountEmail(string token)
  {
    var nukiAccountResult = await _nukiRestApiClient
      .GetAccount(token);

    if (nukiAccountResult.IsFailed)
    {
      return nukiAccountResult.ToResult();
    }

    return nukiAccountResult.Value.Email;
  }

  public async Task<Result<NukiCredentialModel>> GetRefreshedCredential(NukiCredentialModel model)
  {
    if (string.IsNullOrEmpty(model.Token))
    {
      return Result.Fail(new NukiCredentialInvalidTokenError());
    }

    try
    {
      var entity = await _dbContext.NukiCredentials.AsNoTracking().SingleAsync(it => it.Id == model.Id);

      if (string.IsNullOrEmpty(entity.RefreshToken))
      {
        return Result.Fail(new NukiCredentialInvalidTokenError());
      }

      var responseResult = await _nukiRestApiClient.RefreshToken(entity.RefreshToken);

      if (responseResult.IsFailed)
      {
        return responseResult.ToResult();
      }

      var entityToUpdate = await _dbContext.NukiCredentials.SingleAsync(it => it.Id == model.Id);

      NukiCredentialMapper.Map(entity: entityToUpdate, response: responseResult.Value);

      await _dbContext.SaveChangesAsync();

      return NukiCredentialMapper.Map(entityToUpdate);
    }
    catch (NotSupportedException exception)
    {
      _logger.LogError(exception, "Error while retrieving a NukiCredential from the database");
      return Result.Fail(new PersistentResourceNotAvailableError());
    }
    catch (InvalidOperationException exception)
    {
      _logger.LogError(exception, "Error while retrieving a NukiCredential from the database");
      return Result.Fail(new NukiCredentialNotFoundError());
    }
    catch (Exception exception)
    {
      _logger.LogError(exception, "Error while retrieving a NukiCredential from the database");
      return Result.Fail(new KerberoError());
    }
  }

  public async Task<Result<NukiCredentialModel>> Create(NukiCredentialModel model, Guid userId)
  {
    try
    {
      var entity = NukiCredentialMapper.Map(model);
      entity.UserId = userId;

      _dbContext.NukiCredentials.Add(entity);

      var pivotEntity = new UserNukiCredentialEntity
      {
        NukiCredential = entity,
        UserId = userId,
      };

      _dbContext.UserNukiCredentials.Add(pivotEntity);

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
        return Result.Fail(new DuplicateEntryError("Nuki credential"));
      }

      return Result.Fail(new PersistentResourceNotAvailableError());
    }
    catch (Exception exception)
    {
      _logger.LogError(exception, "Error while adding a NukiCredential to the database");
      return Result.Fail(new KerberoError());
    }
  }

  public async Task<Result<NukiCredentialModel>> GetById(int id)
  {
    try
    {
      var entity = await _dbContext.NukiCredentials.AsNoTracking().SingleAsync(it => it.Id == id);

      return Result.Ok(NukiCredentialMapper.Map(entity));
    }
    catch (NotSupportedException exception)
    {
      _logger.LogError(exception, "Error while retrieving a NukiCredential from the database");
      return Result.Fail(new PersistentResourceNotAvailableError());
    }
    catch (InvalidOperationException exception)
    {
      _logger.LogError(exception, "Error while retrieving a NukiCredential from the database");
      return Result.Fail(new NukiCredentialNotFoundError());
    }
    catch (Exception exception)
    {
      _logger.LogError(exception, "Error while retrieving a NukiCredential from the database");
      return Result.Fail(new KerberoError());
    }
  }

  public async Task<Result<List<NukiCredentialModel>>> GetAllByUserId(Guid userId)
  {
    var entities = await _dbContext.UserNukiCredentials
      .AsNoTracking()
      .Where(e => e.UserId == userId)
      .Include(e => e.NukiCredential)
      .Select(e => e.NukiCredential!)
      .ToListAsync();

    return NukiCredentialMapper.Map(entities);
  }

  public async Task<Result> LinkToUser(int nukiCredentialId, Guid userId)
  {
    var entity = new UserNukiCredentialEntity
    {
      NukiCredentialId = nukiCredentialId,
      UserId = userId,
    };

    _dbContext.UserNukiCredentials.Add(entity);

    await _dbContext.SaveChangesAsync();

    return Result.Ok();
  }

  public async Task<Result> ValidateNotRefreshableApiToken(string apiToken)
  {
    var result = await _nukiRestApiClient.CheckTokenValidity(apiToken);

    if (result.IsFailed)
    {
      return Result.Fail(new NukiCredentialInvalidTokenError());
    }

    return Result.Ok();
  }

  public async Task<Result<NukiCredentialModel>> CreateDraft(NukiCredentialDraftModel model)
  {
    try
    {
      var entity = NukiCredentialMapper.Map(model);

      _dbContext.NukiCredentials.Add(entity);

      var pivotEntity = new UserNukiCredentialEntity
      {
        NukiCredential = entity,
        UserId = model.UserId,
      };

      _dbContext.UserNukiCredentials.Add(pivotEntity);

      await _dbContext.SaveChangesAsync();

      return NukiCredentialMapper.Map(entity);
    }
    catch (NotSupportedException exception)
    {
      _logger.LogError(exception, "Error while adding a NukiCredential (draft) to the database");
      return Result.Fail(new PersistentResourceNotAvailableError());
    }
    catch (DbUpdateException exception)
    {
      _logger.LogError(exception, "Error while adding a NukiCredential (draft) to the database");
      if (exception.InnerException?.InnerException is NpgsqlException &&
          exception.InnerException.InnerException.HResult >
          int.Parse(PostgresErrorCodes.IntegrityConstraintViolation) &&
          exception.InnerException.InnerException.HResult <
          int.Parse(PostgresErrorCodes.CheckViolation))
      {
        return Result.Fail(new DuplicateEntryError("Nuki credential"));
      }

      return Result.Fail(new PersistentResourceNotAvailableError());
    }
    catch (Exception exception)
    {
      _logger.LogError(exception, "Error while adding a NukiCredential (draft) to the database");
      return Result.Fail(new KerberoError());
    }
  }

  public async Task<Result<NukiCredentialDraftModel>> GetDraftCredentialsByUserId(Guid userId)
  {
    var entity = await _dbContext.UserNukiCredentials
      .AsNoTracking()
      .Where(e => e.UserId == userId)
      .Include(e => e.NukiCredential)
      .Select(e => e.NukiCredential!)
      .FirstAsync(c => c.IsDraft);


    return NukiCredentialMapper.MapAsDraft(entity);
  }

  public async Task<Result<NukiRefreshableCredentialModel>> GetRefreshableCredential(string oAuthCode,
    string redirectUri)
  {
    var result = await _nukiRestApiClient.AuthenticateWithCode(oAuthCode, redirectUri);

    if (result.IsFailed)
    {
      return result.ToResult();
    }

    return NukiCredentialMapper.Map(result.Value, DateTime.UtcNow);
  }

  public async Task<Result<NukiCredentialModel>> ConfirmDraft(NukiCredentialDraftModel draft,
    NukiRefreshableCredentialModel model)
  {
    try
    {
      var entity = await _dbContext.NukiCredentials.SingleAsync(it => it.Id == draft.Id);

      NukiCredentialMapper.Map(entity, model);

      var emailResult = await GetNukiAccountEmail(model.Token);

      if (emailResult.IsFailed)
      {
        return emailResult.ToResult();
      }

      entity.NukiEmail = emailResult.Value;
      await _dbContext.SaveChangesAsync();
      return NukiCredentialMapper.Map(entity);
    }
    catch (NotSupportedException exception)
    {
      _logger.LogError(exception, "Error while updating a NukiCredential (draft to confirmed) to the database");
      return Result.Fail(new PersistentResourceNotAvailableError());
    }
    catch (DbUpdateException exception)
    {
      _logger.LogError(exception, "Error while updating a NukiCredential (draft to confirmed) to the database");
      if (exception.InnerException?.InnerException is NpgsqlException &&
          exception.InnerException.InnerException.HResult >
          int.Parse(PostgresErrorCodes.IntegrityConstraintViolation) &&
          exception.InnerException.InnerException.HResult <
          int.Parse(PostgresErrorCodes.CheckViolation))
      {
        return Result.Fail(new DuplicateEntryError("Nuki credential (draft to confirmed)"));
      }

      return Result.Fail(new PersistentResourceNotAvailableError());
    }
    catch (Exception exception)
    {
      _logger.LogError(exception, "Error while updating a NukiCredential (draft to confirmed) to the database");
      return Result.Fail(new KerberoError());
    }
  }

  public async Task<Result> DeleteDraftByUserId(Guid userId)
  {
    var entities = await _dbContext.UserNukiCredentials
      .AsNoTracking()
      .Where(e => e.UserId == userId)
      .Include(e => e.NukiCredential)
      .Select(e => e.NukiCredential!)
      .Where(c => c.IsDraft)
      .ToListAsync();

    _dbContext.NukiCredentials.RemoveRange(entities);
    await _dbContext.SaveChangesAsync();

    return Result.Ok();
  }

  public async Task<Result<NukiCredentialModel>> DeleteById(int nukiCredentialId)
  {
    try
    {
      var entity =
        await _dbContext.NukiCredentials.SingleAsync(credentialEntity => credentialEntity.Id == nukiCredentialId);
      _dbContext.NukiCredentials.Remove(entity);
      await _dbContext.SaveChangesAsync();
      return NukiCredentialMapper.Map(entity);
    }
    catch (NotSupportedException exception)
    {
      _logger.LogError(exception, "Error while deleting a NukiCredential from the database");
      return Result.Fail(new PersistentResourceNotAvailableError());
    }
    catch (InvalidOperationException exception)
    {
      _logger.LogError(exception, "Error while deleting a NukiCredential from the database");
      return Result.Fail(new NukiCredentialNotFoundError());
    }
    catch (Exception exception)
    {
      _logger.LogError(exception, "Error while deleting a NukiCredential from the database");
      return Result.Fail(new KerberoError());
    }
  }
}