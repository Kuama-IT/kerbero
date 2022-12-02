using FluentResults;
using Flurl;
using Flurl.Http;
using Kerbero.Domain.Common.Errors;
using Kerbero.Domain.NukiCredentials.Errors;
using Kerbero.Domain.NukiCredentials.Models;
using Kerbero.Domain.NukiCredentials.Repositories;
using Kerbero.Infrastructure.Common.Helpers;
using Kerbero.Infrastructure.Common.Interfaces;
using Kerbero.Infrastructure.NukiCredentials.Dtos;
using Kerbero.Infrastructure.NukiCredentials.Entities;
using Kerbero.Infrastructure.NukiCredentials.Mappers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace Kerbero.Infrastructure.NukiCredentials.Repositories;

public class NukiCredentialRepository : INukiCredentialRepository
{
  private readonly IApplicationDbContext _dbContext;
  private readonly ILogger<NukiCredentialRepository> _logger;
  private readonly IConfiguration _configuration;
  private readonly NukiSafeHttpCallHelper _nukiSafeHttpCallHelper;

  public NukiCredentialRepository(IApplicationDbContext dbContext, ILogger<NukiCredentialRepository> logger,
    NukiSafeHttpCallHelper nukiSafeHttpCallHelper, IConfiguration configuration)
  {
    _logger = logger;
    _nukiSafeHttpCallHelper = nukiSafeHttpCallHelper;
    _configuration = configuration;
    _dbContext = dbContext;
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
    var result = await _nukiSafeHttpCallHelper.Handle(() =>
      _configuration["NUKI_DOMAIN"]
        .AppendPathSegment("account")
        .WithOAuthBearerToken(apiToken)
        .GetAsync()
    );

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
    var result = await _nukiSafeHttpCallHelper.Handle(
      async () => await _configuration["NUKI_DOMAIN"]
        .AppendPathSegment("oauth")
        .AppendPathSegment("token")
        .PostUrlEncodedAsync(new
        {
          client_id = _configuration["NUKI_CLIENT_ID"],
          client_secret = _configuration["NUKI_CLIENT_SECRET"],
          grant_type = "authorization_code",
          code = oAuthCode,
          redirect_uri = redirectUri
        })
        .ReceiveJson<NukiOAuthResponseDto>());

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
}