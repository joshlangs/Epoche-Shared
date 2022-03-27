namespace Epoche.Shared;

/// <summary>
/// Will create tasks such that a maximum of 1 incomplete task can exist at a time.
/// </summary>
public sealed class GatedTaskRunner
{
    readonly object LockObject = new();
    CancellationTokenSource? CancellationTokenSource;
    Task? OutstandingTask;

    void ClearOutstandingTask(Task _)
    {
        lock (LockObject)
        {
            if (OutstandingTask?.IsCompleted == true)
            {
                OutstandingTask = null;
            }
        }
    }

    /// <summary>
    /// Creates a new task, or returns an existing task if one is already in progress.
    /// </summary>
    /// <param name="createTask">The function to create a task, which is only called if no task is in progress.</param>
    /// <param name="cancellationToken">A cancellation token which works for the task you're returned by ExecuteAsync, but not related to the root task in progress.  Call Cancel() to cancel the root task.</param>
    public Task ExecuteAsync(Func<CancellationToken, Task> createTask, CancellationToken cancellationToken = default)
    {
        var t = OutstandingTask;
        if (t?.IsCompleted == false)
        {
            return t.WithCancellation(cancellationToken);
        }
        lock (LockObject)
        {
            if (OutstandingTask?.IsCompleted == false)
            {
                return OutstandingTask.WithCancellation(cancellationToken);
            }
            var cts = CancellationTokenSource = new();
            OutstandingTask = createTask(cts.Token);
            var withCancellation = OutstandingTask.WithCancellation(cancellationToken);
            withCancellation.ContinueWith(t => cts.Dispose(), TaskContinuationOptions.ExecuteSynchronously);
            OutstandingTask.ContinueWith(ClearOutstandingTask);
            return withCancellation;
        }
    }

    /// <summary>
    /// Creates a new task, or returns an existing task if one is already in progress.
    /// </summary>
    /// <param name="createTask">The function to create a task, which is only called if no task is in progress.</param>
    /// <param name="cancellationToken">A cancellation token which works for the task you're returned by ExecuteAsync, but not related to the root task in progress.  Call Cancel() to cancel the root task.</param>
    public Task ExecuteAsync(Func<Task> createTask, CancellationToken cancellationToken = default)
    {
        var t = OutstandingTask;
        if (t?.IsCompleted == false)
        {
            return t.WithCancellation(cancellationToken);
        }
        lock (LockObject)
        {
            if (OutstandingTask?.IsCompleted == false)
            {
                return OutstandingTask.WithCancellation(cancellationToken);
            }
            OutstandingTask = createTask();
            var withCancellation = OutstandingTask.WithCancellation(cancellationToken);
            OutstandingTask.ContinueWith(ClearOutstandingTask);
            return withCancellation;
        }
    }

    /// <summary>
    /// Cancels the task in progress, if cancellation is supported.
    /// </summary>
    public void Cancel()
    {
        lock (LockObject)
        {
            try
            {
                CancellationTokenSource?.Cancel();
            }
            catch (ObjectDisposedException) { }
            CancellationTokenSource = null;
        }
    }
}
/// <summary>
/// Will create tasks such that a maximum of 1 incomplete task can exist at a time.
/// </summary>
public sealed class GatedTaskRunner<T>
{
    readonly object LockObject = new();
    CancellationTokenSource? CancellationTokenSource;
    Task<T>? OutstandingTask;
    void ClearOutstandingTask(Task _)
    {
        lock (LockObject)
        {
            if (OutstandingTask?.IsCompleted == true)
            {
                OutstandingTask = null;
            }
        }
    }

    /// <summary>
    /// Creates a new task, or returns an existing task if one is already in progress.
    /// </summary>
    /// <param name="createTask">The function to create a task, which is only called if no task is in progress.</param>
    /// <param name="cancellationToken">A cancellation token which works for the task you're returned by ExecuteAsync, but not related to the root task in progress.  Call Cancel() to cancel the root task.</param>
    public Task<T> ExecuteAsync(Func<CancellationToken, Task<T>> createTask, CancellationToken cancellationToken = default)
    {
        var t = OutstandingTask;
        if (t?.IsCompleted == false)
        {
            return t.WithCancellation(cancellationToken);
        }
        lock (LockObject)
        {
            if (OutstandingTask?.IsCompleted == false)
            {
                return OutstandingTask.WithCancellation(cancellationToken);
            }
            var cts = CancellationTokenSource = new();
            OutstandingTask = createTask(cts.Token);
            var withCancellation = OutstandingTask.WithCancellation(cancellationToken);
            withCancellation.ContinueWith(t => cts.Dispose(), TaskContinuationOptions.ExecuteSynchronously);
            OutstandingTask.ContinueWith(ClearOutstandingTask);
            return withCancellation;
        }
    }

    /// <summary>
    /// Creates a new task, or returns an existing task if one is already in progress.
    /// </summary>
    /// <param name="createTask">The function to create a task, which is only called if no task is in progress.</param>
    /// <param name="cancellationToken">A cancellation token which works for the task you're returned by ExecuteAsync, but not related to the root task in progress.  Call Cancel() to cancel the root task.</param>
    public Task<T> ExecuteAsync(Func<Task<T>> createTask, CancellationToken cancellationToken = default)
    {
        var t = OutstandingTask;
        if (t?.IsCompleted == false)
        {
            return t.WithCancellation(cancellationToken);
        }
        lock (LockObject)
        {
            if (OutstandingTask?.IsCompleted == false)
            {
                return OutstandingTask.WithCancellation(cancellationToken);
            }
            OutstandingTask = createTask();
            var withCancellation = OutstandingTask.WithCancellation(cancellationToken);
            OutstandingTask.ContinueWith(ClearOutstandingTask);
            return withCancellation;
        }
    }

    /// <summary>
    /// Cancels the task in progress, if cancellation is supported.
    /// </summary>
    public void Cancel()
    {
        lock (LockObject)
        {
            try
            {
                CancellationTokenSource?.Cancel();
            }
            catch (ObjectDisposedException) { }
            CancellationTokenSource = null;
        }
    }
}