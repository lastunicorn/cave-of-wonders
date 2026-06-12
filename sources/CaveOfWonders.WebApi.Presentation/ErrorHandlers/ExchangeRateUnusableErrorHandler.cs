using DustInTheWind.ErrorFlow.AspNetCore;
using System.Net;
using DustInTheWind.CaveOfWonders.Cli.Application.ConvertCurrency;

namespace CaveOfWonders.WebApi.Presentation.ErrorHandlers;

internal class ExchangeRateUnusableErrorHandler : JsonErrorHandler<ExchangeRateUnusableException, string>
{
    protected override HttpStatusCode HttpStatusCode => HttpStatusCode.BadRequest;

    protected override string BuildHttpResponseBody(ExchangeRateUnusableException ex)
    {
        return $"Exchange rate is unusable: {ex.Message}";
    }
}
