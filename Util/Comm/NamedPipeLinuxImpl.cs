using System.IO;

namespace chess_frontend.Util.Comm;

public class NamedPipeLinuxImpl(string path = "/tmp/cstavchess") : INamedPipe
{
    private const int MaxMsgSize = 6;

    public bool Exists => File.Exists(path);
    
    public string WaitForMsg()
    {
        using var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, MaxMsgSize);
        using var streamReader = new StreamReader(fileStream);
        return streamReader.ReadLine()!;
    }

    public void SendMsg(string message)
    {
        using var fileStream = new FileStream(path, FileMode.Open, FileAccess.Write, FileShare.Read, MaxMsgSize);
        using var streamWriter = new StreamWriter(fileStream);
        streamWriter.Write(message);
    }
}