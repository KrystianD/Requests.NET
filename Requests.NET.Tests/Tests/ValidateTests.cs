using NUnit.Framework;

namespace RequestsNET.Tests
{
  public class ValidateTests
  {
    [Test]
    public void ValidateTest()
    {
      Assert.ThrowsAsync<RequestFailedException>(
          () => Requests.Get("http://localhost:9999/status/404")
                        .ValidateRequest()
                        .ExecuteAsync());
    }
  }
}