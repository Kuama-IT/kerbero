using System.Net;
using System.Web.Http;
using FluentResults;
using Kerbero.Common.Errors;
using Kerbero.Common.Errors.CreateNukiAccountErrors;
using Kerbero.WebApi.Exceptions;
using static System.Enum;

namespace Kerbero.WebApi.Models.ErrorMapper;

public static class HttpResponseExceptionMap
{
	public static HttpResponseException Map(KerberoError error)
	{

		var statusCode = HttpResponseKerberoErrorToStatusCode(error);
		var httpResponseMessage = new HttpResponseMessage
		{
			StatusCode = statusCode,
			Content = JsonContent.Create(new KerberoWebApiErrorResponse()
			{
				Error = error.GetType().Name,
				ErrorMessage = error.Message
			})
		};
		return new HttpResponseException(httpResponseMessage);
	}

	private static HttpStatusCode HttpResponseKerberoErrorToStatusCode(KerberoError error)
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
