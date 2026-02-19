using System.Net;

namespace SBC.Api.Controllers.Base;

public class SbcGenericResponse
{
    public string? Message { get; set; }
    public HttpStatusCode StatusCode { get; set; }
    public bool Success { get; set; }
}

public class SbcGenericResponse<T> : SbcGenericResponse
{
    public required T Data { get; set; }
}