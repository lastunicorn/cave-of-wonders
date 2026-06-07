using DustInTheWind.CaveOfWonders.Cli.Application.ImportExchangeRates;
using DustInTheWind.ErrorFlow.AspNetCore;
using System.Net;

namespace CaveOfWonders.WebApi.Presentation.ErrorHandlers;

internal class ImportFileAccessErrorHandler : JsonErrorHandler<ImportFileAccessException, string>
{
    protected override HttpStatusCode HttpStatusCode => HttpStatusCode.FailedDependency;

    protected override string BuildHttpResponseBody(ImportFileAccessException ex)
    {
        return $"Failed to access import file: {ex.Message}";
    }
}
