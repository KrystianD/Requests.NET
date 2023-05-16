using System.Threading.Tasks;
using NUnit.Framework;
using RequestsNET.Exceptions;

namespace RequestsNET.Tests
{
  public class RedirectTests
  {
    [Test]
    public async Task DontFollowRedirects()
    {
      var resp = await Requests.Get("http://localhost:9999/redirect-to")
                               .Parameter("url", "https://google.com")
                               .ExecuteAsync();

      Assert.AreEqual(302, (int)resp.StatusCode);
      Assert.AreEqual("http://localhost:9999/redirect-to?url=https%3a%2f%2fgoogle.com", resp.HttpResponse.RequestMessage.RequestUri.ToString());
    }

    [Test]
    public async Task FollowRedirects()
    {
      var resp = await Requests.Get("http://localhost:9999/redirect-to")
                               .Parameter("url", "https://google.com")
                               .FollowRedirects()
                               .ExecuteAsync();

      Assert.AreEqual(200, (int)resp.StatusCode);
      Assert.AreEqual("https://www.google.com/", resp.HttpResponse.RequestMessage.RequestUri.ToString());

      resp = await Requests.Post("http://localhost:9999/redirect-to")
                           .Parameter("url", "https://google.com")
                           .FollowRedirects()
                           .ExecuteAsync();

      Assert.AreEqual(200, (int)resp.StatusCode);
      Assert.AreEqual("https://www.google.com/", resp.HttpResponse.RequestMessage.RequestUri.ToString());
    }

    [Test]
    public async Task FollowRedirectsData()
    {
      var resp = await Requests.Get("http://localhost:9999/redirect-to")
                               .Parameter("url", "http://localhost:9999/get")
                               .FollowRedirects()
                               .ToJsonAsync();

      Assert.AreEqual("http://localhost:9999/get", resp.Value<string>("url"));
    }

    [Test]
    public void FollowRedirectsError()
    {
      Assert.ThrowsAsync<RequestFailedException>(() => Requests.Get("http://localhost:9999/redirect-to")
                                                               .Parameter("url", "http://localhost:9999/status/503")
                                                               .FollowRedirects()
                                                               .ToJsonAsync());
    }
  }
}