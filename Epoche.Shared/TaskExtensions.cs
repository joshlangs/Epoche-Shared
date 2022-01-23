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
}