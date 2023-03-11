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
    private readonly NetDataWriter _writer;
    private readonly NetPeer _peer;
    private readonly INetworkSerializer _serializer;

    public Client(NetPeer peer, INetworkSerializer serializer)
    {
        _peer = peer;
        _serializer = serializer;
        _writer = new NetDataWriter();
    }

    /// <summary>
    /// Sends a given object T to either the server, or the client depending on which context it is called in.
    /// Called in server-space, it will send a server to the client - while in client space it will send to the server.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="method"></param>
    /// <typeparam name="T"></typeparam>
    public void Send<T>(T message, DeliveryMethod method = DeliveryMethod.ReliableUnordered)
    {
        _writer.Reset();

        byte[] bytes = _serializer.ToBytes(message);
        
        _writer.Put(bytes);
        _peer.Send(_writer, method);
    }

    public override string ToString()
    {
        return $"Peer[{_peer.Id}]";
    }

    /// <summary>
    /// Returns the NetPeer that backs the client.
    /// </summary>
    public NetPeer Peer => _peer;
}