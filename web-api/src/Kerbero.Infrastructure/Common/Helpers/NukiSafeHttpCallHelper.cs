using System.Net;
using FluentResults;
using Flurl.Http;
using Kerbero.Domain.Common.Errors;
using Kerbero.Domain.Common.Models.ExternalResponses;
using Kerbero.Domain.SmartLocks.Errors;
using Microsoft.Extensions.Logging;

namespace Kerbero.Infrastructure.Common.Helpers;

public class NukiSafeHttpCallHelper
{
    private readonly ILogger<NukiSafeHttpCallHelper> _logger;

    public NukiSafeHttpCallHelper(ILogger<NukiSafeHttpCallHelper> logger)
    {
        _logger = logger;
    }
    
    public async Task<Result<TResponse>> Handle<TResponse>(Func<Task<TResponse>> call)
    {
        try
        {
            var response = await call.Invoke();
            
            if (response is null)
            {
                return Result.Fail(new UnableToParseResponseError("Response is null"));
            }

            return Result.Ok(response);
        }
        #region ErrorManagement
        catch (FlurlHttpException exception)
        {
            _logger.LogError(exception, "Error while calling nuki Apis with request");
            _logger.LogDebug(await exception.GetResponseStringAsync());
            if (exception.StatusCode is (int)HttpStatusCode.Unauthorized or (int)HttpStatusCode.MethodNotAllowed or (int)HttpStatusCode.Forbidden)
            {
                var error = await exception.GetResponseJsonAsync<NukiErrorExternalResponse>();
                if (error?.Error?.Contains("invalid") == true)
                {
                    return Result.Fail(new InvalidParametersError(error.Error + ": " + error.ErrorMessage));
                }
                return Result.Fail(new UnauthorizedAccessError());
            }

            if (exception.StatusCode is (int)HttpStatusCode.BadRequest)
            {
                return Result.Fail(new InvalidParametersError(exception.Call.HttpRequestMessage.RequestUri!.PathAndQuery));
            }

            if (exception.StatusCode is (int)HttpStatusCode.NotFound)
            {
                return Result.Fail(new SmartLockNotFoundError(exception.Call.HttpRequestMessage.RequestUri!.Segments.Last()));
            }

            if (exception.StatusCode is (int)HttpStatusCode.RequestTimeout or >= 500)
            {
                return Result.Fail(new ExternalServiceUnreachableError());
            }

            return Result.Fail(new UnknownExternalError());
        }
        catch (ArgumentNullException exception)
        {
            _logger.LogError(exception, "Error while calling nuki Apis with request");
            return Result.Fail(new InvalidParametersError("options"));
        }
        catch(Exception exception)
        {
            _logger.LogError(exception, "Error while calling nuki Apis with request");
            return Result.Fail(new KerberoError());
        }

        #endregion
    }
}