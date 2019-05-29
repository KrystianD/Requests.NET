using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using NUnit.Framework;

namespace RequestsNET.Tests
{
  public class HeadersTests
  {
    [Test]
    public async Task HeaderString()
    {
      var resp = await Requests.Get("http://localhost:9999/headers")
                               .Header("X-Custom", "value1")
                               .ToJsonAsync<HttpBinResponse>();

      CollectionAssert.AreEquivalent(
          new Dictionary<string, string>() {
              ["Host"] = "localhost:9999",
              ["X-Custom"] = "value1",
          },
          resp.Headers);

      resp = await Requests.Post("http://localhost:9999/post")
                           .Header("X-Custom", "value1")
                           .ToJsonAsync<HttpBinResponse>();

      CollectionAssert.AreEquivalent(
          new Dictionary<string, string>() {
              ["Host"] = "localhost:9999",
              ["Content-Length"] = "0",
              ["X-Custom"] = "value1",
          },
          resp.Headers);
    }

    [Test]
    public async Task HeaderEnum()
    {
      var resp = await Requests.Get("http://localhost:9999/headers")
                               .Header(HttpRequestHeader.Host, "test.com")
                               .ToJsonAsync<HttpBinResponse>();

      CollectionAssert.AreEquivalent(
          new Dictionary<string, string>() {
              ["Host"] = "test.com",
          },
          resp.Headers);

      resp = await Requests.Post("http://localhost:9999/post")
                           .Header(HttpRequestHeader.Host, "test.com")
                           .ToJsonAsync<HttpBinResponse>();

      CollectionAssert.AreEquivalent(
          new Dictionary<string, string>() {
              ["Host"] = "test.com",
              ["Content-Length"] = "0",
          },
          resp.Headers);
    }

    [Test]
    public async Task HeaderDict()
    {
      var resp = await Requests.Get("http://localhost:9999/headers")
                               .Header(new Dictionary<string, string>() {
                                   ["X-Custom1"] = "value1",
                                   ["X-Custom2"] = "value2",
                               })
                               .ToJsonAsync<HttpBinResponse>();

      CollectionAssert.AreEquivalent(
          new Dictionary<string, string>() {
              ["Host"] = "localhost:9999",
              ["X-Custom1"] = "value1",
              ["X-Custom2"] = "value2",
          },
          resp.Headers);

      resp = await Requests.Post("http://localhost:9999/post")
                           .Header(new Dictionary<string, string>() {
                               ["X-Custom1"] = "value1",
                               ["X-Custom2"] = "value2",
                           })
                           .ToJsonAsync<HttpBinResponse>();

      CollectionAssert.AreEquivalent(
          new Dictionary<string, string>() {
              ["Host"] = "localhost:9999",
              ["Content-Length"] = "0",
              ["X-Custom1"] = "value1",
              ["X-Custom2"] = "value2",
          },
          resp.Headers);
    }

    [Test]
    public async Task HeaderMultiple()
    {
      var resp = await Requests.Get("http://localhost:9999/headers")
                               .Header("X-Custom", "value1")
                               .Header("X-Custom", "value2")
                               .ToJsonAsync<HttpBinResponse>();

      CollectionAssert.AreEquivalent(
          new Dictionary<string, string>() {
              ["Host"] = "localhost:9999",
              ["X-Custom"] = "value1, value2",
          },
          resp.Headers);

      resp = await Requests.Post("http://localhost:9999/post")
                           .Header("X-Custom", "value1")
                           .Header("X-Custom", "value2")
                           .ToJsonAsync<HttpBinResponse>();

      CollectionAssert.AreEquivalent(
          new Dictionary<string, string>() {
              ["Host"] = "localhost:9999",
              ["Content-Length"] = "0",
              ["X-Custom"] = "value1, value2",
          },
          resp.Headers);
    }
  }
}