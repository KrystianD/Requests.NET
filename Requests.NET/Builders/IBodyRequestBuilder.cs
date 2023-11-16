using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using Newtonsoft.Json.Linq;

namespace RequestsNET.Builders
{
  [PublicAPI]
  public class IBodyRequestBuilder<T> : BaseRequestBuilder<T> where T : BaseRequestBuilder<T>
  {
    public IBodyRequestBuilder<T> Form(string name, string value)
    {
      CheckMode(RequestData.ModeEnum.UrlEncoded, allowedModes: RequestData.ModeEnum.Multipart);
      RequestData.FormData.Add(name, value);
      return this;
    }

    public IBodyRequestBuilder<T> Form(IDictionary<string, string> parameters)
    {
      CheckMode(RequestData.ModeEnum.UrlEncoded, allowedModes: RequestData.ModeEnum.Multipart);
      foreach (var keyValuePair in parameters)
        RequestData.FormData.Add(keyValuePair);
      return this;
    }

    public IBodyRequestBuilder<T> Json(JToken data, Encoding encoding = null, string contentType = null)
    {
      CheckMode(RequestData.ModeEnum.Json);
      RequestData.JsonData = data;
      RequestData.TextDataEncoding = encoding ?? Encoding.UTF8;
      RequestData.OverrideContentType = contentType;
      return this;
    }

    public IBodyRequestBuilder<T> Json(object data, Encoding encoding = null, string contentType = null)
    {
      CheckMode(RequestData.ModeEnum.Json);
      RequestData.JsonData = JToken.FromObject(data);
      RequestData.TextDataEncoding = encoding ?? Encoding.UTF8;
      RequestData.OverrideContentType = contentType;
      return this;
    }

    public IBodyRequestBuilder<T> Binary(byte[] data, string contentType = null)
    {
      CheckMode(RequestData.ModeEnum.Binary);
      RequestData.BinaryData = data;
      RequestData.OverrideContentType = contentType;
      return this;
    }

    public IBodyRequestBuilder<T> String(string data, Encoding encoding = null, string contentType = null)
    {
      CheckMode(RequestData.ModeEnum.Text);
      RequestData.StringData = data;
      RequestData.TextDataEncoding = encoding ?? Encoding.UTF8;
      RequestData.OverrideContentType = contentType;
      return this;
    }

    public IBodyRequestBuilder<T> File(string name, byte[] data, string fileName = null)
    {
      CheckModeForMultipart();
      RequestData.Files.Add(new RequestData.FileDescriptor() { Name = name, FileName = fileName, Data = data });
      return this;
    }

    public IBodyRequestBuilder<T> File(string name, string data, string fileName = null, Encoding encoding = null)
    {
      encoding = encoding ?? Encoding.ASCII;

      CheckModeForMultipart();
      RequestData.Files.Add(new RequestData.FileDescriptor() { Name = name, FileName = fileName, Data = encoding.GetBytes(data) });
      return this;
    }

    public IBodyRequestBuilder<T> File(string name, Stream stream, string fileName = null)
    {
      CheckModeForMultipart();
      RequestData.Files.Add(new RequestData.FileDescriptor() { Name = name, FileName = fileName, Stream = stream });
      return this;
    }

    private void CheckMode(RequestData.ModeEnum setMode, params RequestData.ModeEnum[] allowedModes)
    {
      if (RequestData.Mode == RequestData.ModeEnum.Unknown)
        RequestData.Mode = setMode;
      else if (!(RequestData.Mode == setMode || allowedModes.Contains(RequestData.Mode)))
        throw new ArgumentException($"Unable to mix mode. Current mode: {RequestData.Mode}");
    }

    private void CheckModeForMultipart()
    {
      CheckMode(RequestData.ModeEnum.Multipart, allowedModes: RequestData.ModeEnum.UrlEncoded);
      if (RequestData.Mode == RequestData.ModeEnum.UrlEncoded)
        RequestData.Mode = RequestData.ModeEnum.Multipart;
    }
  }
}