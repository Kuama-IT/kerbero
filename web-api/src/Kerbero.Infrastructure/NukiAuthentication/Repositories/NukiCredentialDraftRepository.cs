using FluentResults;
using Kerbero.Domain.Common.Errors;
using Kerbero.Domain.NukiAuthentication.Errors;
using Kerbero.Domain.NukiAuthentication.Models;
using Kerbero.Domain.NukiAuthentication.Repositories;
using Kerbero.Infrastructure.Common.Interfaces;
using Kerbero.Infrastructure.NukiAuthentication.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace Kerbero.Infrastructure.NukiAuthentication.Repositories;

public class NukiCredentialDraftRepository : INukiCredentialDraftRepository
{
  private readonly IApplicationDbContext _dbContext;
  // TODO use logger
  private readonly ILogger<NukiCredentialDraftRepository> _logger;

  public NukiCredentialDraftRepository(IApplicationDbContext dbContext, ILogger<NukiCredentialDraftRepository> logger)
  {
    _logger = logger;
    _dbContext = dbContext;
  }

  public async Task<Result> Create(NukiCredentialDraft nukiCredentialDraft)
  {
    _dbContext.NukiCredentialDrafts.Add(new NukiCredentialDraftEntity()
    {
      UserId = nukiCredentialDraft.UserId,
      ClientId = nukiCredentialDraft.ClientId,
      RedirectUrl = nukiCredentialDraft.RedirectUrl
    });

    // TODO handle exceptions
    await _dbContext.SaveChangesAsync();

    return Result.Ok();
  }

  public async Task<Result<NukiCredentialDraft>> GetByClientId(string clientId)
  {
    // TODO handle exceptions
    var entity = await _dbContext.NukiCredentialDrafts
      .AsNoTracking()
      .SingleAsync(it => it.ClientId == clientId);

    return Result.Ok(new NukiCredentialDraft(
      ClientId: entity.ClientId,
      RedirectUrl: entity.RedirectUrl,
      UserId: entity.UserId
    ));
  }

  public async Task<Result> DeleteByClientId(string clientId)
  {
    var entity = await _dbContext.NukiCredentialDrafts
      .SingleAsync(it => it.ClientId == clientId);

    _dbContext.NukiCredentialDrafts.Remove(entity);
    // TODO handle exceptions
    await _dbContext.SaveChangesAsync();

    return Result.Ok();
  }
}