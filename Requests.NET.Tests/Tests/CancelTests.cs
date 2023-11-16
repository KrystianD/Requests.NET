using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace RequestsNET.Tests
{
  public class CancelTests
  {
    [Test]
    public async Task TimeoutTest()
    {
      var resp = await Requests.Get("http://localhost:9999/delay/1").ExecuteAsync(TimeSpan.FromSeconds(2));
      Assert.AreEqual(HttpStatusCode.OK, resp.StatusCode);

      Assert.ThrowsAsync<TimeoutException>(
          async () => await Requests.Get("http://localhost:9999/delay/2").ExecuteAsync(TimeSpan.FromSeconds(1)));

      Assert.ThrowsAsync<TimeoutException>(
          async () => await Requests.Get("http://localhost:9999/delay/2").ExecuteAsync(TimeSpan.FromSeconds(1)));
    }

    [Test]
    public async Task CancelTest()
    {
      var cts = new CancellationTokenSource();

      var resp2 = Requests.Get("http://localhost:9999/delay/10").ExecuteAsync(timeout: TimeSpan.FromSeconds(20), cancellationToken: cts.Token);

      cts.Cancel();
      await Task.Delay(100);

      Assert.AreEqual(TaskStatus.Canceled, resp2.Status);

      Assert.ThrowsAsync<TaskCanceledException>(async () => await resp2);
    }
  }
}