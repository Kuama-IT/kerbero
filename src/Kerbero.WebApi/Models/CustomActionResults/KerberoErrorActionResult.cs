using System.Net;
using FluentResults;
using Kerbero.Domain.Common.Errors.CommonErrors;
using Kerbero.Domain.Common.Errors.CreateNukiAccountErrors;
using Kerbero.WebApi.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Kerbero.WebApi.Models.CustomActionResults;

public static class ModelStateDictionaryExtensions
{
	public static ActionResult AddErrorAndReturnAction(this ModelStateDictionary modelStateDict, IError error)
	{
		var key = error.GetType().Name;
		var errorMessage = error.Message;
		
		modelStateDict.AddModelError(key, errorMessage);

		return new ObjectResult(error) { StatusCode = (int?)HttpResponseKerberoErrorToStatusCode(error)};
	}
	
	private static HttpStatusCode HttpResponseKerberoErrorToStatusCode(IError error)
	{
		switch (error)
		{
			case ExternalServiceUnreachableError:
			case UnknownExternalError:
			case PersistentResourceNotAvailableError:
				return HttpStatusCode.ServiceUnavailable;
			case UnableToParseResponseError:
				return HttpStatusCode.BadGateway;
			case UnauthorizedAccessError:
				return HttpStatusCode.Unauthorized;
			case InvalidParametersError:
			case DuplicateEntryError:
				return HttpStatusCode.BadRequest;
			default:
				throw new DevException("Forgot to map the error with status code");
		}
	}
}
