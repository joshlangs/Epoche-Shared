namespace Epoche.Shared;
public class GatedTaskRunnerTests
{
    [Test]
    public async Task ExecuteAsync_CompletedReturnsNew()
    {
        var runner = new GatedTaskRunner();
        var t1 = runner.ExecuteAsync(() => Task.CompletedTask);
        await Assert.That(t1.IsCompletedSuccessfully).IsTrue();

        bool called = false;
        t1 = runner.ExecuteAsync(() => { called = true; return Task.CompletedTask; });
        await Assert.That(t1.IsCompletedSuccessfully).IsTrue();
        await Assert.That(called).IsTrue();

        called = false;
        t1 = runner.ExecuteAsync(cancellationToken => { called = true; return Task.CompletedTask; });
        await Assert.That(t1.IsCompletedSuccessfully).IsTrue();
        await Assert.That(called).IsTrue();
    }

    [Test]
    public async Task ExecuteAsync_CompletedReturnsOld()
    {
        var runner = new GatedTaskRunner();
        TaskCompletionSource tcs = new();
        var t1 = runner.ExecuteAsync(() => tcs.Task);
        await Assert.That(t1.IsCompleted).IsFalse();

        bool called = false;
        var t2 = runner.ExecuteAsync(() => { called = true; return tcs.Task; });
        await Assert.That(t2.IsCompleted).IsFalse();
        await Assert.That(called).IsFalse();

        var t3 = runner.ExecuteAsync(cancellationToken => { called = true; return tcs.Task; });
        await Assert.That(t3.IsCompleted).IsFalse();
        await Assert.That(called).IsFalse();

        tcs.TrySetResult();

        await Task.WhenAll(t1, t2, t3);
    }

    [Test]
    public async Task ExecuteAsync_Cancel()
    {
        var runner = new GatedTaskRunner();
        TaskCompletionSource tcs = new();
        var t1 = runner.ExecuteAsync(() => tcs.Task);
        runner.Cancel();

        await Task.Delay(1);
        await Assert.That(t1.IsCompleted).IsFalse();

        tcs.SetResult();
        await t1;

        tcs = new();
        t1 = runner.ExecuteAsync(cancellationToken => tcs.Task);
        runner.Cancel();

        await Task.Delay(1);
        await Assert.That(t1.IsCanceled).IsFalse();

        tcs.SetResult();
        await t1;

        t1 = runner.ExecuteAsync(async cancellationToken => { while (true) { cancellationToken.ThrowIfCancellationRequested(); await Task.Delay(1); } } );
        runner.Cancel();

        await Assert.ThrowsAsync<OperationCanceledException>(() => t1);
    }

    [Test]
    public async Task ExecuteAsync_CancelSingle()
    {
        var runner = new GatedTaskRunner();
        TaskCompletionSource tcs = new();
        using CancellationTokenSource cts = new();
        var t1 = runner.ExecuteAsync(() => tcs.Task, cts.Token);
        var t2 = runner.ExecuteAsync(() => tcs.Task);

        cts.Cancel();

        await Assert.ThrowsAsync<TaskCanceledException>(() => t1);
        await Assert.That(t1.IsCanceled).IsTrue();
        await Assert.That(t2.IsCompleted).IsFalse();
        
        tcs.TrySetResult();
        await t2;
    }
}
