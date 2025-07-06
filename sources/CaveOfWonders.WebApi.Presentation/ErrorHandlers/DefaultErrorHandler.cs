using DustInTheWind.ErrorFlow.AspNetCore;
using System.Net;

namespace CaveOfWonders.WebApi.Presentation.ErrorHandlers;

public class DefaultErrorHandler : JsonErrorHandler<Exception, string>
{
    protected override HttpStatusCode HttpStatusCode => HttpStatusCode.InternalServerError;
 
    protected override string BuildHttpResponseBody(Exception ex)
    {
        return "An unexpected error occurred. Please try again later.";
    }
}