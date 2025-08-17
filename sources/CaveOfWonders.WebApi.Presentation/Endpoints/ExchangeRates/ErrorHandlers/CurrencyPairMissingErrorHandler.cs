using CaveOfWonders.WebApi.Presentation.Models;
using DustInTheWind.CaveOfWonders.Cli.Application.PresentExchangeRate;
using DustInTheWind.ErrorFlow.AspNetCore;
using System.Net;

namespace CaveOfWonders.WebApi.Presentation.Endpoints.ExchangeRates.ErrorHandlers;

internal class CurrencyPairMissingErrorHandler : JsonErrorHandler<CurrencyPairMissingException, ErrorApiDto>
{
    protected override HttpStatusCode HttpStatusCode => HttpStatusCode.BadRequest;

    protected override ErrorApiDto BuildHttpResponseBody(CurrencyPairMissingException ex)
    {
        return new ErrorApiDto
        {
            ErrorMessage = ex.Message
        };
    }
}