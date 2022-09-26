using FluentResults;
using Kerbero.Common.Entities;
using Kerbero.Common.Errors;
using Kerbero.Common.Errors.CreateNukiAccountErrors;
using Kerbero.Common.Repositories;
using Kerbero.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace Kerbero.Infrastructure.Repositories;

public class NukiPersistentAccountRepository: INukiPersistentAccountRepository
{
	private readonly IApplicationDbContext _dbContext;
	private readonly ILogger<NukiPersistentAccountRepository> _logger;

	public NukiPersistentAccountRepository(IApplicationDbContext dbContext, ILogger<NukiPersistentAccountRepository> logger)
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
}
