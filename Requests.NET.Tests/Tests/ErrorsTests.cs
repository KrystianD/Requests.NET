using NUnit.Framework;

namespace RequestsNET.Tests
{
  public class ErrorsTests
  {
    [Test]
    public void NoResponse()
    {
      Assert.ThrowsAsync<RequestFailedException>(() => Requests.Post("http://localhost:9999/status/404").ToJsonAsync<string>());
      Assert.ThrowsAsync<RequestFailedException>(() => Requests.Post("http://localhost:9999/status/404").ToJsonAsync());
      Assert.ThrowsAsync<RequestFailedException>(() => Requests.Post("http://localhost:9999/status/503").ToJsonAsync());
    }
  }
}