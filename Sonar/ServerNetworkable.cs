using System.Net;
using System.Net.Sockets;
using LiteNetLib;
using Sonar.Builtin;
using Sonar.Configuration;
using Sonar.Extensions;

namespace Sonar;

public class ServerNetworkable : BaseNetworkable, IListenable
{
    private ServerConfig _config;

    public List<NetPeer> Peers => _manager.ConnectedPeerList;
    public int PeerCount => _manager.ConnectedPeersCount;
    
    public ServerNetworkable(ServerConfig config)
    {
        _config = config;
        _manager = new NetManager(_listener);

        #region Set up events
        _listener.ConnectionRequestEvent += OnConnectionRequest;
        _listener.PeerConnectedEvent += OnConnection;
        _listener.PeerDisconnectedEvent += OnDisconnection;
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

    private void OnDisconnection(NetPeer peer, DisconnectInfo disconnectInfo)
    {
        var client = peer.Client(_serializer);
        var message = new ClientDisconnected(disconnectInfo.Reason);
        Router.ManualTrigger(client, message);
    }

    private void OnConnection(NetPeer peer)
    {
        var client = peer.Client(_serializer);
        var message = new ClientConnected();
        Router.ManualTrigger(client, message);
    }

    private void OnConnectionRequest(ConnectionRequest request)
    {
        if (_manager.ConnectedPeersCount < _config.MaxConnections)
            request.AcceptIfKey(_config.Key);
        else
            request.Reject();
    }

    public void Start()
    {
        _manager.Start((int)_config.Port);
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