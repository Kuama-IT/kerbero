using System.Net;
using FluentResults;
using Kerbero.Domain.Common.Errors;
using Kerbero.Domain.NukiCredentials.Errors;
using Kerbero.Domain.SmartLockKeys.Errors;
using Kerbero.Domain.SmartLocks.Errors;
using Kerbero.WebApi.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Kerbero.WebApi.Utils.Extensions;

public static class ModelStateDictionaryExtension
{
  public static ActionResult AddErrorAndReturnAction(this ModelStateDictionary modelStateDict, IError error)
  {
    var key = error.GetType().Name;
    var errorMessage = error.Message;

    modelStateDict.AddModelError(key, errorMessage);

    return new ObjectResult(error) { StatusCode = (int?)HttpResponseKerberoErrorToStatusCode(error) };
  }

  private static HttpStatusCode HttpResponseKerberoErrorToStatusCode(IError error)
  {
    switch (error)
    {
      case UnknownExternalError:
        return HttpStatusCode.ServiceUnavailable;
      case UnableToParseResponseError:
      case ExternalServiceUnreachableError:
      case PersistentResourceNotAvailableError:
        return HttpStatusCode.BadGateway;
      case UnauthorizedAccessError:
      case NukiCredentialNotFoundError:
        return HttpStatusCode.Unauthorized;
      case InvalidParametersError:
      case DuplicateEntryError:
      case SmartLockNotFoundError:
      case NukiCredentialInvalidTokenError:
      case SmartLockKeyExpiredError:
      case SmartLockKeyPastValidFromError:
      case SmartLockKeyValidUntilPrecedeValidFromError:
        return HttpStatusCode.BadRequest;
      default:
        throw new DevException("Forgot to map the error with status code");
    }
  }
}