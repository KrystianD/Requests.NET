using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;

namespace RequestsNET.Tests
{
  public class CookieTests
  {
    [Test]
    public async Task MultipleCookie()
    {
      var resp = await Requests.Get("http://localhost:9999/get")
                               .Cookie("name1", "val1")
                               .Cookie("name2", "val2")
                               .ToJsonAsync<HttpBinResponse>();

      Assert.True(resp.Headers.ContainsKey("Cookie"));
      Assert.AreEqual("name1=val1; name2=val2", resp.Headers["Cookie"]);

      resp = await Requests.Post("http://localhost:9999/post")
                           .Cookie("name1", "val1")
                           .Cookie("name2", "val2")
                           .ToJsonAsync<HttpBinResponse>();

      Assert.True(resp.Headers.ContainsKey("Cookie"));
      Assert.AreEqual("name1=val1; name2=val2", resp.Headers["Cookie"]);
    }

    [Test]
    public async Task CookieEscape()
    {
      var resp = await Requests.Get("http://localhost:9999/get")
                               .Cookie("name1", "val #1;=b")
                               .ToJsonAsync<HttpBinResponse>();

      Assert.True(resp.Headers.ContainsKey("Cookie"));
      Assert.AreEqual("name1=val+%231%3b%3db", resp.Headers["Cookie"]);
    }

    [Test]
    public async Task CookieDict()
    {
      var resp = await Requests.Get("http://localhost:9999/get")
                               .Cookie(new Dictionary<string, string>() {
                                   ["name1"] = "val1",
                                   ["name2"] = "val2",
                               })
                               .ToJsonAsync<HttpBinResponse>();

      Assert.True(resp.Headers.ContainsKey("Cookie"));
      Assert.AreEqual("name1=val1; name2=val2", resp.Headers["Cookie"]);

      resp = await Requests.Post("http://localhost:9999/post")
                           .Cookie(new Dictionary<string, string>() {
                               ["name1"] = "val1",
                               ["name2"] = "val2",
                           })
                           .ToJsonAsync<HttpBinResponse>();

      Assert.True(resp.Headers.ContainsKey("Cookie"));
      Assert.AreEqual("name1=val1; name2=val2", resp.Headers["Cookie"]);
    }
  }
}