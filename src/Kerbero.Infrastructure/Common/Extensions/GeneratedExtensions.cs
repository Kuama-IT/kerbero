using System.Net;
using FluentResults;
using Flurl;
using Flurl.Http;
using Kerbero.Domain.Common.Errors;
using Kerbero.Domain.NukiAuthentication.Models;
using Microsoft.Extensions.Logging;

namespace Kerbero.Infrastructure.Common.Extensions;

public static class GeneratedExtensions
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
        catch (FlurlHttpException ex)
        {
            logger.LogError(ex, "Error while calling nuki Apis with request {}", url.ToString());
            if (ex.StatusCode == (int)HttpStatusCode.Unauthorized)
            {
                var error = await ex.GetResponseJsonAsync<NukiErrorExternalResponseDto>();
                if (error?.Error != null && error.Error.Contains("invalid"))
                    return Result.Fail(new InvalidParametersError(error.Error + ": " + error.ErrorMessage));
                return Result.Fail(new UnauthorizedAccessError());
            }

            if (ex.StatusCode == (int)HttpStatusCode.RequestTimeout || ex.StatusCode % 100 == 5)
            {
                return Result.Fail(new ExternalServiceUnreachableError());
            }

            return Result.Fail(new UnknownExternalError());
        }
        catch (ArgumentNullException e)
        {
            logger.LogError(e, "Error while calling nuki Apis with request {}", url.ToString());

            return Result.Fail(new InvalidParametersError("options"));
        }
        catch(Exception e)
        {
            logger.LogError(e, "Error while calling nuki Apis with request {}", url.ToString());
            return Result.Fail(new KerberoError());
        }

        #endregion
    }
}