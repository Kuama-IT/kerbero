using FluentResults;
using Flurl;
using Flurl.Http;
using Kerbero.Domain.Common.Errors;
using Kerbero.Domain.NukiCredentials.Errors;
using Kerbero.Domain.NukiCredentials.Models;
using Kerbero.Domain.NukiCredentials.Repositories;
using Kerbero.Infrastructure.Common.Helpers;
using Kerbero.Infrastructure.Common.Interfaces;
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

  public async Task<Result<NukiCredentialModel>> GetById(int id)
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
      return Result.Fail(new NukiCredentialNotFoundError());
    }
    catch (Exception exception)
    {
      _logger.LogError(exception, "Error while adding a NukiCredential to the database");
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

  public async Task<Result> ValidateApiToken(string apiToken)
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

  public Task<Result<NukiCredentialModel>> CreateDraft(NukiCredentialDraftModel model)
  {
    throw new NotImplementedException();
  }

  public Task<Result<NukiCredentialDraftModel>> GetDraftCredentialsByUserId(Guid userId)
  {
    throw new NotImplementedException();
  }

  public Task<Result<NukiRefreshableCredentialModel>> GetRefreshableCredential(string oAuthCode, string redirectUri)
  {
    throw new NotImplementedException();
  }

  public Task<Result<NukiCredentialModel>> ConfirmDraft(NukiCredentialDraftModel draft,
    NukiRefreshableCredentialModel model)
  {
    throw new NotImplementedException();
  }

  public Task<Result> DeleteDraftByUserId(Guid userId)
  {
    throw new NotImplementedException();
  }
}