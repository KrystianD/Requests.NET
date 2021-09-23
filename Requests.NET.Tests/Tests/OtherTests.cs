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
  }
}