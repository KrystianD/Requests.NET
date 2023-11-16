using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using RequestsNET.Exceptions;

namespace RequestsNET.Builders
{
  public class BaseRequestBuilder
  {
    protected readonly HttpClientConfig HttpConfig = new HttpClientConfig();
    protected readonly RequestData RequestData = new RequestData();

    public HttpRequestMessage BuildRequest() => RequestExecutor.BuildRequest(RequestData);

    public Task<Response> ExecuteAsync(TimeSpan? timeout = null,
                                       CancellationToken cancellationToken = default,
                                       IRequestObserver observer = null)
    {
      return RequestExecutor.ExecuteAsync(HttpConfig, RequestData, timeout, cancellationToken, observer);
    }

    public async Task<JToken> ToJsonAsync(TimeSpan? timeout = null,
                                          CancellationToken cancellationToken = default,
                                          IRequestObserver observer = null)
    {
      var resp = await ExecuteAsync(timeout, cancellationToken, observer);
      resp.ValidateResponse();
      if (resp.Data is null || resp.Data.Length == 0)
        throw new NoResponseException();
      return resp.ParseAsJson();
    }

    public async Task<T> ToJsonAsync<T>(TimeSpan? timeout = null,
                                        CancellationToken cancellationToken = default,
                                        IRequestObserver observer = null)
    {
      var resp = await ExecuteAsync(timeout, cancellationToken, observer);
      resp.ValidateResponse();
      if (resp.Data is null || resp.Data.Length == 0)
        throw new NoResponseException();
      return resp.ParseAsJson().ToObject<T>(Utils.JsonDeserializer);
    }

    public async Task<byte[]> ToBinaryAsync(TimeSpan? timeout = null,
                                            CancellationToken cancellationToken = default,
                                            IRequestObserver observer = null)
    {
      var resp = await ExecuteAsync(timeout, cancellationToken, observer);
      resp.ValidateResponse();
      return resp.Data;
    }

    public async Task<string> ToTextAsync(TimeSpan? timeout = null,
                                          CancellationToken cancellationToken = default,
                                          IRequestObserver observer = null)
    {
      var resp = await ExecuteAsync(timeout, cancellationToken, observer);
      resp.ValidateResponse();
      return resp.ParseAsText();
    }
  }
}