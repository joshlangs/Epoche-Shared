namespace Epoche.Shared;
public class ConcurrentActionRunnerTests
{
    [Test]
    public async Task EmptyItems_DoesNothing() => await Array.Empty<int>().ToConcurrentActions(x => Task.CompletedTask, 32, default);

    [Test]
    public async Task AllTasksExecuted()
    {
        var count = 0;
        await Enumerable.Range(0, 100).ToConcurrentActions(x => { Interlocked.Increment(ref count); return Task.CompletedTask; }, 32, default);
        await Assert.That(count).IsEqualTo(100);
    }

    [Test]
    public async Task AllTasksExecuted_WithExceptions()
    {
        try
        {
            await Enumerable.Range(0, 100).ToConcurrentActions(x => { throw new Exception(); }, 32, default);
        }
        catch (AggregateException e)
        {
            await Assert.That(e.InnerExceptions.Count).IsEqualTo(100);
        }
    }

    [Test]
    public async Task CancelledBeforeExecution()
    {
        using var cts = new CancellationTokenSource();
        cts.Cancel();
        var count = 0;
        await Assert.ThrowsAsync<OperationCanceledException>(() => Enumerable.Range(0, 100).ToConcurrentActions(x => { Interlocked.Increment(ref count); return Task.CompletedTask; }, 32, cts.Token));
        await Assert.That(count).IsEqualTo(0);
    }

    [Test]
    public async Task CancelsLongTasks()
    {
        using var cts = new CancellationTokenSource();
        var count = 0;
        await Assert.ThrowsAsync<OperationCanceledException>(() => Enumerable.Range(0, 100).ToConcurrentActions(x => { if (Interlocked.Increment(ref count) == 40) { cts.Cancel(); }; return Task.Delay(TimeSpan.FromSeconds(1)); }, 32, cts.Token));
        await Assert.That(count >= 40 && count <= 80).IsTrue();
    }
}
