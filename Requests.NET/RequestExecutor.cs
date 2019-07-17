using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RequestsNET
{
  public static class RequestExecutor
  {
    public static HttpRequestMessage BuildRequest(RequestData requestData)
    {
      var finalUrl = Utils.ExtendUrl(requestData.Url, requestData.Parameters);
      var request = new HttpRequestMessage(requestData.Method, finalUrl);

      // Authorization
      request.Headers.Authorization = requestData.Auth;

      // Headers
      foreach (var keyValuePair in requestData.Headers.AsEnumerable())
        request.Headers.Add(keyValuePair.Key, keyValuePair.Value);

      // Data
      SetupData(requestData, request);

      return request;
    }

    public static async Task<Response> ExecuteAsync(
        RequestData requestData,
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default,
        IRequestObserver observer = null)
    {
      observer = observer ?? Config.DefaultObserver;
      timeout = timeout ?? Config.DefaultTimeout;

      // Create a request
      var request = BuildRequest(requestData);

      // Observing
      var requestTag = observer.CreateTag();
      observer.BeforeSent(requestTag, requestData, request);

      // Timeout
      var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
      if (timeout != Timeout.InfiniteTimeSpan)
        cts.CancelAfter(timeout.Value);

      // Perform the request
      var stopwatch = Stopwatch.StartNew();
      try {
        var response = await Shared.HTTPClient.SendAsync(request, cts.Token).ConfigureAwait(false);

        observer.OnSent(requestTag, requestData, request, stopwatch.Elapsed);

        var respData = await response.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
        var resp = new Response(response, respData);

        observer.OnReceived(requestTag, requestData, request, stopwatch.Elapsed, resp);

        return resp;
      }
      catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested) {
        observer.OnTimeout(requestTag, requestData, request, stopwatch.Elapsed);
        throw new TimeoutException();
      }
      catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested) {
        observer.OnCancelled(requestTag, requestData, request, stopwatch.Elapsed);
        throw new TaskCanceledException();
      }
      catch (Exception e) {
        observer.OnFailed(requestTag, requestData, request, stopwatch.Elapsed, e);
        throw;
      }
    }

    private static void SetupData(RequestData requestData, HttpRequestMessage request)
    {
      switch (requestData.Mode) {
        case RequestData.ModeEnum.Unknown:
          break;
        case RequestData.ModeEnum.UrlEncoded:
          request.Content = new FormUrlEncodedContent(requestData.FormData);
          break;
        case RequestData.ModeEnum.Multipart:
          var content = new MultipartFormDataContent();

          foreach (var keyValuePair in requestData.FormData)
            content.Add(new StringContent(keyValuePair.Value), keyValuePair.Key);

          foreach (var fileDescriptor in requestData.Files) {
            if (fileDescriptor.Data != null)
              content.Add(new ByteArrayContent(fileDescriptor.Data), fileDescriptor.Name, fileDescriptor.FileName);
            else if (fileDescriptor.Stream != null)
              content.Add(new StreamContent(fileDescriptor.Stream), fileDescriptor.Name, fileDescriptor.FileName);
          }

          request.Content = content;
          break;
        case RequestData.ModeEnum.Json:
          request.Content = new StringContent(requestData.JsonData.ToString(),
                                              requestData.TextDataEncoding,
                                              requestData.OverrideContentType ?? "application/json");
          break;
        case RequestData.ModeEnum.Binary:
          request.Content = new ByteArrayContent(requestData.BinaryData);
          request.Content.Headers.ContentType = new MediaTypeHeaderValue(requestData.OverrideContentType ?? "application/octet-stream");
          break;
        case RequestData.ModeEnum.Text:
          request.Content = new StringContent(requestData.StringData,
                                              requestData.TextDataEncoding,
                                              requestData.OverrideContentType ?? "text/plain");
          break;
      }
    }
  }
}