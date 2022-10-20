using System.Net;
using FluentResults;
using Flurl;
using Flurl.Http;
using Kerbero.Domain.Common.Errors;
using Kerbero.Domain.Common.Models.ExternalResponses;
using Microsoft.Extensions.Logging;

namespace Kerbero.Infrastructure.Common.Extensions;

public static class UrlExtensions
{
    public static async Task<Result<TResponse>> NukiPostJsonAsync<TResponse>(this Url url, object body, ILogger logger)
    {
        try
        {
            var response = await url.PostJsonAsync(body).ReceiveJson<TResponse>();
            
            if (response is null)
            {
                return Result.Fail(new UnableToParseResponseError("Response is null"));
            }

            return response;
        }
        #region ErrorManagement
        catch (FlurlHttpException exception)
        {
            logger.LogError(exception, "Error while calling nuki Apis with request {}", url.ToString());
            if (exception.StatusCode == (int)HttpStatusCode.Unauthorized)
            {
                var error = await exception.GetResponseJsonAsync<NukiErrorExternalResponse>();
                if (error?.Error != null && error.Error.Contains("invalid"))
                    return Result.Fail(new InvalidParametersError(error.Error + ": " + error.ErrorMessage));
                return Result.Fail(new UnauthorizedAccessError());
            }

            if (exception.StatusCode == (int)HttpStatusCode.RequestTimeout || exception.StatusCode % 100 == 5)
            {
                return Result.Fail(new ExternalServiceUnreachableError());
            }

            return Result.Fail(new UnknownExternalError());
        }
        catch (ArgumentNullException exception)
        {
            logger.LogError(exception, "Error while calling nuki Apis with request {}", url.ToString());

            return Result.Fail(new InvalidParametersError("options"));
        }
        catch(Exception exception)
        {
            logger.LogError(exception, "Error while calling nuki Apis with request {}", url.ToString());
            return Result.Fail(new KerberoError());
        }

        #endregion
    }
    
    public static async Task<Result<TResponse>> NukiAuthGetJsonAsync<TResponse>(this Url url, string bearer,
        ILogger logger)
    {
        try
        {
            var response = await url
                .WithOAuthBearerToken(bearer)
                .GetJsonAsync<TResponse>();
            if (response is null)
            {
                return Result.Fail(new UnableToParseResponseError("Response is null"));
            }

            return response;
        }
        #region ErrorManagement
        catch (FlurlHttpException exception)
        {
            logger.LogError(exception, "Error while retrieving tokens from Nuki Web");
            if (exception.StatusCode == (int)HttpStatusCode.Unauthorized)
            {
                var error = await exception.GetResponseJsonAsync<NukiErrorExternalResponse>();
                if (error?.Error != null && error.Error.Contains("invalid"))
                    return Result.Fail(new InvalidParametersError(error.Error + ": " + error.ErrorMessage));
                return Result.Fail(new UnauthorizedAccessError());
            }

            if (exception.StatusCode == (int)HttpStatusCode.RequestTimeout || exception.StatusCode / 100 == 5)
            {
                return Result.Fail(new ExternalServiceUnreachableError());
            }

            return Result.Fail(new UnknownExternalError());
        }
        catch (ArgumentNullException exception)
        {
            logger.LogError(exception, "Error while retrieving tokens from Nuki Web");
            return Result.Fail(new InvalidParametersError("options"));
        }
        catch(Exception exception)
        {
            logger.LogError(exception, "Error while retrieving tokens from Nuki Web");
            return Result.Fail(new KerberoError());
        }

        #endregion
    }
}