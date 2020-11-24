using System;
using System.Web;
using System.Collections.Generic;
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
  }
}