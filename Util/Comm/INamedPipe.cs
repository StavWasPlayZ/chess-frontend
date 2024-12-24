using System;
using System.Threading.Tasks;

namespace chess_frontend.Util.Comm;

public interface INamedPipe
{
    private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(5);
    
    bool Exists { get; }
    
    string WaitForMsg();
    void SendMsg(string message);
    
    
    // Async stuffies
    // (Timeout implementations)
    
    public async Task SendMsgAsync(string message, TimeSpan? timeout = null) =>
        await RunWTimeout(Task.Run(() => SendMsg(message)), timeout);
    
    public async Task<string> WaitForMsgAsync(TimeSpan? timeout = null) =>
        await RunWTimeout(Task.Run(WaitForMsg), timeout);
    

    private static async Task<T> RunWTimeout<T>(Task<T> task, TimeSpan? timeout = null)
    {
        var delayTask = Task.Delay(timeout ?? DefaultTimeout);
        var completed = await Task.WhenAny(task, delayTask);

        if (completed == delayTask)
        {
            //TODO: Log in log viewer and reset "backend connected" state
            throw new TimeoutException("Timed out waiting for a message");
        }
        
        return task.Result;
    }
    private static async Task RunWTimeout(Task task, TimeSpan? timeout = null)
    {
        var delayTask = Task.Delay(timeout ?? DefaultTimeout);
        var completed = await Task.WhenAny(task, delayTask);

        if (completed == delayTask)
        {
            //TODO: Log in log viewer and reset "backend connected" state
            throw new TimeoutException("Timed out waiting for a message");
        }
    }
}