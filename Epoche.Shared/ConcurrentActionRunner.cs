namespace Epoche.Shared;
public static class ConcurrentActionRunner
{
    public static async Task ToConcurrentActions<T>(this IEnumerable<T> items, Func<T, Task> createTask, int maxConcurrency, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(createTask);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(maxConcurrency);
        cancellationToken.ThrowIfCancellationRequested();
        var exceptions = new List<Exception>();
        using var semaphore = new SemaphoreSlim(maxConcurrency);
        var tasks = new ConcurrentSet<Task>();
        void release(Task t)
        {
            tasks.Remove(t);
            if (t.Exception is Exception e)
            {
                lock (exceptions)
                {
                    exceptions.Add(e);
                }
            }
            semaphore.Release();
        }
        var stoppedEarly = false;
        foreach (var item in items)
        {
            try
            {
                await semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                stoppedEarly = true;
                break;
            }
            try
            {
                var task = createTask(item);
                tasks.Add(task);
                _ = task.ContinueWith(release, TaskContinuationOptions.ExecuteSynchronously);
                if (task.Status == TaskStatus.Created)
                {
                    task.Start();
                }
            }
            catch (Exception e)
            {
                lock (exceptions)
                {
                    exceptions.Add(e);
                }
                semaphore.Release();
            }
        }
        var first = true;
        while (true)
        {
            // in case ExecuteSynchronously isn't honored, pump until all exceptions (if any) are collected
            List<Task> outstandingTasks;
            if (tasks.IsEmpty) { break; }
            outstandingTasks = [.. tasks];
            try
            {
                await Task.WhenAll(outstandingTasks).ConfigureAwait(false);
            }
            catch { }
            if (!first)
            {
                await Task.Yield();
            }
            first = false;
        }
        if (stoppedEarly)
        {
            cancellationToken.ThrowIfCancellationRequested();
        }
        if (exceptions.Count > 0)
        {
            throw new AggregateException(exceptions);
        }
    }
}
