using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace RequestsNET.Tests
{
  public class MultipartTests
  {
    [Test]
    public async Task File()
    {
      HttpBinResponse resp;

      // string
      resp = await Requests.Post("http://localhost:9999/post")
                           .File("file1", "content", "filename1.txt")
                           .ToJsonAsync<HttpBinResponse>();

      Assert.AreEqual("http://localhost:9999/post", resp.Url);
      CollectionAssert.AreEquivalent(
          new Dictionary<string, string>() {
              ["file1"] = "content",
          },
          resp.Files);

      // bytes
      resp = await Requests.Post("http://localhost:9999/post")
                           .File("file1", new byte[] { 1, 2, 3 }, "filename1.txt")
                           .ToJsonAsync<HttpBinResponse>();

      Assert.AreEqual("http://localhost:9999/post", resp.Url);
      CollectionAssert.AreEquivalent(
          new Dictionary<string, string>() {
              ["file1"] = "\x01\x02\x03",
          },
          resp.Files);
    }

    [Test]
    public async Task FileStream()
    {
      var ms = new MemoryStream(Encoding.ASCII.GetBytes("content"));

      // string
      var resp = await Requests.Post("http://localhost:9999/post")
                               .File("file1", ms, "filename1.txt")
                               .ToJsonAsync<HttpBinResponse>();

      Assert.AreEqual("http://localhost:9999/post", resp.Url);
      CollectionAssert.AreEquivalent(
          new Dictionary<string, string>() {
              ["file1"] = "content",
          },
          resp.Files);
    }

    [Test]
    public async Task Multipart()
    {
      var resp = await Requests.Post("http://localhost:9999/post")
                               .Parameter("p1", "v1")
                               .Form("a", "a")
                               .File("file1", "content1", "file1.txt")
                               .File("file2", "content2", "file2.txt")
                               .Form("c", "c")
                               .ToJsonAsync<HttpBinResponse>();

      Assert.AreEqual("http://localhost:9999/post?p1=v1", resp.Url);
      CollectionAssert.AreEquivalent(
          new Dictionary<string, string>() {
              ["p1"] = "v1",
          },
          resp.Args);

      CollectionAssert.AreEquivalent(
          new Dictionary<string, string>() {
              ["a"] = "a",
              ["c"] = "c",
          },
          resp.Form);

      CollectionAssert.AreEquivalent(
          new Dictionary<string, string>() {
              ["file1"] = "content1",
              ["file2"] = "content2",
          },
          resp.Files);
    }
  }
}