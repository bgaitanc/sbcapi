using System.Net;

namespace SBC.Domain.Exceptions;

public class DomainException(string message) : SbcException(HttpStatusCode.BadRequest, message);
