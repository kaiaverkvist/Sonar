using LiteNetLib;

namespace Sonar.Builtin;

/// <summary>
/// Built in message that gets dispatched upon a client disconnecting.
/// Includes the DisconnectReason enum.
/// </summary>
public class ClientDisconnected
{
    public DisconnectReason Reason;

    public ClientDisconnected(DisconnectReason reason)
    {
        Reason = reason;
    }
}