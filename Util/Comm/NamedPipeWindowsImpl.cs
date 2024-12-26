using System;
using System.IO;
using System.IO.Pipes;
using System.Threading.Tasks;

namespace chess_frontend.Util.Comm;

public class NamedPipeWindowsImpl : INamedPipe
{
    const string PipeName = "cstavchess";
    private NamedPipeClientStream? _pipe;
    
    private StreamReader? _reader;
    private StreamWriter? _writer;

    public async Task<bool> OpenAndHandshake()
    {
        INamedPipe self = this;
        
        try
        {
            await self.OpenAsync(INamedPipe.DefaultTimeout);
        }
        catch (TimeoutException)
        {
            return false;
        }

        await self.SendReadyAsync();
        return true;
    }

    public bool Exists => _pipe?.IsConnected == true;

    public void Open()
    {
        Close();
        
        _pipe = new NamedPipeClientStream(".", PipeName, PipeDirection.InOut, PipeOptions.Asynchronous);
        _pipe.Connect();
        
        _reader = new StreamReader(_pipe!);
        _writer = new StreamWriter(_pipe!) { AutoFlush = true };
    }
    public void Close()
    {
        if (_pipe == null)
            return;
        
        _pipe.Dispose();
        _pipe = null;
        
        // Should already dispose of reader & writer.
        _reader = null;
        _writer = null;
    }
    
    public string WaitForMsg()
    {
        return _reader!.ReadLine()!;
    }
    public void SendMsg(string message)
    {
        _writer!.Write(message);
    }
}