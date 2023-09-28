using System;
using System.Net.Http;

namespace RequestsNET
{
  public class NullObserver : IRequestObserver
  {
    public object CreateTag()
    {
      return null;
    }

    public void BeforeSend(object tag, RequestData requestData, HttpRequestMessage request) { }

    public void OnSent(object tag, RequestData requestData, HttpRequestMessage request, TimeSpan elapsed) { }

    public void OnReceived(object tag, RequestData requestData, HttpRequestMessage request, TimeSpan elapsed, Response resp) { }

    public void OnTimeout(object tag, RequestData requestData, HttpRequestMessage request, TimeSpan elapsed) { }

    public void OnCancelled(object tag, RequestData requestData, HttpRequestMessage request, TimeSpan elapsed) { }

    public void OnFailed(object tag, RequestData requestData, HttpRequestMessage request, TimeSpan elapsed, Exception exception) { }
  }
}