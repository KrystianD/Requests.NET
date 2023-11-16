using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Newtonsoft.Json.Linq;
using RequestsNET.Builders;

namespace RequestsNET
{
  public static class Config
  {
    public static IRequestObserver DefaultObserver = new NullObserver();
    public static TimeSpan DefaultTimeout = Timeout.InfiniteTimeSpan;
  }

  [PublicAPI]
  public static class Requests
  {
    public static RequestBuilder Method(HttpMethod method, string url) => new RequestBuilder(method, url);

    public static SendOnlyRequestBuilder Head(string url) => new SendOnlyRequestBuilder(HttpMethod.Head, url);
    public static SendOnlyRequestBuilder Get(string url) => new SendOnlyRequestBuilder(HttpMethod.Get, url);
    public static RequestBuilder Post(string url) => new RequestBuilder(HttpMethod.Post, url);
    public static RequestBuilder Put(string url) => new RequestBuilder(HttpMethod.Put, url);
    public static RequestBuilder Delete(string url) => new RequestBuilder(HttpMethod.Delete, url);
    public static SendOnlyRequestBuilder Options(string url) => new SendOnlyRequestBuilder(HttpMethod.Options, url);

    public static GenericRequestBuilder Builder() => new GenericRequestBuilder();
  }
}