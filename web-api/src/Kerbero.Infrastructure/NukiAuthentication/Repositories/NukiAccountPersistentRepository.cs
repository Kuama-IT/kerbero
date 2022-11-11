using FluentResults;
using Kerbero.Domain.Common.Errors;
using Kerbero.Domain.NukiAuthentication.Entities;
using Kerbero.Domain.NukiAuthentication.Errors;
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
		catch (NotSupportedException exception)
		{
			_logger.LogError(exception, "Error while adding a NukiAccount to the database");
			return Result.Fail(new PersistentResourceNotAvailableError());
		}
		catch (DbUpdateException exception)
		{
			_logger.LogError(exception, "Error while adding a NukiAccount to the database");
			if (exception.InnerException?.InnerException is NpgsqlException && exception.InnerException.InnerException.HResult >
			    int.Parse(PostgresErrorCodes.IntegrityConstraintViolation) && exception.InnerException.InnerException.HResult <
			    int.Parse(PostgresErrorCodes.CheckViolation))
			{
				return Result.Fail(new DuplicateEntryError("Nuki account"));
			}
			return Result.Fail(new PersistentResourceNotAvailableError());
		}
		catch(Exception exception)
		{
			_logger.LogError(exception, "Error while adding a NukiAccount to the database");
			return Result.Fail(new KerberoError());
		}
	}
	
	public async Task<Result<NukiAccount>> GetById(int id)
	{
		try
		{
			var res = await _dbContext.NukiAccounts.FindAsync(id);
			return res is null ? Result.Fail(new NukiAccountNotFoundError()) : Result.Ok(res);
		}
		catch (NotSupportedException exception)
		{
			_logger.LogError(exception, "Error while adding a NukiAccount to the database");
			return Result.Fail(new PersistentResourceNotAvailableError());
		}
		catch(InvalidOperationException exception)
		{
			_logger.LogError(exception, "Error while adding a NukiAccount to the database");
			return Result.Fail(new UnauthorizedAccessError());
		}
		catch(Exception exception)
		{
			_logger.LogError(exception, "Error while adding a NukiAccount to the database");
			return Result.Fail(new KerberoError());
		}
	}

	public async Task<Result<NukiAccount>> Update(NukiAccount nukiAccount)
	{
		try
		{
			var res = await GetById(nukiAccount.Id);
			if (res.IsFailed)
			{
				return Result.Fail(new UnauthorizedAccessError());
			}

			var account = res.Value;
			account.Token = nukiAccount.Token;
			account.ExpiryDate = nukiAccount.ExpiryDate;
			account.RefreshToken = nukiAccount.RefreshToken;
			account.TokenType = nukiAccount.TokenType;
			account.TokenExpiringTimeInSeconds = nukiAccount.TokenExpiringTimeInSeconds;
			await _dbContext.SaveChangesAsync();
			return Result.Ok(account);
		}
		catch (NotSupportedException exception)
		{
			_logger.LogError(exception, "Error while adding a NukiAccount to the database");
			return Result.Fail(new PersistentResourceNotAvailableError());
		}
		catch (DbUpdateException exception)
		{
			_logger.LogError(exception, "Error while adding a NukiAccount to the database");
			if (exception.InnerException?.InnerException is NpgsqlException && exception.InnerException.InnerException.HResult >
			    int.Parse(PostgresErrorCodes.IntegrityConstraintViolation) && exception.InnerException.InnerException.HResult <
			    int.Parse(PostgresErrorCodes.CheckViolation))
			{
				return Result.Fail(new DuplicateEntryError("Nuki account"));
			}
			return Result.Fail(new PersistentResourceNotAvailableError());
		}
		catch(Exception exception)
		{
			_logger.LogError(exception, "Error while adding a NukiAccount to the database");
			return Result.Fail(new KerberoError());
		}
	}
}
