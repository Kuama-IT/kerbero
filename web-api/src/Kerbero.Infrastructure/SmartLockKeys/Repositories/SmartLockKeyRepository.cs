using FluentResults;
using Kerbero.Domain.Common.Errors;
using Kerbero.Domain.NukiCredentials.Models;
using Kerbero.Domain.SmartLockKeys.Models;
using Kerbero.Domain.SmartLockKeys.Repositories;
using Kerbero.Infrastructure.Common.Interfaces;
using Kerbero.Infrastructure.SmartLockKeys.Entities;
using Kerbero.Infrastructure.SmartLockKeys.Mappers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace Kerbero.Infrastructure.SmartLockKeys.Repositories;

public class SmartLockKeyRepository: ISmartLockKeyRepository
{
	private readonly ILogger<SmartLockKeyRepository> _logger;
	private readonly IApplicationDbContext _applicationDbContext;

	public SmartLockKeyRepository(ILogger<SmartLockKeyRepository> logger, IApplicationDbContext applicationDbContext)
	{
		_logger = logger;
		_applicationDbContext = applicationDbContext;
	}

	public async Task<Result<SmartLockKeyModel>> Create(SmartLockKeyModel model)
	{
		try
		{
			var entity = SmartLockKeyMapper.Map(model);
			_applicationDbContext.SmartLockKeys.Add(entity);
			
			await _applicationDbContext.SaveChangesAsync();

			return SmartLockKeyMapper.Map(entity) ;
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
				return Result.Fail(new DuplicateEntryError(nameof(SmartLockKeyEntity)));
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
