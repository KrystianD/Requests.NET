using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;

namespace RequestsNET.Tests
{
  public class UrlParamsTests
  {
    [Test]
    public async Task GetParams()
    {
      var resp = await Requests.Get("http://localhost:9999/get")
                               .Parameter("p1", "v1")
                               .ToJsonAsync<HttpBinResponse>();

      Assert.AreEqual("http://localhost:9999/get?p1=v1", resp.Url);
      CollectionAssert.AreEquivalent(
          new Dictionary<string, string>() {
              ["p1"] = "v1",
          },
          resp.Args);
    }

    [Test]
    public async Task GetParamsExisting()
    {
      var resp = await Requests.Get("http://localhost:9999/get?p1=v1")
                               .Parameter("p2", "v2")
                               .ToJsonAsync<HttpBinResponse>();

      Assert.AreEqual("http://localhost:9999/get?p1=v1&p2=v2", resp.Url);
      CollectionAssert.AreEquivalent(
          new Dictionary<string, string>() {
              ["p1"] = "v1",
              ["p2"] = "v2",
          },
          resp.Args);
    }

    [Test]
    public async Task GetParamsEscape()
    {
      var resp = await Requests.Get("http://localhost:9999/get?a=b")
                               .Parameter("p1", "a b")
                               .Parameter("p2", "c d")
                               .ToJsonAsync<HttpBinResponse>();

      Assert.AreEqual("http://localhost:9999/get?a=b&p1=a+b&p2=c+d", resp.Url);
      CollectionAssert.AreEquivalent(
          new Dictionary<string, string>() {
              ["a"] = "b",
              ["p1"] = "a b",
              ["p2"] = "c d",
          },
          resp.Args);
    }

    [Test]
    public async Task GetParamsMultiple()
    {
      var resp = await Requests.Get("http://localhost:9999/get")
                               .Parameter(new Dictionary<string, string>() {
                                   ["a"] = "b",
                                   ["c"] = "d",
                               })
                               .Parameter("e", "f")
                               .ToJsonAsync<HttpBinResponse>();

      Assert.AreEqual("http://localhost:9999/get?a=b&c=d&e=f", resp.Url);
      CollectionAssert.AreEquivalent(
          new Dictionary<string, string>() {
              ["a"] = "b",
              ["c"] = "d",
              ["e"] = "f",
          },
          resp.Args);
    }
  }
}