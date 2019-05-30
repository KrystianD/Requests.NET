using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace RequestsNET.Tests
{
  public class SimpleInterfaceTests
  {
    [Test]
    public async Task Get()
    {
      var resp = await Requests.GetAsync("http://localhost:9999/get?p1=v1",
                                    parameters: new Dictionary<string, string>() { ["p1"] = "v1" },
                                    headers: new Dictionary<string, string>() { ["h2"] = "v2" },
                                    authUser: "user",
                                    authPass: "pass");

      var httpbinData = resp.Json.ToObject<HttpBinResponse>();

      Assert.AreEqual("http://localhost:9999/get?p1=v1", httpbinData.Url);
      CollectionAssert.AreEquivalent(
          new Dictionary<string, string>() {
              ["p1"] = "v1",
          },
          httpbinData.Args);

      CollectionAssert.AreEquivalent(
          new Dictionary<string, string>() {
              ["Host"] = "localhost:9999",
              ["Authorization"] = "Basic dXNlcjpwYXNz",
              ["H2"] = "v2",
          },
          httpbinData.Headers);

      resp = await Requests.PutAsync("http://localhost:9999/put", authBearerToken: "token");
      httpbinData = resp.Json.ToObject<HttpBinResponse>();

      CollectionAssert.AreEquivalent(
          new Dictionary<string, string>() {
              ["Host"] = "localhost:9999",
              ["Content-Length"] = "0",
              ["Authorization"] = "Bearer token",
          },
          httpbinData.Headers);
    }

    [Test]
    public async Task PostForm()
    {
      var resp = await Requests.PostAsync("http://localhost:9999/post",
                                     data: new Dictionary<string, string>() {
                                         ["f1"] = "v1",
                                     }
      );

      var ht = resp.Json.ToObject<HttpBinResponse>();

      Assert.AreEqual("http://localhost:9999/post", ht.Url);
      CollectionAssert.IsEmpty(ht.Args);

      CollectionAssert.AreEquivalent(
          new Dictionary<string, string>() {
              ["Host"] = "localhost:9999",
              ["Content-Length"] = "5",
              ["Content-Type"] = "application/x-www-form-urlencoded",
          },
          ht.Headers);

      CollectionAssert.AreEquivalent(
          new Dictionary<string, string>() {
              ["f1"] = "v1",
          },
          ht.Form);
    }

    [Test]
    public async Task PostJson()
    {
      var resp = await Requests.PostAsync("http://localhost:9999/post",
                                     json: JToken.FromObject(new {
                                         f1 = "v2",
                                     })
      );

      var ht = resp.Json.ToObject<HttpBinResponse>();

      Assert.AreEqual("http://localhost:9999/post", ht.Url);
      CollectionAssert.IsEmpty(ht.Args);

      CollectionAssert.AreEquivalent(
          new Dictionary<string, string>() {
              ["Host"] = "localhost:9999",
              ["Content-Length"] = "16",
              ["Content-Type"] = "application/json; charset=utf-8",
          },
          ht.Headers);

      Assert.AreEqual(
          JToken.FromObject(new {
              f1 = "v2",
          }),
          ht.Json);
    }
  }
}