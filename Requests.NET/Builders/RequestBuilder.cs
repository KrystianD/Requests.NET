using System.Net.Http;
using JetBrains.Annotations;

namespace RequestsNET.Builders
{
  [PublicAPI]
  public class RequestBuilder : IBodyRequestBuilder<RequestBuilder>
  {
    public RequestBuilder(HttpMethod method, string url)
    {
      RequestData.Method = method;
      RequestData.Url = url;
    }
  }
}