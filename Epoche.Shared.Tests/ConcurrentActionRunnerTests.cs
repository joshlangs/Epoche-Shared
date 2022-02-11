using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Epoche.Shared;
public class ConcurrentActionRunnerTests
{
    [Fact]
    public async Task EmptyItems_DoesNothing() => await Array.Empty<int>().ToConcurrentActions(x => Task.CompletedTask, 32, default);

    [Fact]
    public async Task AllTasksExecuted()
    {
        var count = 0;
        await Enumerable.Range(0, 100).ToConcurrentActions(x => { Interlocked.Increment(ref count); return Task.CompletedTask; }, 32, default);
        Assert.Equal(100, count);
    }

    [Fact]
    public async Task AllTasksExecuted_WithExceptions()
    {
        try
        {
            await Enumerable.Range(0, 100).ToConcurrentActions(x => { throw new Exception(); }, 32, default);
        }
        catch (AggregateException e)
        {
            Assert.Equal(100, e.InnerExceptions.Count);
        }
    }

    [Fact]
    public async Task CancelledBeforeExecution()
    {
        using var cts = new CancellationTokenSource();
        cts.Cancel();
        var count = 0;
        await Assert.ThrowsAsync<OperationCanceledException>(() => Enumerable.Range(0, 100).ToConcurrentActions(x => { Interlocked.Increment(ref count); return Task.CompletedTask; }, 32, cts.Token));
        Assert.Equal(0, count);
    }

    [Fact]
    public async Task CancelsLongTasks()
    {
        using var cts = new CancellationTokenSource();
        var count = 0;
        await Assert.ThrowsAsync<OperationCanceledException>(() => Enumerable.Range(0, 100).ToConcurrentActions(x => { if (Interlocked.Increment(ref count) == 40) { cts.Cancel(); }; return Task.Delay(TimeSpan.FromSeconds(1)); }, 32, cts.Token));
        Assert.True(count >= 40 && count <= 80);
    }
}
