using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using RequestsNET.Exceptions;

namespace RequestsNET
{
  public class BaseRequestBuilder
  {
    protected readonly HttpClientConfig HttpConfig = new HttpClientConfig();
    protected readonly RequestData RequestData = new RequestData();

    public HttpRequestMessage BuildRequest() => RequestExecutor.BuildRequest(RequestData);

    public Task<Response> ExecuteAsync(TimeSpan? timeout = null,
                                       CancellationToken cancellationToken = default,
                                       IRequestObserver observer = null)
    {
      return RequestExecutor.ExecuteAsync(HttpConfig, RequestData, timeout, cancellationToken, observer);
    }

    public async Task<JToken> ToJsonAsync(TimeSpan? timeout = null,
                                          CancellationToken cancellationToken = default,
                                          IRequestObserver observer = null)
    {
      var resp = await ExecuteAsync(timeout, cancellationToken, observer);
      resp.ValidateResponse();
      if (resp.Data is null || resp.Data.Length == 0)
        throw new NoResponseException();
      return resp.ParseAsJson();
    }

    public async Task<T> ToJsonAsync<T>(TimeSpan? timeout = null,
                                        CancellationToken cancellationToken = default,
                                        IRequestObserver observer = null)
    {
      var resp = await ExecuteAsync(timeout, cancellationToken, observer);
      resp.ValidateResponse();
      if (resp.Data is null || resp.Data.Length == 0)
        throw new NoResponseException();
      return resp.ParseAsJson().ToObject<T>(Utils.JsonDeserializer);
    }

    public async Task<byte[]> ToBinaryAsync(TimeSpan? timeout = null,
                                            CancellationToken cancellationToken = default,
                                            IRequestObserver observer = null)
    {
      var resp = await ExecuteAsync(timeout, cancellationToken, observer);
      resp.ValidateResponse();
      return resp.Data;
    }

    public async Task<string> ToTextAsync(TimeSpan? timeout = null,
                                          CancellationToken cancellationToken = default,
                                          IRequestObserver observer = null)
    {
      var resp = await ExecuteAsync(timeout, cancellationToken, observer);
      resp.ValidateResponse();
      return resp.ParseAsText();
    }
  }

  public class RequestBuilder : BaseRequestBuilder
  {
    public RequestBuilder(HttpMethod method, string url)
    {
      RequestData.Method = method;
      RequestData.Url = url;
    }

    public RequestBuilder ValidateResponse()
    {
      RequestData.ValidateResponse = true;
      return this;
    }

    public RequestBuilder FollowRedirects(bool followRedirects = true)
    {
      HttpConfig.FollowRedirects = followRedirects;
      return this;
    }

    public RequestBuilder SkipCertificateValidation(bool skipCertificateValidation = true)
    {
      HttpConfig.SkipCertificateValidation = skipCertificateValidation;
      return this;
    }

    public RequestBuilder Header(string name, string value)
    {
      RequestData.Headers.Add(name, value);
      return this;
    }

    public RequestBuilder Header(HttpRequestHeader header, string value)
    {
      RequestData.Headers.Add(header.ToString(), value);
      return this;
    }

    public RequestBuilder Header(IDictionary<string, string> headers)
    {
      foreach (var keyValuePair in headers)
        RequestData.Headers.Add(keyValuePair.Key, keyValuePair.Value);
      return this;
    }

    public RequestBuilder Cookie(string name, string value)
    {
      RequestData.Cookies[name] = value;
      return this;
    }

    public RequestBuilder Cookie(IDictionary<string, string> cookies)
    {
      foreach (var keyValuePair in cookies)
        RequestData.Cookies.Add(keyValuePair.Key, keyValuePair.Value);
      return this;
    }

    public RequestBuilder AuthBasic(string user, string password)
    {
      var authB64 = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{user}:{password}"));
      RequestData.Auth = new AuthenticationHeaderValue("Basic", authB64);
      return this;
    }

    public RequestBuilder AuthBearer(string token)
    {
      RequestData.Auth = new AuthenticationHeaderValue("Bearer", token);
      return this;
    }

    public RequestBuilder Auth(AuthenticationHeaderValue auth)
    {
      RequestData.Auth = auth;
      return this;
    }

    public RequestBuilder Parameter(string name, string value)
    {
      RequestData.Parameters.Add(name, value);
      return this;
    }

    public RequestBuilder Parameter(IDictionary<string, string> parameters)
    {
      foreach (var keyValuePair in parameters)
        RequestData.Parameters.Add(keyValuePair);
      return this;
    }

    public RequestBuilder Form(string name, string value)
    {
      CheckMode(RequestData.ModeEnum.UrlEncoded, allowedModes: RequestData.ModeEnum.Multipart);
      RequestData.FormData.Add(name, value);
      return this;
    }

