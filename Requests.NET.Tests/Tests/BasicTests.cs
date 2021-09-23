using System.Net;
using System.Threading.Tasks;
using NUnit.Framework;

namespace RequestsNET.Tests
{
  public class BasicTests
  {
    [Test]
    public async Task Head()
    {
      var resp = await Requests.Head("http://localhost:9999/anything")
                               .ExecuteAsync();

      Assert.AreEqual(HttpStatusCode.OK, resp.StatusCode);
      Assert.IsTrue(resp.Is2XX);
    }

    [Test]
    public async Task Get()
    {
      var resp = await Requests.Get("http://localhost:9999/get")
                               .ToJsonAsync<HttpBinResponse>();

      Assert.AreEqual("http://localhost:9999/get", resp.Url);
    }

    [Test]
    public async Task Post()
    {
      var resp = await Requests.Post("http://localhost:9999/post")
                               .ToJsonAsync<HttpBinResponse>();

      Assert.AreEqual("http://localhost:9999/post", resp.Url);
    }

    [Test]
    public async Task Put()
    {
      var resp = await Requests.Put("http://localhost:9999/put")
                               .ToJsonAsync<HttpBinResponse>();

      Assert.AreEqual("http://localhost:9999/put", resp.Url);
    }

    [Test]
    public async Task Delete()
    {
      var resp = await Requests.Delete("http://localhost:9999/delete")
                               .ToJsonAsync<HttpBinResponse>();

      Assert.AreEqual("http://localhost:9999/delete", resp.Url);
    }

    [Test]
    public async Task Options()
    {
      var resp = await Requests.Options("http://localhost:9999/anything")
                               .ExecuteAsync();

      Assert.AreEqual(HttpStatusCode.OK, resp.StatusCode);
    }
  }
}