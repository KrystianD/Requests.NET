using System;
using JetBrains.Annotations;

namespace RequestsNET.Exceptions
{
  [PublicAPI]
  public class ResponseProcessingException : Exception
  {
    public Response Response { get; }

    public ResponseProcessingException(Response response, Exception innerException) : base($"Error during processing response: [{innerException.GetType().Name}] {innerException.Message}", innerException)
    {
      Response = response;
    }
  }
}