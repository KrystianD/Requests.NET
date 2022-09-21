using System;
using System.Net.Http;

namespace RequestsNET
{
  public interface IRequestObserver
  {
    object CreateTag();

    void BeforeSend(object tag,
                    RequestData requestData,
                    HttpRequestMessage request);

    void OnSent(object tag,
                RequestData requestData,
                HttpRequestMessage request,
                TimeSpan elapsed);

    void OnReceived(object tag,
                    RequestData requestData,
                    HttpRequestMessage request,
                    TimeSpan elapsed,
                    Response resp);

    void OnTimeout(object tag,
                   RequestData requestData,
                   HttpRequestMessage request,
                   TimeSpan elapsed);

    void OnCancelled(object tag,
                     RequestData requestData,
                     HttpRequestMessage request,
                     TimeSpan elapsed);

    void OnFailed(object tag,
                  RequestData requestData,
                  HttpRequestMessage request,
                  TimeSpan elapsed,
                  Exception exception);
  }
}