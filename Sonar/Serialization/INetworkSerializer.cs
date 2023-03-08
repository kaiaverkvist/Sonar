using Sonar.Routing;

namespace Sonar.Serialization;

public interface INetworkSerializer
{
    byte[] ToBytes(object message);
    object? FromBytes(byte[] bytes, Router router);
}