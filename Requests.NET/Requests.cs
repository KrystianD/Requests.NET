using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace RequestsNET
{
  public static class Config
  {
    public static IRequestObserver DefaultObserver = new NullObserver();
    public static TimeSpan DefaultTimeout = Timeout.InfiniteTimeSpan;
  }

  public static class Requests
  {
    public static RequestBuilder Method(HttpMethod method, string url) => new RequestBuilder(method, url);

    public static SendOnlyRequestBuilder Head(string url) => new SendOnlyRequestBuilder(HttpMethod.Head, url);
    public static SendOnlyRequestBuilder Get(string url) => new SendOnlyRequestBuilder(HttpMethod.Get, url);
    public static RequestBuilder Post(string url) => new RequestBuilder(HttpMethod.Post, url);
    public static RequestBuilder Put(string url) => new RequestBuilder(HttpMethod.Put, url);
    public static RequestBuilder Delete(string url) => new RequestBuilder(HttpMethod.Delete, url);
    public static SendOnlyRequestBuilder Options(string url) => new SendOnlyRequestBuilder(HttpMethod.Options, url);

    public static Task<Response> GetAsync(
        string url,
        Dictionary<string, string> parameters = null,
        Dictionary<string, string> headers = null,
        string authUser = null,
        string authPass = null,
        string authBearerToken = null,
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default)
    {
      return DoRequestAsync(HttpMethod.Get, url, parameters, null, null, headers, authUser, authPass, authBearerToken, timeout, cancellationToken: cancellationToken);
    }

    public static Task<Response> PostAsync(
        string url,
        Dictionary<string, string> parameters = null,
        Dictionary<string, string> data = null,
        JToken json = null,
        Dictionary<string, string> headers = null,
        string authUser = null,
        string authPass = null,
        string authBearerToken = null,
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default)
    {
      return DoRequestAsync(HttpMethod.Post, url, parameters, data, json, headers, authUser, authPass, authBearerToken, timeout, cancellationToken: cancellationToken);
    }

    public static Task<Response> PutAsync(
        string url,
        Dictionary<string, string> parameters = null,
        Dictionary<string, string> data = null,
        JToken json = null,
        Dictionary<string, string> headers = null,
        string authUser = null,
        string authPass = null,
        string authBearerToken = null,
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default)
    {
      return DoRequestAsync(HttpMethod.Put, url, parameters, data, json, headers, authUser, authPass, authBearerToken, timeout, cancellationToken: cancellationToken);
    }

    private static Task<Response> DoRequestAsync(
        HttpMethod method,
        string url,
        Dictionary<string, string> parameters = null,
        Dictionary<string, string> data = null,
        JToken json = null,
        Dictionary<string, string> headers = null,
        string authUser = null,
        string authPass = null,
        string authBearerToken = null,
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default,
        IRequestObserver observer = null)
    {
      var b = Method(method, url);

      if (headers != null) b.Header(headers);
      if (parameters != null) b.Parameter(parameters);

      if (data != null) b.Form(data);
      if (json != null) b.Json(json);

      if (authUser != null) b.AuthBasic(authUser, authPass);
      if (authBearerToken != null) b.AuthBearer(authBearerToken);

      return b.ExecuteAsync(timeout, cancellationToken, observer);
    }
  }
}