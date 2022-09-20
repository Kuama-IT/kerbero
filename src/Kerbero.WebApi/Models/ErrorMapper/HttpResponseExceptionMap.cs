using System.Net;
using System.Web.Http;
using Kerbero.Common.Errors;
using static System.Enum;

namespace Kerbero.WebApi.Models.ErrorMapper;

public static class HttpResponseExceptionMap
{
	public static HttpResponseException Map(KerberoError error)
	{

		var resParse = TryParse(error.GetType().Name, out ErrorToStatusCode statusCode);
		var httpResponseMessage = new HttpResponseMessage()
		{
			StatusCode = resParse ? (HttpStatusCode)statusCode : HttpStatusCode.InternalServerError,
			Content = JsonContent.Create(new KerberoWebApiErrorResponse()
			{
				Error = error.GetType().Name,
				ErrorMessage = error.Message
			})
		};
		return new HttpResponseException(httpResponseMessage);
	}

	public enum ErrorToStatusCode
	{
		ExternalServiceUnreachableError = HttpStatusCode.ServiceUnavailable,
		UnableToParseResponseError = HttpStatusCode.BadGateway,
		UnauthorizedAccessError = HttpStatusCode.Unauthorized,
		KerberoError = HttpStatusCode.InternalServerError,
		InvalidParametersError = HttpStatusCode.BadRequest
	}
}
