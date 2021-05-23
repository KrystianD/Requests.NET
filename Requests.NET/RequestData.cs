using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using Newtonsoft.Json.Linq;

namespace RequestsNET
{
  public class RequestData
  {
    public enum ModeEnum
    {
      Unknown,
      UrlEncoded,
      Multipart,
      Json,
      Binary,
      Text,
    }

    public class FileDescriptor
    {
      public string Name, FileName;
      public byte[] Data;
      public Stream Stream;
    }

    public bool ValidateResponse = false;

    public HttpMethod Method;
    public string Url;
    public AuthenticationHeaderValue Auth;
    public readonly HttpRequestHeaders Headers = new HttpRequestMessage().Headers;
    public readonly IDictionary<string, string> Parameters = new Dictionary<string, string>();
    public readonly Dictionary<string, string> Cookies = new Dictionary<string, string>();

    public ModeEnum Mode = ModeEnum.Unknown;

    public readonly IDictionary<string, string> FormData = new Dictionary<string, string>();

    public JToken JsonData = null;
    public string StringData = null;
    public Encoding TextDataEncoding = null;
    public byte[] BinaryData = null;
    public string OverrideContentType = null;

    public readonly List<FileDescriptor> Files = new List<FileDescriptor>();
  }
}