using FluentResults;
using Kerbero.Domain.Common.Errors;
using Kerbero.Domain.NukiAuthentication.Entities;
using Kerbero.Domain.NukiAuthentication.Errors.CreateNukiAccountErrors;
using Kerbero.Domain.NukiAuthentication.Repositories;
using Kerbero.Infrastructure.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace Kerbero.Infrastructure.NukiAuthentication.Repositories;

public class NukiAccountPersistentRepository: INukiAccountPersistentRepository
{
	private readonly IApplicationDbContext _dbContext;
	private readonly ILogger<NukiAccountPersistentRepository> _logger;

	public NukiAccountPersistentRepository(IApplicationDbContext dbContext, ILogger<NukiAccountPersistentRepository> logger)
	{
		_logger = logger;
		_dbContext = dbContext;
	}

	public async Task<Result<NukiAccount>> Create(NukiAccount nukiAccount)
	{
		try
		{
			var res = _dbContext.NukiAccounts.Add(nukiAccount);
			await _dbContext.SaveChangesAsync();
			return Result.Ok(res.Entity);
		}
		catch (NotSupportedException e)
		{
			_logger.LogError(e, "Error while adding a NukiAccount to the database");
			return Result.Fail(new PersistentResourceNotAvailableError());
		}
		catch (DbUpdateException e)
		{
			_logger.LogError(e, "Error while adding a NukiAccount to the database");
			if (e.InnerException?.InnerException is NpgsqlException && e.InnerException.InnerException.HResult >
			    int.Parse(PostgresErrorCodes.IntegrityConstraintViolation) && e.InnerException.InnerException.HResult <
			    int.Parse(PostgresErrorCodes.CheckViolation))
			{
				return Result.Fail(new DuplicateEntryError("Nuki account"));
			}
			return Result.Fail(new PersistentResourceNotAvailableError());
		}
		catch(Exception e)
		{
			_logger.LogError(e, "Error while adding a NukiAccount to the database");
			return Result.Fail(new KerberoError());
		}
	}
	
	public Result<NukiAccount> GetAccount(int kerberoAccountId)
	{
		try
		{
			var res = _dbContext.NukiAccounts.First(c => c.Id == kerberoAccountId);
			return Result.Ok(res);
		}
		catch (NotSupportedException e)
		{
			_logger.LogError(e, "Error while adding a NukiAccount to the database");
			return Result.Fail(new PersistentResourceNotAvailableError());
		}
		catch(InvalidOperationException e)
		{
			_logger.LogError(e, "Error while adding a NukiAccount to the database");
			return Result.Fail(new UnauthorizedAccessError());
		}
		catch(Exception e)
		{
			_logger.LogError(e, "Error while adding a NukiAccount to the database");
			return Result.Fail(new KerberoError());
		}
	}

	public async Task<Result<NukiAccount>> Update(NukiAccount nukiAccount)
	{
		try
		{
			var res = _dbContext.NukiAccounts.Update(nukiAccount);
			await _dbContext.SaveChangesAsync();
			return Result.Ok(res.Entity);
		}
		catch (NotSupportedException e)
		{
			_logger.LogError(e, "Error while adding a NukiAccount to the database");
			return Result.Fail(new PersistentResourceNotAvailableError());
		}
		catch (DbUpdateException e)
		{
			_logger.LogError(e, "Error while adding a NukiAccount to the database");
			if (e.InnerException?.InnerException is NpgsqlException && e.InnerException.InnerException.HResult >
			    int.Parse(PostgresErrorCodes.IntegrityConstraintViolation) && e.InnerException.InnerException.HResult <
			    int.Parse(PostgresErrorCodes.CheckViolation))
			{
				return Result.Fail(new DuplicateEntryError("Nuki account"));
			}
			return Result.Fail(new PersistentResourceNotAvailableError());
		}
		catch(Exception e)
		{
			_logger.LogError(e, "Error while adding a NukiAccount to the database");
			return Result.Fail(new KerberoError());
		}
	}
}
