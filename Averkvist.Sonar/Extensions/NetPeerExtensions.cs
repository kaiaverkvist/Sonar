using LiteNetLib;
using Sonar.Serialization;

namespace Sonar.Extensions;

public static class NetPeerExtensions
{
    public static Client Client(this NetPeer peer, INetworkSerializer serializer)
    {
        return new Client(peer, serializer);
    }
}