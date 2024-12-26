using System;
using System.IO;
using System.IO.Pipes;
using System.Threading.Tasks;

namespace chess_frontend.Util.Comm;

public class NamedPipeWindowsImpl : INamedPipe
{
    const string PipeName = "cstavchess";
    private NamedPipeClientStream? _pipe;

    public async Task<bool> MayConnect()
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
        
        return true;
    }

    public bool Exists => _pipe?.IsConnected == true;

    public void Open()
    {
        Close();
        _pipe = new NamedPipeClientStream(".", PipeName, PipeDirection.InOut, PipeOptions.Asynchronous);
        _pipe.Connect();
    }
    public void Close()
    {
        if (_pipe == null)
            return;
        
        _pipe.Dispose();
        _pipe = null;
    }
    
    public string WaitForMsg()
    {
        using var streamReader = new StreamReader(_pipe!);
        return streamReader.ReadLine()!;
    }
    public void SendMsg(string message)
    {
        using var streamWriter = new StreamWriter(_pipe!);
        streamWriter.Write(message);
    }
}