using System.Net;
using FluentAssertions;
using FluentResults;
using Kerbero.Domain.Common.Errors;
using Kerbero.Domain.NukiAuthentication.Models;
using Kerbero.WebApi.Utils.Extensions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Kerbero.WebApi.Tests;

public class ModelStateDictionaryExtensionsTest: ControllerBase
{
	
	public static IEnumerable<object[]> ErrorToTest =>
		new List<object[]>
		{
			new object[] { new ExternalServiceUnreachableError()},
			new object[] { new UnableToParseResponseError()},
			new object[] { new UnauthorizedAccessError()},
			new object[] { new InvalidParametersError("VALID_CLIENT_ID") },
			new object[] { new DuplicateEntryError("Nuki account") },
			new object[] { new UnknownExternalError() },
			new object[] { new PersistentResourceNotAvailableError() }
		};
	
	[MemberData(nameof(ErrorToTest))]
	[Theory]
	public void AddErrorAndReturnAction_Test(KerberoError error)
	{
		// Arrange
		
		// Act
		var action = ModelState.AddErrorAndReturnAction(error) as ObjectResult;

		// Assert
		switch (error)
		{
			case InvalidParametersError:
			case DuplicateEntryError:
				action!.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
				break;
			case UnknownExternalError:
				action!.StatusCode.Should().Be((int)HttpStatusCode.ServiceUnavailable);
				break;
			case UnableToParseResponseError:
			case ExternalServiceUnreachableError:
			case PersistentResourceNotAvailableError:
				action!.StatusCode.Should().Be((int)HttpStatusCode.BadGateway);
				break;
			case UnauthorizedAccessError:
				action!.StatusCode.Should().Be((int)HttpStatusCode.Unauthorized);
				break;
			case not null:
				action!.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
				break;
		}
	}
}
