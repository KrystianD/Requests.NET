using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;

namespace RequestsNET
{
  public class HttpClientConfig
  {
    public bool FollowRedirects;
    public bool SkipCertificateValidation;
    public string ProxyDsn;

    private sealed class HttpClientConfigEqualityComparer : IEqualityComparer<HttpClientConfig>
    {
      public bool Equals(HttpClientConfig x, HttpClientConfig y)
      {
        if (ReferenceEquals(x, y)) return true;
        if (ReferenceEquals(x, null)) return false;
        if (ReferenceEquals(y, null)) return false;
        if (x.GetType() != y.GetType()) return false;
        return x.FollowRedirects == y.FollowRedirects && x.SkipCertificateValidation == y.SkipCertificateValidation && x.ProxyDsn == y.ProxyDsn;
      }

      public int GetHashCode(HttpClientConfig obj)
      {
        unchecked {
          var hashCode = obj.FollowRedirects.GetHashCode();
          hashCode = (hashCode * 397) ^ obj.SkipCertificateValidation.GetHashCode();
          hashCode = (hashCode * 397) ^ (obj.ProxyDsn != null ? obj.ProxyDsn.GetHashCode() : 0);
          return hashCode;
        }
      }
    }

    public static IEqualityComparer<HttpClientConfig> Comparer { get; } = new HttpClientConfigEqualityComparer();
  }

  internal static class Shared
  {
    private static readonly Dictionary<HttpClientConfig, HttpClient> _httpClients = new Dictionary<HttpClientConfig, HttpClient>(HttpClientConfig.Comparer);

    internal static HttpClient GetHttpClient(HttpClientConfig config)
    {
      lock (_httpClients) {
        if (_httpClients.TryGetValue(config, out var client))
          return client;

        var handler = new HttpClientHandler {
            UseCookies = false,
            AllowAutoRedirect = config.FollowRedirects,
            AutomaticDecompression = DecompressionMethods.GZip,
        };
        if (config.SkipCertificateValidation)
          handler.ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, cetChain, policyErrors) => true;

        if (config.ProxyDsn != null) {
          Utils.ParseUri(config.ProxyDsn, out var schema, out var username, out var password, out var host, out var port, out _);

          var hasCredentials = !string.IsNullOrEmpty(username) || !string.IsNullOrEmpty(password);
          handler.Proxy = new WebProxy(new Uri($"{schema}://{host}:{port}"),
                                       BypassOnLocal: false,
                                       BypassList: null,
                                       Credentials: hasCredentials ? new NetworkCredential(username, password) : null);
        }

        client = new HttpClient(handler);
        _httpClients.Add(config, client);
        return client;
      }
    }
  }
}