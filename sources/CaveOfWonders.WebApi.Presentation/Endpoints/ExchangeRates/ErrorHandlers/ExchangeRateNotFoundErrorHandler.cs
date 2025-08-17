using CaveOfWonders.WebApi.Presentation.Models;
using DustInTheWind.CaveOfWonders.Cli.Application.PresentExchangeRate;
using DustInTheWind.ErrorFlow.AspNetCore;
using System.Net;

namespace CaveOfWonders.WebApi.Presentation.Endpoints.ExchangeRates.ErrorHandlers;

internal class ExchangeRateNotFoundErrorHandler : JsonErrorHandler<ExchangeRateNotFoundException, ErrorApiDto>
{
    protected override HttpStatusCode HttpStatusCode => HttpStatusCode.NotFound;

    protected override ErrorApiDto BuildHttpResponseBody(ExchangeRateNotFoundException ex)
    {
        return new ErrorApiDto
        {
            ErrorMessage = ex.Message
        };
    }
}
