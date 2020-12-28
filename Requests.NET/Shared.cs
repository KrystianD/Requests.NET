using System.Net.Http;

namespace RequestsNET
{
  internal static class Shared
  {
    internal static readonly HttpClient HTTPClient = new HttpClient(new HttpClientHandler { UseCookies = false });
  }
}