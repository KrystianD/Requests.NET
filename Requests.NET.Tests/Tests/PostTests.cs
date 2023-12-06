using System.Collections.Generic;
using System.Text;
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
              ["Accept-Encoding"] = "gzip",
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
    public async Task BinaryContentType()
    {
      var resp = await Requests.Post("http://localhost:9999/anything")
                               .Binary(new byte[] { 97, 98, 99 }, contentType: "text/custom")
                               .ToJsonAsync<HttpBinResponse>();

      CollectionAssert.AreEquivalent(
          new Dictionary<string, string>() {
              ["Host"] = "localhost:9999",
              ["Content-Length"] = "3",
              ["Content-Type"] = "text/custom",
              ["Accept-Encoding"] = "gzip",
          },
          resp.Headers);
    }

    [Test]
    public async Task String()
    {
      var resp = await Requests.Post("http://localhost:9999/anything")
                               .String("abc")
                               .ToJsonAsync<HttpBinResponse>();

      CollectionAssert.AreEquivalent("abc", resp.Data);

      CollectionAssert.AreEquivalent(
          new Dictionary<string, string>() {
              ["Host"] = "localhost:9999",
              ["Content-Length"] = "3",
              ["Content-Type"] = "text/plain; charset=utf-8",
              ["Accept-Encoding"] = "gzip",
          },
          resp.Headers);
    }

    [Test]
    public async Task StringEncoding()
    {
      var resp = await Requests.Post("http://localhost:9999/anything")
                               .String("abc", Encoding.UTF32)
                               .ToJsonAsync<HttpBinResponse>();

      CollectionAssert.AreEquivalent("a\0\0\0b\0\0\0c\0\0\0", resp.Data);

      CollectionAssert.AreEquivalent(
          new Dictionary<string, string>() {
              ["Host"] = "localhost:9999",
              ["Content-Length"] = "12",
              ["Content-Type"] = "text/plain; charset=utf-32",
              ["Accept-Encoding"] = "gzip",
          },
          resp.Headers);
    }

    [Test]
    public async Task StringContentType()
    {
      var resp = await Requests.Post("http://localhost:9999/anything")
                               .String("abc", contentType: "text/custom")
                               .ToJsonAsync<HttpBinResponse>();

      CollectionAssert.AreEquivalent(
          new Dictionary<string, string>() {
              ["Host"] = "localhost:9999",
              ["Content-Length"] = "3",
              ["Content-Type"] = "text/custom; charset=utf-8",
              ["Accept-Encoding"] = "gzip",
          },
          resp.Headers);
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

      CollectionAssert.AreEquivalent(
          new Dictionary<string, string>() {
              ["Host"] = "localhost:9999",
              ["Content-Length"] = "12",
              ["Content-Type"] = "application/json; charset=utf-8",
              ["Accept-Encoding"] = "gzip",
          },
          resp.Headers);
    }

    [Test]
    public async Task JsonCustomType()
    {
      var resp = await Requests.Post("http://localhost:9999/anything")
                               .Json(JToken.FromObject(new { a = 1 }), contentType: "text/custom")
                               .ToJsonAsync<HttpBinResponse>();

      CollectionAssert.AreEquivalent(
          new Dictionary<string, string>() {
              ["Host"] = "localhost:9999",
              ["Content-Length"] = "12",
              ["Content-Type"] = "text/custom; charset=utf-8",
              ["Accept-Encoding"] = "gzip",
          },
          resp.Headers);
    }
  }
}