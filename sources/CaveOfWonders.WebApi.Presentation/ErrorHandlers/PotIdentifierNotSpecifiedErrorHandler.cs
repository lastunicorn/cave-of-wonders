using DustInTheWind.CaveOfWonders.Cli.Application.PresentPot;
using DustInTheWind.ErrorFlow.AspNetCore;
using System.Net;

namespace CaveOfWonders.WebApi.Presentation.ErrorHandlers;

internal class PotIdentifierNotSpecifiedErrorHandler : JsonErrorHandler<PotIdentifierNotSpecifiedException, string>
{
    protected override HttpStatusCode HttpStatusCode => HttpStatusCode.BadRequest;

    protected override string BuildHttpResponseBody(PotIdentifierNotSpecifiedException ex)
    {
        return "Pot identifier is required";
    }
}
