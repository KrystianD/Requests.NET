using System;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace RequestsNET.Tests
{
  public class ContentTypeTests
  {
    [Test]
    public async Task Binary()
    {
      var resp = await Requests.Get("http://localhost:9999/bytes/5")
                               .Parameter("seed", "1")
                               .ExecuteAsync();

      Assert.True(resp.Success);
      Assert.AreEqual("application/octet-stream", resp.ContentType);
      Assert.AreEqual(new byte[] { 68, 32, 130, 60, 253 }, resp.Data);

      var fmt = resp.FormatResponse();

      Assert.AreEqual("<binary:5>", fmt);

      var resp2 = await Requests.Get("http://localhost:9999/bytes/5")
                                .Parameter("seed", "1")
                                .ToBinaryAsync();

      Assert.AreEqual(new byte[] { 68, 32, 130, 60, 253 }, resp2);
    }

    [Test]
    public async Task Html()
    {
      var resp = await Requests.Get("http://localhost:9999/html")
                               .ExecuteAsync();

      Assert.True(resp.Success);
      Assert.AreEqual("text/html", resp.ContentType);
      Assert.IsNotEmpty(resp.Text);
    }

    [Test]
    public async Task Text()
    {
      var data = Convert.ToBase64String(Encoding.ASCII.GetBytes("test"));
      var resp = await Requests.Get($"http://localhost:9999/base64/{data}")
                               .ExecuteAsync();

      Assert.True(resp.Success);
      Assert.AreEqual("text/html", resp.ContentType);
      Assert.IsNotEmpty(resp.Text);

      var fmt = resp.FormatResponse();

      Assert.AreEqual("test", fmt);

      var textData = await Requests.Get($"http://localhost:9999/base64/{data}")
                                   .ToTextAsync();

      Assert.AreEqual("test", textData);
    }

    [Test]
    public async Task Json()
    {
      var resp = await Requests.Get("http://localhost:9999/json")
                               .ExecuteAsync();

      Assert.True(resp.Success);
      Assert.AreEqual("application/json", resp.ContentType);
      Assert.NotNull(resp.Json);

      var fmt = resp.FormatResponse();

      Assert.AreEqual(@"{
  ""slideshow"": {
    ""author"": ""Yours Truly"",
    ""date"": ""date of publication"",
    ""slides"": [
      {
        ""title"": ""Wake up to WonderWidgets!"",
        ""type"": ""all""
      },
      {
        ""items"": [
          ""Why <em>WonderWidgets</em> are great"",
          ""Who <em>buys</em> WonderWidgets""
        ],
        ""title"": ""Overview"",
        ""type"": ""all""
      }
    ],
    ""title"": ""Sample Slide Show""
  }
}", fmt);

      var jsonData = await Requests.Get("http://localhost:9999/json")
                                   .ToJsonAsync();

      Assert.AreEqual(resp.Json.ToString(), jsonData.ToString());
    }

    [Test]
    public async Task Xml()
    {
      var resp = await Requests.Get("http://localhost:9999/xml")
                               .ExecuteAsync();

      Assert.True(resp.Success);
      Assert.AreEqual("application/xml", resp.ContentType);
      Assert.NotNull(resp.Xml);
      var fmt = resp.FormatResponse();

      Assert.AreEqual(@"<?xml version=""1.0"" encoding=""us-ascii""?><!--  A SAMPLE set of slides  --><slideshow title=""Sample Slide Show"" date=""Date of publication"" author=""Yours Truly""><!-- TITLE SLIDE --><slide type=""all""><title>Wake up to WonderWidgets!</title></slide><!-- OVERVIEW --><slide type=""all""><title>Overview</title><item>Why <em>WonderWidgets</em> are great</item><item /><item>Who <em>buys</em> WonderWidgets</item></slide></slideshow>", fmt);
    }

    [Test]
    public async Task Other()
    {
      var resp = await Requests.Get("http://localhost:9999/image/jpeg")
                               .ExecuteAsync();

      Assert.True(resp.Success);
      Assert.AreEqual("image/jpeg", resp.ContentType);
      var fmt = resp.FormatResponse();

      Assert.AreEqual("<unknown:image/jpeg>", fmt);
    }
  }
}