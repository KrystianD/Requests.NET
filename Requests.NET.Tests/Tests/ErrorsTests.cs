using System.Net;
using NUnit.Framework;
using RequestsNET.Exceptions;

namespace RequestsNET.Tests
{
  public class ErrorsTests
  {
    [Test]
    public void NoResponse()
    {
      Assert.ThrowsAsync<RequestFailedException>(() => Requests.Post("http://localhost:9999/status/404").ToJsonAsync<string>());
      Assert.ThrowsAsync<RequestFailedException>(() => Requests.Post("http://localhost:9999/status/404").ToJsonAsync());
      var e = Assert.ThrowsAsync<RequestFailedException>(() => Requests.Post("http://localhost:9999/status/503").ToJsonAsync());
      
      Assert.AreEqual(HttpStatusCode.ServiceUnavailable, e.Response.StatusCode);

      Assert.ThrowsAsync<NoResponseException>(
          () => Requests.Get("http://localhost:9999/bytes/0")
                        .ToJsonAsync());

      Assert.ThrowsAsync<NoResponseException>(
          () => Requests.Get("http://localhost:9999/bytes/0")
                        .ToJsonAsync<string>());
    }
  }
}