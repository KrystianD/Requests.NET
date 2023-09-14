using System;
using System.Web;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Newtonsoft.Json;

namespace RequestsNET
{
  internal static class Utils
  {
    public static readonly JsonSerializerSettings JsonDeserializerSettings = new JsonSerializerSettings { DateParseHandling = DateParseHandling.None };
    public static readonly JsonSerializer JsonDeserializer = JsonSerializer.Create(JsonDeserializerSettings);

    public static string ExtendUrl(string url, IEnumerable<KeyValuePair<string, string>> newQueryData)
    {
      var qs = HttpUtility.ParseQueryString(new Uri(url).Query);
      foreach (var keyValuePair in newQueryData)
        qs.Set(keyValuePair.Key, keyValuePair.Value);

      var qb = new UriBuilder(url) {
          Query = qs.ToString(),
      };
      if (qb.Uri.IsDefaultPort)
        qb.Port = -1;
      return qb.ToString();
    }

    public static void ParseUri(string uri, out string scheme, out string username, out string password,
                                out string host, out int port, out string path)
    {
      int idx = uri.LastIndexOf('@');
      if (idx != -1) {
        uri = uri.Substring(0, idx).Replace("@", "___AT___") + uri.Substring(idx);
      }

      var u = new Uri(uri);

      scheme = u.Scheme;
      username = u.GetUsername()?.Replace("___AT___", "@");
      password = u.GetPassword()?.Replace("___AT___", "@");
      host = u.Host;
      port = u.Port;
      path = u.AbsolutePath;
    }

    public static string GetHeaderName(HttpRequestHeader header)
    {
      // source: https://stackoverflow.com/questions/25574333/how-to-use-system-net-httprequestheader-enum-with-an-asp-net-request

      const string HTTPHeaderNameSeparator = "-";

      var enumName = header.ToString();
      var sb = new StringBuilder();

      // skip first letter
      sb.Append(enumName[0]);
      for (int i = 1; i < enumName.Length; i++) {
        if (char.IsUpper(enumName[i]))
          sb.Append(HTTPHeaderNameSeparator);
        sb.Append(enumName[i]);
      }

      // cover special case for 2 character enum name "Te" to "TE" header case
      var headerName = sb.ToString();
      if (headerName.Length == 2)
        headerName = headerName.ToUpper();
      return headerName;
    }
  }

  internal static class UriExtensions
  {
    public static string GetUsername(this Uri uri)
    {
      if (uri == null || string.IsNullOrWhiteSpace(uri.UserInfo))
        return string.Empty;

      var items = uri.UserInfo.Split(new[] { ':' });
      return items.Length > 0 ? items[0] : string.Empty;
    }

    public static string GetPassword(this Uri uri)
    {
      if (uri == null || string.IsNullOrWhiteSpace(uri.UserInfo))
        return string.Empty;

      var items = uri.UserInfo.Split(new[] { ':' });
      return items.Length > 1 ? items[1] : string.Empty;
    }
  }
}