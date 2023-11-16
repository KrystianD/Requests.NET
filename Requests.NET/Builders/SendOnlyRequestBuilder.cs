using System.Net.Http;
using JetBrains.Annotations;

namespace RequestsNET.Builders
{
  [PublicAPI]
  public class SendOnlyRequestBuilder : BaseRequestBuilder<SendOnlyRequestBuilder>
  {
    public SendOnlyRequestBuilder(HttpMethod method, string url)
    {
      RequestData.Method = method;
      RequestData.Url = url;
    }
  }
}