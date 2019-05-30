using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Xml;
using Newtonsoft.Json.Linq;
using Formatting = Newtonsoft.Json.Formatting;

namespace RequestsNET
{
  public class Response
  {
    public HttpResponseMessage HttpResponse { get; }
    public byte[] Data { get; }

    private string _textCache;
    private JToken _jsonCache;
    private XmlDocument _xmlCache;

    public HttpStatusCode StatusCode => HttpResponse.StatusCode;
    public string ContentType => HttpResponse.Content?.Headers?.ContentType?.MediaType;

    public bool Success
    {
      get
      {
        var codeGroup = (int) StatusCode / 100;
        return codeGroup == 2 || codeGroup == 3;
      }
    }

    public string Text => _textCache;
    public JToken Json => _jsonCache;
    public XmlDocument Xml => _xmlCache;

    public Response(HttpResponseMessage resp, byte[] data)
    {
      HttpResponse = resp;
      Data = data;

      switch (ContentType) {
        case "application/json":
          ParseAsJson();
          break;

        case "application/xml":
          ParseAsXml();
          break;

        case "application/octet-stream":
          break;

        case "text/plain":
        case "text/html":
          ParseAsText();
          break;
      }
    }

    public string ParseAsText()
    {
      if (_textCache == null) {
        var charset = HttpResponse.Content.Headers.ContentType?.CharSet;
        _textCache = charset == null
            ? Encoding.ASCII.GetString(Data)
            : Encoding.GetEncoding(charset).GetString(Data);
      }

      return _textCache;
    }

    public JToken ParseAsJson()
    {
      if (_jsonCache == null) {
        _jsonCache = Newtonsoft.Json.JsonConvert.DeserializeObject<JToken>(Encoding.ASCII.GetString(Data));
      }

      return _jsonCache;
    }

    public XmlDocument ParseAsXml()
    {
      if (_xmlCache == null) {
        var d = new XmlDocument();
        using (var ms = new MemoryStream(Data)) {
          d.Load(ms);
        }

        _xmlCache = d;
      }

      return _xmlCache;
    }

    public string FormatResponse()
    {
      switch (ContentType) {
        case "application/octet-stream":
          return $"<binary:{Data.Length}>";

        case "application/json":
          return Json.ToString(Formatting.Indented);

        case "text/plain":
        case "text/html":
          return Text;

        case "application/xml":
          return Xml.OuterXml;

        default:
          return $"<unknown:{ContentType}>";
      }
    }
  }
}