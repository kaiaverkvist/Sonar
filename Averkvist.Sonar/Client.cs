using LiteNetLib;
using LiteNetLib.Utils;
using Sonar.Serialization;

namespace Sonar;

/// <summary>
/// Client is a server's representation of a single connection.
/// Do not instantiate this clientside.
/// </summary>
public class Client
{
    private NetDataWriter _writer;
    private readonly NetPeer _peer;
    private readonly INetworkSerializer _serializer;

    public Client(NetPeer peer, INetworkSerializer serializer)
    {
        _peer = peer;
        _serializer = serializer;
        _writer = new NetDataWriter();
    }

    public void Send<T>(T message, DeliveryMethod method = DeliveryMethod.ReliableUnordered)
    {
        _writer.Reset();

        byte[] bytes = _serializer.ToBytes(message);
        
        _writer.Put(bytes);
        _peer.Send(_writer, method);
    }

    public override string ToString()
    {
        return $" [ID:{_peer.Id}]";
    }
}