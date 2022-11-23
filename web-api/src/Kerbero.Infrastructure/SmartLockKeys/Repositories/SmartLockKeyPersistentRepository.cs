using FluentResults;
using Kerbero.Domain.Common.Errors;
using Kerbero.Domain.SmartLockKeys.Entities;
using Kerbero.Domain.SmartLockKeys.Repositories;
using Kerbero.Infrastructure.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace Kerbero.Infrastructure.SmartLockKeys.Repositories;

public class SmartLockKeyPersistentRepository: ISmartLockKeyPersistentRepository
{
	private readonly ILogger<SmartLockKeyPersistentRepository> _logger;
	private readonly IApplicationDbContext _applicationDbContext;

	public SmartLockKeyPersistentRepository(ILogger<SmartLockKeyPersistentRepository> logger, IApplicationDbContext applicationDbContext)
	{
		_logger = logger;
		_applicationDbContext = applicationDbContext;
	}

	public async Task<Result<SmartLockKey>> Create(SmartLockKey smartLockKey)
	{
		try
		{
			var smartLockKeyEntityEntry = _applicationDbContext.SmartLockKeys.Add(smartLockKey);
			await _applicationDbContext.SaveChangesAsync();
			return smartLockKeyEntityEntry.Entity;
		}
		catch (NotSupportedException exception)
		{
			_logger.LogError(exception, "Error while adding a SmartLockKey to the database");
			return Result.Fail(new PersistentResourceNotAvailableError());
		}
		catch (DbUpdateException exception)
		{
			_logger.LogError(exception, "Error while adding a SmartLockKey to the database");
			if (exception.InnerException?.InnerException is NpgsqlException && exception.InnerException.InnerException.HResult >
			    int.Parse(PostgresErrorCodes.IntegrityConstraintViolation) && exception.InnerException.InnerException.HResult <
			    int.Parse(PostgresErrorCodes.CheckViolation))
			{
				return Result.Fail(new DuplicateEntryError(nameof(SmartLockKey)));
			}
			return Result.Fail(new PersistentResourceNotAvailableError());
		}
		catch(Exception exception)
		{
			_logger.LogError(exception, "Error while adding a SmartLockKey to the database");
			return Result.Fail(new KerberoError());
		}
	}
}
