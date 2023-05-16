using System;
using JetBrains.Annotations;

namespace RequestsNET.Exceptions
{
  [PublicAPI]
  public class RequestFailedException : Exception
  {
    public Response Response { get; }

    public RequestFailedException(Response response)
        : base($"Request failed with code: {(int)response.StatusCode}")
    {
      Response = response;
    }
  }
}