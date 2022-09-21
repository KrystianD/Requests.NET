using System;
using System.Net.Http;
using JetBrains.Annotations;
using Newtonsoft.Json;
using NLog.Fluent;
using _NLog = NLog;

namespace RequestsNET.NLog
{
  [PublicAPI]
  public class LoggerObserver : IRequestObserver
  {
    private _NLog.Logger Logger { get; }

    public LoggerObserver(_NLog.Logger logger)
    {
      Logger = logger;
    }

    public object CreateTag()
    {
      string requestId = StringUtils.GenerateRandomString(4);
      return requestId;
    }

    public void BeforeSend(object tag, RequestData requestData, HttpRequestMessage request)
    {
      var requestId = (string)tag;
      var method = request.Method;
      var url = request.RequestUri.ToString();

      Logger.Debug()
            .Message($"Sending request ({requestId}) to {method} {url}")
            .Property("request_id", requestId)
            .Property("parameters", requestData.Parameters == null ? null : JsonConvert.SerializeObject(requestData.Parameters))
            .Property("headers", requestData.Headers == null ? null : JsonConvert.SerializeObject(requestData.Headers))
            .Property("data", requestData.FormData == null ? null : Utils.LimitText(JsonConvert.SerializeObject(requestData.FormData), 4000))
            .Property("json", requestData.JsonData == null ? null : Utils.LimitText(requestData.JsonData.ToString(Formatting.Indented), 4000))
            .Write();
    }

    public void OnSent(object tag, RequestData requestData, HttpRequestMessage request, TimeSpan elapsed) { }

    public void OnReceived(object tag, RequestData requestData, HttpRequestMessage request, TimeSpan elapsed, Response resp)
    {
      var requestId = (string)tag;
      var method = request.Method;
      var url = request.RequestUri.ToString();
      var elapsedMs = (int)elapsed.TotalMilliseconds;

      int codeGroup = (int)resp.StatusCode / 100;
      if (codeGroup == 2 || codeGroup == 3) {
        Logger.Debug()
              .Message($"Request ({requestId}) to {method} {url} succeeded with code: {(int)resp.StatusCode} ({elapsedMs}ms)")
              .Property("request_id", requestId)
              .Property("response", Utils.LimitText(resp.FormatResponse(), 4000))
              .Write();
      }
      else {
        Logger.Warn()
              .Message($"Request ({requestId}) to {method} {url} failed with code: {(int)resp.StatusCode} ({elapsedMs}ms)")
              .Property("response", Utils.LimitText(resp.FormatResponse(), 4000))
              .Property("request_id", requestId)
              .Property("status_code", (int)resp.StatusCode)
              .Write();
      }
    }

    public void OnTimeout(object tag, RequestData requestData, HttpRequestMessage request, TimeSpan elapsed)
    {
      var requestId = (string)tag;
      var method = request.Method;
      var url = request.RequestUri.ToString();
      var elapsedMs = (int)elapsed.TotalMilliseconds;

      Logger.Warn()
            .Message($"Request ({requestId}) to {method} {url} timed out: ({elapsedMs}ms)")
            .Property("request_id", requestId)
            .Write();
    }

    public void OnCancelled(object tag, RequestData requestData, HttpRequestMessage request, TimeSpan elapsed) { }

    public void OnFailed(object tag, RequestData requestData, HttpRequestMessage request, TimeSpan elapsed, Exception exception)
    {
      var requestId = (string)tag;
      var method = request.Method;
      var url = request.RequestUri.ToString();
      var elapsedMs = (int)elapsed.TotalMilliseconds;

      Logger.Warn()
            .Message($"Request ({requestId}) to {method} {url} failed: {exception.Message} ({elapsedMs}ms)")
            .Property("request_id", requestId)
            .Exception(exception)
            .Write();
    }
  }
}