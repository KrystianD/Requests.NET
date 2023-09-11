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
}