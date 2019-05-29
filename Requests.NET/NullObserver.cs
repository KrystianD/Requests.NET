using System;
using System.Net.Http;

namespace RequestsNET
{
  internal class NullObserver : IRequestObserver
  {
    public object CreateTag()
    {
      return null;
    }

    public void BeforeSent(object tag, RequestData requestData, HttpRequestMessage request)
    {
    }

    public void OnSent(object tag, RequestData requestData, HttpRequestMessage request, TimeSpan elapsed)
    {
    }

    public void OnReceived(object tag, RequestData requestData, HttpRequestMessage request, TimeSpan elapsed, Response resp)
    {
    }

    public void OnTimeout(object tag, RequestData requestData, HttpRequestMessage request, TimeSpan elapsed)
    {
    }

    public void OnCancelled(object tag, RequestData requestData, HttpRequestMessage request, TimeSpan elapsed)
    {
    }

    public void OnFailed(object tag, RequestData requestData, HttpRequestMessage request, TimeSpan elapsed, Exception exception)
    {
    }
  }
}