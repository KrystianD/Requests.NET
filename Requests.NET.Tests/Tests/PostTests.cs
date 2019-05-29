using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace RequestsNET.Tests
{
  public class PostTests
  {
    [Test]
    public async Task Form()
    {
      var resp = await Requests.Post("http://localhost:9999/post")
                               .Form("a", "a")
                               .Form("c", "c")
                               .ToJsonAsync<HttpBinResponse>();

      CollectionAssert.AreEquivalent(
          new Dictionary<string, string>() {
              ["a"] = "a",
              ["c"] = "c",
          },
          resp.Form);

      CollectionAssert.AreEquivalent(
          new Dictionary<string, string>() {
              ["Host"] = "localhost:9999",
              ["Content-Length"] = "7",
              ["Content-Type"] = "application/x-www-form-urlencoded",
          },
          resp.Headers);
    }

    [Test]
    public async Task Binary()
    {
      var resp = await Requests.Post("http://localhost:9999/anything")
                               .Binary(new byte[] { 97, 98, 99 })
                               .ToJsonAsync<HttpBinResponse>();

      CollectionAssert.AreEquivalent("abc", resp.Data);
    }

    [Test]
    public async Task String()
    {
      var resp = await Requests.Post("http://localhost:9999/anything")
                               .String("abc")
                               .ToJsonAsync<HttpBinResponse>();

      CollectionAssert.AreEquivalent("abc", resp.Data);
    }

    [Test]
    public async Task Json()
    {
      var resp = await Requests.Post("http://localhost:9999/anything")
                               .Json(JToken.FromObject(new { a = 1 }))
                               .ToJsonAsync<HttpBinResponse>();

      Assert.AreEqual(JToken.FromObject(new { a = 1 }), resp.Json);

      resp = await Requests.Post("http://localhost:9999/anything")
                           .Json(new { a = 1 })
                           .ToJsonAsync<HttpBinResponse>();

      Assert.AreEqual(JToken.FromObject(new { a = 1 }), resp.Json);
    }
  }
}