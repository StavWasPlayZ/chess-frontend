using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace chess_frontend.Util.Comm;

public interface INamedPipe : IDisposable, IAsyncDisposable
{
    public static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(1);
    private const string
        ReadyCommand = "rdy",
        ExitCommand = "ext"
    ; 

    /// <summary>
    /// Creates a platform-specific named pipe.
    /// </summary>
    public static INamedPipe Create()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return new NamedPipeWindowsImpl();
        }

        return new NamedPipeLinuxImpl();
    }

    /// <summary>
    /// Performs a platform-specific check on whether
    /// the pipe may be opened. If so - it will.
    /// </summary>
    /// <returns>Whether the pipe may open</returns>
    public Task<bool> MayConnect();
    
    
    bool Exists { get; }
    
    string WaitForMsg();
    void SendMsg(string message);
    
    void SendExit() => SendMsg(ExitCommand);

    void Close() {}
    void Open() {}
    
    
    // Async stuffies
    // (Timeout implementations)
    
    public async Task SendMsgAsync(string message, TimeSpan? timeout = null) =>
        await RunWTimeout(Task.Run(() => SendMsg(message)), timeout);
    
    public async Task SendReadyAsync(TimeSpan? timeout = null) =>
        await SendMsgAsync(ReadyCommand, timeout);
    public async Task SendExitAsync(TimeSpan? timeout = null) =>
        await SendMsgAsync(ExitCommand, timeout);
    
    
    public async Task<string> WaitForMsgAsync(TimeSpan? timeout = null) =>
        await RunWTimeout(Task.Run(WaitForMsg), timeout);
    
    
    public async Task OpenAsync(TimeSpan? timeout = null) =>
        await RunWTimeout(Task.Run(Open), timeout);
    public async Task CloseAsync(TimeSpan? timeout = null) =>
        await RunWTimeout(Task.Run(Close), timeout);
    
    
    // Disposable
    void IDisposable.Dispose()
    {
        if (!Exists)
            return;
        
        SendExit();
        Close();
    }
    async ValueTask IAsyncDisposable.DisposeAsync()
    {
        if (!Exists)
            return;
        
        await SendExitAsync(DefaultTimeout);
        await CloseAsync();
    }
    
    
    // Disable timeouts for debug stuff
    protected static async Task<T> RunWTimeout<T>(Task<T> task, TimeSpan? timeout = null)
    {
        if (timeout == null)
            return await task;
        
        var delayTask = Task.Delay((TimeSpan)timeout);
        var completed = await Task.WhenAny(task, delayTask);
        
        if (completed == delayTask)
        {
            //TODO: Log in log viewer and reset "backend connected" state
            // Or something
            throw new TimeoutException("Timed out waiting for a message");
        }
        
        return task.Result;
    }
    protected static async Task RunWTimeout(Task task, TimeSpan? timeout = null)
    {
        if (timeout == null)
        {
            await task;
            return;
        }
        
        var delayTask = Task.Delay((TimeSpan)timeout);
        var completed = await Task.WhenAny(task, delayTask);
        
        if (completed == delayTask)
        {
            //TODO: Log in log viewer and reset "backend connected" state
            throw new TimeoutException("Timed out waiting for a message");
        }
    }
}