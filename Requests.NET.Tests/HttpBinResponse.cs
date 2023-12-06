using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace RequestsNET.Tests
{
  public class HttpBinResponse
  {
    [JsonProperty("args")]
    public Dictionary<string, string> Args;

    [JsonProperty("data")]
    public string Data;

    [JsonProperty("files")]
    public Dictionary<string, string> Files;

    [JsonProperty("form")]
    public Dictionary<string, string> Form;

    [JsonProperty("headers")]
    public Dictionary<string, string> Headers;

    [JsonProperty("json")]
    public JToken Json;

    [JsonProperty("origin")]
    public string Origin;

    [JsonProperty("url")]
    public string Url;

    [JsonProperty("gzipped")]
    public bool Gzipped;
  }
}