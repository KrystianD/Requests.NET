using System;
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
                        .ValidateResponse()
                        .ExecuteAsync());

      Assert.ThrowsAsync<RequestFailedException>(
          () => Requests.Post("http://localhost:9999/status/404")
                        .ValidateResponse()
                        .ExecuteAsync());
    }

    [Test]
    public void MixedModeErrorTest()
    {
      Assert.ThrowsAsync<ArgumentException>(
          () => Requests.Post("http://localhost:9999/post")
                        .File("file", "content" , "W")
                        .String("txt")
                        .ExecuteAsync());
    }
  }
}