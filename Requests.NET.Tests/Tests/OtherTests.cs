using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace RequestsNET.Tests
{
  public class OtherTests
  {
    [Test]
    public void DefaultPort()
    {
      var req = Requests.Get("http://localhost:80/").BuildRequest();

      Assert.AreEqual("http://localhost/", req.RequestUri.ToString());
    }

    [Test]
    public void NoResponse()
    {
      Assert.ThrowsAsync<NoResponseException>(() => Requests.Post("http://localhost:9999/status/404").ToJsonAsync<string>());
      Assert.ThrowsAsync<NoResponseException>(() => Requests.Post("http://localhost:9999/status/404").ToJsonAsync());
    }
  }
}