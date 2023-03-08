using System.Net.Sockets;

namespace Sonar.Builtin;

public class NetworkError
{
    public SocketError Error { get; set; }

    public NetworkError(SocketError error)
    {
        this.Error = error;
    }
}