using System.Net;

namespace SBC.Domain.Exceptions;

public class NotFoundException(string name, object key) 
    : SbcException(HttpStatusCode.NotFound, $"Entity \"{name}\" ({key}) was not found.");
