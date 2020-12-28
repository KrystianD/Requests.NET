[![Build Status](https://travis-ci.org/KrystianD/Requests.NET.svg?branch=master)](https://travis-ci.org/KrystianD/Requests.NET)

# Requests.NET
Fluent C# HTTP client

## Examples
```csharp
HttpBinResponse resp = await Requests.Get("https://httpbin.org/get")
                                     .AuthBasic("user", "pass")
                                     .Parameter("param1", "value1")
                                     .FollowRedirects()
                                     .ToJsonAsync<HttpBinResponse>();
```

```csharp
Response resp = await Requests.Post("https://httpbin.org/post")
                              .Form("name1", "value1")
                              .File("file1", File.ReadAllBytes("file.txt"), "file.txt")
                              .ExecuteAsync();
```
