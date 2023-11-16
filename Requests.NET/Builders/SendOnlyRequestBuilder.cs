using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using JetBrains.Annotations;

namespace RequestsNET.Builders
{
  [PublicAPI]
  public class SendOnlyRequestBuilder : BaseRequestBuilder
  {
    public SendOnlyRequestBuilder(HttpMethod method, string url)
    {
      RequestData.Method = method;
      RequestData.Url = url;
    }

    public SendOnlyRequestBuilder ValidateResponse()
    {
      RequestData.ValidateResponse = true;
      return this;
    }

    public SendOnlyRequestBuilder FollowRedirects(bool followRedirects = true)
    {
      HttpConfig.FollowRedirects = followRedirects;
      return this;
    }

    public SendOnlyRequestBuilder SkipCertificateValidation(bool skipCertificateValidation = true)
    {
      HttpConfig.SkipCertificateValidation = skipCertificateValidation;
      return this;
    }

    public SendOnlyRequestBuilder Header(string name, string value, bool validate = true)
    {
      if (validate)
        RequestData.Headers.Add(name, value);
      else
        RequestData.Headers.TryAddWithoutValidation(name, value);
      return this;
    }

    public SendOnlyRequestBuilder Header(HttpRequestHeader header, string value, bool validate = true)
    {
      if (validate)
        RequestData.Headers.Add(header.ToString(), value);
      else
        RequestData.Headers.TryAddWithoutValidation(header.ToString(), value);
      return this;
    }

    public SendOnlyRequestBuilder Header(IDictionary<string, string> headers, bool validate = true)
    {
      foreach (var keyValuePair in headers) {
        if (validate)
          RequestData.Headers.Add(keyValuePair.Key, keyValuePair.Value);
        else
          RequestData.Headers.TryAddWithoutValidation(keyValuePair.Key, keyValuePair.Value);
      }

      return this;
    }

    public SendOnlyRequestBuilder Cookie(string name, string value)
    {
      RequestData.Cookies[name] = value;
      return this;
    }

    public SendOnlyRequestBuilder Cookie(IDictionary<string, string> cookies)
    {
      foreach (var keyValuePair in cookies)
        RequestData.Cookies.Add(keyValuePair.Key, keyValuePair.Value);
      return this;
    }

    public SendOnlyRequestBuilder AuthBasic(string user, string password)
    {
      var authB64 = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{user}:{password}"));
      RequestData.Auth = new AuthenticationHeaderValue("Basic", authB64);
      return this;
    }

    public SendOnlyRequestBuilder AuthBearer(string token)
    {
      RequestData.Auth = new AuthenticationHeaderValue("Bearer", token);
      return this;
    }

    public SendOnlyRequestBuilder Auth(AuthenticationHeaderValue auth)
    {
      RequestData.Auth = auth;
      return this;
    }

    public SendOnlyRequestBuilder Proxy(string dsn)
    {
      HttpConfig.ProxyDsn = dsn;
      return this;
    }

    public SendOnlyRequestBuilder Parameter(string name, string value)
    {
      RequestData.Parameters.Add(name, value);
      return this;
    }

    public SendOnlyRequestBuilder Parameter(IDictionary<string, string> parameters)
    {
      foreach (var keyValuePair in parameters)
        RequestData.Parameters.Add(keyValuePair);
      return this;
    }
  }
}