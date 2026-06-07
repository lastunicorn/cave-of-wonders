using DustInTheWind.CaveOfWonders.Cli.Application.ImportExchangeRates;
using DustInTheWind.ErrorFlow.AspNetCore;
using System.Net;

namespace CaveOfWonders.WebApi.Presentation.ErrorHandlers;

internal class BnrWebsiteAccessErrorHandler : JsonErrorHandler<BnrWebsiteAccessException, string>
{
    protected override HttpStatusCode HttpStatusCode => HttpStatusCode.FailedDependency;

    protected override string BuildHttpResponseBody(BnrWebsiteAccessException ex)
    {
        return $"Failed to access BNR website: {ex.Message}";
    }
}
