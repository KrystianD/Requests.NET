using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Newtonsoft.Json.Linq;
using RequestsNET.Exceptions;

namespace RequestsNET.Builders
{
  [PublicAPI]
  public class BaseRequestBuilder<T> where T : BaseRequestBuilder<T>
  {
    protected readonly HttpClientConfig HttpConfig = new HttpClientConfig();
    protected readonly RequestData RequestData = new RequestData();

    public T ValidateResponse()
    {
      RequestData.ValidateResponse = true;
      return (T)this;
    }

    public T FollowRedirects(bool followRedirects = true)
    {
      HttpConfig.FollowRedirects = followRedirects;
      return (T)this;
    }

    public T SkipCertificateValidation(bool skipCertificateValidation = true)
    {
      HttpConfig.SkipCertificateValidation = skipCertificateValidation;
      return (T)this;
    }

    public T Header(string name, string value, bool validate = true)
    {
      if (validate)
        RequestData.Headers.Add(name, value);
      else
        RequestData.Headers.TryAddWithoutValidation(name, value);
      return (T)this;
    }

    public T Header(HttpRequestHeader header, string value, bool validate = true)
    {
      if (validate)
        RequestData.Headers.Add(header.ToString(), value);
      else
        RequestData.Headers.TryAddWithoutValidation(header.ToString(), value);
      return (T)this;
    }

    public T Header(IDictionary<string, string> headers, bool validate = true)
    {
      foreach (var keyValuePair in headers) {
        if (validate)
          RequestData.Headers.Add(keyValuePair.Key, keyValuePair.Value);
        else
          RequestData.Headers.TryAddWithoutValidation(keyValuePair.Key, keyValuePair.Value);
      }

      return (T)this;
    }

    public T Cookie(string name, string value)
    {
      RequestData.Cookies[name] = value;
      return (T)this;
    }

    public T Cookie(IDictionary<string, string> cookies)
    {
      foreach (var keyValuePair in cookies)
        RequestData.Cookies.Add(keyValuePair.Key, keyValuePair.Value);
      return (T)this;
    }

    public T AuthBasic(string user, string password)
    {
      var authB64 = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{user}:{password}"));
      RequestData.Auth = new AuthenticationHeaderValue("Basic", authB64);
      return (T)this;
    }

    public T AuthBearer(string token)
    {
      RequestData.Auth = new AuthenticationHeaderValue("Bearer", token);
      return (T)this;
    }

    public T Auth(AuthenticationHeaderValue auth)
    {
      RequestData.Auth = auth;
      return (T)this;
    }

    public T Proxy(string dsn)
    {
      HttpConfig.ProxyDsn = dsn;
      return (T)this;
    }

    public T Parameter(string name, string value)
    {
      RequestData.Parameters.Add(name, value);
      return (T)this;
    }

    public T Parameter(IDictionary<string, string> parameters)
    {
      foreach (var keyValuePair in parameters)
        RequestData.Parameters.Add(keyValuePair);
      return (T)this;
    }

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

    public async Task<TObj> ToJsonAsync<TObj>(TimeSpan? timeout = null,
                                              CancellationToken cancellationToken = default,
                                              IRequestObserver observer = null)
    {
      var resp = await ExecuteAsync(timeout, cancellationToken, observer);
      resp.ValidateResponse();
      if (resp.Data is null || resp.Data.Length == 0)
        throw new NoResponseException();
      return resp.ParseAsJson().ToObject<TObj>(jsonSerializer ?? Utils.JsonDeserializer);
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