    public RequestBuilder Form(IDictionary<string, string> parameters)
    {
      CheckMode(RequestData.ModeEnum.UrlEncoded, allowedModes: RequestData.ModeEnum.Multipart);
      foreach (var keyValuePair in parameters)
        RequestData.FormData.Add(keyValuePair);
      return this;
    }

    public RequestBuilder Json(JToken data, Encoding encoding = null, string contentType = null)
    {
      CheckMode(RequestData.ModeEnum.Json);
      RequestData.JsonData = data;
      RequestData.TextDataEncoding = encoding ?? Encoding.UTF8;
      RequestData.OverrideContentType = contentType;
      return this;
    }

    public RequestBuilder Json(object data, Encoding encoding = null, string contentType = null)
    {
      CheckMode(RequestData.ModeEnum.Json);
      RequestData.JsonData = JToken.FromObject(data);
      RequestData.TextDataEncoding = encoding ?? Encoding.UTF8;
      RequestData.OverrideContentType = contentType;
      return this;
    }

    public RequestBuilder Binary(byte[] data, string contentType = null)
    {
      CheckMode(RequestData.ModeEnum.Binary);
      RequestData.BinaryData = data;
      RequestData.OverrideContentType = contentType;
      return this;
    }

    public RequestBuilder String(string data, Encoding encoding = null, string contentType = null)
    {
      CheckMode(RequestData.ModeEnum.Text);
      RequestData.StringData = data;
      RequestData.TextDataEncoding = encoding ?? Encoding.UTF8;
      RequestData.OverrideContentType = contentType;
      return this;
    }

    public RequestBuilder File(string name, byte[] data, string fileName = null)
    {
      CheckModeForMultipart();
      RequestData.Files.Add(new RequestData.FileDescriptor() { Name = name, FileName = fileName, Data = data });
      return this;
    }

    public RequestBuilder File(string name, string data, string fileName = null, Encoding encoding = null)
    {
      encoding = encoding ?? Encoding.ASCII;

      CheckModeForMultipart();
      RequestData.Files.Add(new RequestData.FileDescriptor() { Name = name, FileName = fileName, Data = encoding.GetBytes(data) });
      return this;
    }

    public RequestBuilder File(string name, Stream stream, string fileName = null)
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

  public class SendOnlyRequestBuilder : BaseRequestBuilder
  {
    public SendOnlyRequestBuilder(HttpMethod method, string url)
    {
      RequestData.Method = method;
      RequestData.Url = url;
    }

    public SendOnlyRequestBuilder ValidateResponse()
    {
      RequestData.ValidateResponse = true;
      return this;
    }

    public SendOnlyRequestBuilder FollowRedirects(bool followRedirects = true)
    {
      HttpConfig.FollowRedirects = followRedirects;
      return this;
    }

    public SendOnlyRequestBuilder SkipCertificateValidation(bool skipCertificateValidation = true)
    {
      HttpConfig.SkipCertificateValidation = skipCertificateValidation;
      return this;
    }

    public SendOnlyRequestBuilder Header(string name, string value)
    {
      RequestData.Headers.Add(name, value);
      return this;
    }

    public SendOnlyRequestBuilder Header(HttpRequestHeader header, string value)
    {
      RequestData.Headers.Add(header.ToString(), value);
      return this;
    }

    public SendOnlyRequestBuilder Header(IDictionary<string, string> headers)
    {
      foreach (var keyValuePair in headers)
        RequestData.Headers.Add(keyValuePair.Key, keyValuePair.Value);
      return this;
    }

    public SendOnlyRequestBuilder Cookie(string name, string value)
    {
      RequestData.Cookies[name] = value;
      return this;
    }

    public SendOnlyRequestBuilder Cookie(IDictionary<string, string> cookies)
    {
      foreach (var keyValuePair in cookies)
        RequestData.Cookies.Add(keyValuePair.Key, keyValuePair.Value);
      return this;
    }

    public SendOnlyRequestBuilder AuthBasic(string user, string password)
    {
      var authB64 = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{user}:{password}"));
      RequestData.Auth = new AuthenticationHeaderValue("Basic", authB64);
      return this;
    }

    public SendOnlyRequestBuilder AuthBearer(string token)
    {
      RequestData.Auth = new AuthenticationHeaderValue("Bearer", token);
      return this;
    }

    public SendOnlyRequestBuilder Auth(AuthenticationHeaderValue auth)
    {
      RequestData.Auth = auth;
      return this;
    }

    public SendOnlyRequestBuilder Parameter(string name, string value)
    {
      RequestData.Parameters.Add(name, value);
      return this;
    }

    public SendOnlyRequestBuilder Parameter(IDictionary<string, string> parameters)
    {
      foreach (var keyValuePair in parameters)
        RequestData.Parameters.Add(keyValuePair);
      return this;
    }
  }
}