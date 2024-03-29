using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Xml;
using JetBrains.Annotations;
using Newtonsoft.Json.Linq;
using RequestsNET.Exceptions;
using Formatting = Newtonsoft.Json.Formatting;

namespace RequestsNET
{
  [PublicAPI]
  public class Response
  {
    public HttpResponseMessage HttpResponse { get; }
    public byte[] Data { get; }
    public TimeSpan RequestDuration { get; }

    private string _textCache;
    private JToken _jsonCache;
    private XmlDocument _xmlCache;

    public HttpStatusCode StatusCode => HttpResponse.StatusCode;
    public string ContentType => HttpResponse.Content?.Headers?.ContentType?.MediaType;

    public int StatusCodeGroup => (int)StatusCode / 100;

    public bool Success => StatusCodeGroup == 2 || StatusCodeGroup == 3;
    public bool Is2XX => StatusCodeGroup == 2;

    public string Text => _textCache;
    public JToken Json => _jsonCache;
    public XmlDocument Xml => _xmlCache;

    internal Response(HttpResponseMessage resp, byte[] data, TimeSpan requestDuration)
    {
      HttpResponse = resp;
      Data = data;
      RequestDuration = requestDuration;
    }

    public void ParseKnownTypes()
    {
      switch (ContentType) {
        case "application/json":
        case "application/vnd.api+json":
          ParseAsJson();
          break;

        case "text/xml":
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
      if (_textCache == null)
        _textCache = DecodeText(Encoding.ASCII);

      return _textCache;
    }

    public JToken ParseAsJson()
    {
      if (_jsonCache == null)
        _jsonCache = Newtonsoft.Json.JsonConvert.DeserializeObject<JToken>(DecodeText(Encoding.UTF8), Utils.JsonDeserializerSettings);

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

    public void ValidateResponse()
    {
      if (!Success)
        throw new RequestFailedException(this);
    }

    private string DecodeText(Encoding defaultEncoding)
    {
      var charset = HttpResponse.Content.Headers.ContentType?.CharSet;
      return charset == null
          ? defaultEncoding.GetString(Data)
          : Encoding.GetEncoding(charset).GetString(Data);
    }
  }
}