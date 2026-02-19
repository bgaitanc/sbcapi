using System.Net;

namespace SBC.Domain.Exceptions;

public class SbcException(HttpStatusCode statusCode, string message) : Exception(message)
{
    public HttpStatusCode StatusCode { get; } = statusCode;
}