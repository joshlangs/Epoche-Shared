using System.Diagnostics;

namespace Epoche.Shared;
public static class TaskExtensions
{
    public static event Action<Task>? OnException;

#if DEBUG
    static TaskExtensions()
    {
        OnException += task =>
        {
            foreach (var e in (task.Exception?.InnerExceptions).EmptyIfNull())
            {
                Debug.WriteLine($"Swallowed Exception: {e.GetType().Name} - {e.Message}");
                Debug.WriteLine(e);
                Debug.WriteLine("--------");
            }
        };
    }
#endif

    static void SwallowException(Task task)
    {
        _ = task.Exception;

        OnException?.Invoke(task);
    }

    public static void SwallowExceptions(this Task task) => _ = task.ContinueWith(SwallowException, TaskContinuationOptions.OnlyOnFaulted | TaskContinuationOptions.ExecuteSynchronously);

    public static void SwallowExceptions(this ValueTask task)
    {
        if (task.IsCompleted)
        {
            return;
        }
        task.AsTask().SwallowExceptions();
    }

    public static void SwallowExceptions<T>(this ValueTask<T> task)
    {
        if (task.IsCompleted)
        {
            return;
        }
        task.AsTask().SwallowExceptions();
    }

    public static Task WithCancellation(this Task task, CancellationToken cancellationToken) =>
        task.IsCompleted || !cancellationToken.CanBeCanceled ? task :
        cancellationToken.IsCancellationRequested ? Task.FromCanceled(cancellationToken) :
        WithCancellationCore(task, cancellationToken);

    static async Task WithCancellationCore(Task task, CancellationToken cancellationToken)
    {
        var tcs = new TaskCompletionSource();
        using var reg = cancellationToken.Register(() => tcs.TrySetCanceled());
        await await Task.WhenAny(tcs.Task, task);
    }

    public static Task<T> WithCancellation<T>(this Task<T> task, CancellationToken cancellationToken) =>
        task.IsCompleted || !cancellationToken.CanBeCanceled ? task :
        cancellationToken.IsCancellationRequested ? Task.FromCanceled<T>(cancellationToken) :
        WithCancellationCore(task, cancellationToken);

    static async Task<T> WithCancellationCore<T>(Task<T> task, CancellationToken cancellationToken)
    {
        var tcs = new TaskCompletionSource<T>();
        using var reg = cancellationToken.Register(() => tcs.TrySetCanceled());
        return await await Task.WhenAny(tcs.Task, task);
    }
}