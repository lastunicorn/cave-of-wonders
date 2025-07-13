using DustInTheWind.CaveOfWonders.Cli.Application.Convert;
using DustInTheWind.ErrorFlow.AspNetCore;
using System.Net;

namespace CaveOfWonders.WebApi.Presentation.ErrorHandlers;

internal class ExchangeRateUnusableErrorHandler : JsonErrorHandler<ExchangeRateUnusableException, string>
{
    protected override HttpStatusCode HttpStatusCode => HttpStatusCode.BadRequest;

    protected override string BuildHttpResponseBody(ExchangeRateUnusableException ex)
    {
        return $"Exchange rate is unusable: {ex.Message}";
    }
}
