using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Http;

namespace RequestsNET
{
  public class HttpClientConfig
  {
    public bool FollowRedirects;
    public bool SkipCertificateValidation;

    private sealed class HttpClientConfigEqualityComparer : IEqualityComparer<HttpClientConfig>
    {
      public bool Equals(HttpClientConfig x, HttpClientConfig y)
      {
        if (ReferenceEquals(x, y)) return true;
        if (ReferenceEquals(x, null)) return false;
        if (ReferenceEquals(y, null)) return false;
        if (x.GetType() != y.GetType()) return false;
        return x.FollowRedirects == y.FollowRedirects && x.SkipCertificateValidation == y.SkipCertificateValidation;
      }

      public int GetHashCode(HttpClientConfig obj)
      {
        unchecked { return (obj.FollowRedirects.GetHashCode() * 397) ^ obj.SkipCertificateValidation.GetHashCode(); }
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
        
        client = new HttpClient(handler);
        _httpClients.Add(config, client);
        return client;
      }
    }
  }
}