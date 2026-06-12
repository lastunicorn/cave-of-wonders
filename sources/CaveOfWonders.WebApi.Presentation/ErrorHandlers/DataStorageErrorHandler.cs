using DustInTheWind.ErrorFlow.AspNetCore;
using System.Net;
using DustInTheWind.CaveOfWonders.Cli.Application.ImportCpi;

namespace CaveOfWonders.WebApi.Presentation.ErrorHandlers;

public class DataStorageErrorHandler : JsonErrorHandler<DataStorageException, string>
{
    protected override HttpStatusCode HttpStatusCode => HttpStatusCode.FailedDependency;

    protected override string BuildHttpResponseBody(DataStorageException ex)
    {
        return $"Failed to store data: {ex.Message}";
    }
}
