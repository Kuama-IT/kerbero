using FluentResults;
using Kerbero.Common.Entities;
using Kerbero.Common.Errors;
using Kerbero.Common.Errors.CreateNukiAccountErrors;
using Kerbero.Common.Repositories;
using Kerbero.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Kerbero.Infrastructure.Repositories;

public class NukiPersistentAccountRepository: INukiPersistentAccountRepository
{
	private readonly IApplicationDbContext _dbContext;

	public NukiPersistentAccountRepository(IApplicationDbContext dbContext)
	{
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
		catch (NotSupportedException)
		{
			return Result.Fail(new PersistentResourceNotAvailableError());
		}
		catch (DbUpdateException e)
		{
			if (e.InnerException?.InnerException is NpgsqlException && e.InnerException.InnerException.HResult >
			    int.Parse(PostgresErrorCodes.IntegrityConstraintViolation) && e.InnerException.InnerException.HResult <
			    int.Parse(PostgresErrorCodes.CheckViolation))
			{
				return Result.Fail(new DuplicateEntryError("Nuki account"));
			}
			return Result.Fail(new PersistentResourceNotAvailableError());
		}
		catch
		{
			return Result.Fail(new KerberoError());
		}
	}
}
