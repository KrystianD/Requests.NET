using System.Collections.Generic;
using System.Net;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using NUnit.Framework;

namespace RequestsNET.Tests
{
  public class AuthTests
  {
    [Test]
    public async Task BasicAuth()
    {
      var resp = await Requests.Get("http://localhost:9999/headers")
                               .AuthBasic("user", "pass")
                               .ToJsonAsync<HttpBinResponse>();

      CollectionAssert.AreEquivalent(
          new Dictionary<string, string>() {
              ["Host"] = "localhost:9999",
              ["Authorization"] = "Basic dXNlcjpwYXNz",
          },
          resp.Headers);
    }

    [Test]
    public async Task Bearer()
    {
      var resp = await Requests.Get("http://localhost:9999/headers")
                               .AuthBearer("token")
                               .ToJsonAsync<HttpBinResponse>();

      CollectionAssert.AreEquivalent(
          new Dictionary<string, string>() {
              ["Host"] = "localhost:9999",
              ["Authorization"] = "Bearer token",
          },
          resp.Headers);
    }

    [Test]
    public async Task Custom()
    {
      var resp = await Requests.Get("http://localhost:9999/headers")
                               .Auth(new AuthenticationHeaderValue("scheme", "param"))
                               .ToJsonAsync<HttpBinResponse>();

      CollectionAssert.AreEquivalent(
          new Dictionary<string, string>() {
              ["Host"] = "localhost:9999",
              ["Authorization"] = "scheme param",
          },
          resp.Headers);

      resp = await Requests.Post("http://localhost:9999/post")
                           .Auth(new AuthenticationHeaderValue("scheme", "param"))
                           .ToJsonAsync<HttpBinResponse>();

      CollectionAssert.AreEquivalent(
          new Dictionary<string, string>() {
              ["Host"] = "localhost:9999",
              ["Content-Length"] = "0",
              ["Authorization"] = "scheme param",
          },
          resp.Headers);
    }
  }
}