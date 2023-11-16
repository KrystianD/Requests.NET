using System.Net.Http;
using JetBrains.Annotations;

namespace RequestsNET.Builders
{
  [PublicAPI]
  public class GenericRequestBuilder : IBodyRequestBuilder<GenericRequestBuilder>
  {
    public GenericRequestBuilder Method(HttpMethod method)
    {
      RequestData.Method = method;
      return this;
    }

    public GenericRequestBuilder Url(string url)
    {
      RequestData.Url = url;
      return this;
    }
  }
}