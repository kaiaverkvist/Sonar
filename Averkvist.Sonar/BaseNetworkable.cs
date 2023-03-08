using LiteNetLib;
using Sonar.Routing;
using Sonar.Serialization;

namespace Sonar;

public abstract class BaseNetworkable
{
    
    // Serializer determines how the data is transported across the wire.
    internal INetworkSerializer _serializer = new JsonNetworkSerializer();
    
    public Router Router = new Router();
    
    internal EventBasedNetListener _listener = new();
    internal NetManager _manager;
}