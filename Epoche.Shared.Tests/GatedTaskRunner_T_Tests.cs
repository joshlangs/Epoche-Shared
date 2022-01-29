using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Epoche.Shared;
public class GatedTaskRunnerTests_T
{
    [Fact]
    public void ExecuteAsync_CompletedReturnsNew()
    {
        var runner = new GatedTaskRunner<int>();
        var t1 = runner.ExecuteAsync(() => Task.FromResult(0));
        Assert.True(t1.IsCompletedSuccessfully);

        bool called = false;
        t1 = runner.ExecuteAsync(() => { called = true; return Task.FromResult(0); });
        Assert.True(t1.IsCompletedSuccessfully);
        Assert.True(called);

        called = false;
        t1 = runner.ExecuteAsync(cancellationToken => { called = true; return Task.FromResult(0); });
        Assert.True(t1.IsCompletedSuccessfully);
        Assert.True(called);
    }

    [Fact]
    public async Task ExecuteAsync_CompletedReturnsOld()
    {
        var runner = new GatedTaskRunner<int>();
        TaskCompletionSource<int> tcs = new();
        var t1 = runner.ExecuteAsync(() => tcs.Task);
        Assert.False(t1.IsCompleted);

        bool called = false;
        var t2 = runner.ExecuteAsync(() => { called = true; return tcs.Task; });
        Assert.False(t2.IsCompleted);
        Assert.False(called);

        var t3 = runner.ExecuteAsync(cancellationToken => { called = true; return tcs.Task; });
        Assert.False(t3.IsCompleted);
        Assert.False(called);

        tcs.TrySetResult(0);

        await Task.WhenAll(t1, t2, t3);
    }

    [Fact]
    public async Task ExecuteAsync_Cancel()
    {
        var runner = new GatedTaskRunner<int>();
        TaskCompletionSource<int> tcs = new();
        var t1 = runner.ExecuteAsync(() => tcs.Task);
        runner.Cancel();

        await Task.Delay(1);
        Assert.False(t1.IsCompleted);

        tcs.SetResult(0);
        await t1;

        tcs = new();
        t1 = runner.ExecuteAsync(cancellationToken => tcs.Task);
        runner.Cancel();

        await Task.Delay(1);
        Assert.False(t1.IsCanceled);

        tcs.SetResult(0);
        await t1;

        t1 = runner.ExecuteAsync(async cancellationToken => { while (true) { cancellationToken.ThrowIfCancellationRequested(); await Task.Delay(1); } } );
        runner.Cancel();

        await Assert.ThrowsAsync<OperationCanceledException>(() => t1);
    }

    [Fact]
    public async Task ExecuteAsync_CancelSingle()
    {
        var runner = new GatedTaskRunner<int>();
        TaskCompletionSource<int> tcs = new();
        using CancellationTokenSource cts = new();
        var t1 = runner.ExecuteAsync(() => tcs.Task, cts.Token);
        var t2 = runner.ExecuteAsync(() => tcs.Task);

        cts.Cancel();

        await Assert.ThrowsAsync<TaskCanceledException>(() => t1);
        Assert.True(t1.IsCanceled);
        Assert.False(t2.IsCompleted);
        
        tcs.TrySetResult(0);
        await t2;
    }
}
