using System.Net;
using System.Net.Sockets;
using LiteNetLib;
using Sonar.Configuration;
using Sonar.Extensions;

namespace Sonar;

public class ClientNetworkable : BaseNetworkable, IListenable
{
    private ClientConfig _config;
    
    public ClientNetworkable(ClientConfig config)
    {
        _config = config;
        _manager = new NetManager(_listener);

        #region Set up events
        _listener.ConnectionRequestEvent += (request) => request.Reject(); // Clients always reject!
        _listener.NetworkReceiveEvent += OnNetReceive;
        _listener.NetworkErrorEvent += OnNetworkErrorEvent;
        #endregion
    }

    private void OnNetworkErrorEvent(IPEndPoint endpoint, SocketError error)
    {
        Router.Trigger(null, error);
    }

    private void OnNetReceive(NetPeer peer, NetPacketReader reader, byte channel, DeliveryMethod deliveryMethod)
    {
        var client = peer.Client(_serializer);
        byte[] bytes = reader.GetRemainingBytes();

        var instance = _serializer.FromBytes(bytes, Router);
        Router.Trigger(client, instance);
    }
    
    public void Start()
    {
        _manager.Start();
    }

    public void Connect(string host, int port)
    {
        _manager.Connect(host, port, _config.Key);
    }

    public void Stop()
    {
        _manager.Stop(true);
    }

    public void Poll()
    {
        _manager.PollEvents();
        Thread.Sleep(_config.PollRate);
    }
}