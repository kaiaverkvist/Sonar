using System.Security;
using System.Text;
using System.Text.Json;
using Sonar.Routing;

namespace Sonar.Serialization;

/// <summary>
/// The JsonNetworkSerializer works by sending a classname appended to the json.
/// Example:
///
/// Game.Messages.UpdatePosition|{"pos": "1:1"}
/// </summary>
public class JsonNetworkSerializer : INetworkSerializer
{
    private const string Delimiter = "|";
    
    public byte[] ToBytes(object message)
    {
        Type t = message.GetType();
        string className = t.ToString();

        var data = $"{className}{Delimiter}{JsonSerializer.Serialize(message)}";
        return Encoding.ASCII.GetBytes(data);
    }

    public object? FromBytes(byte[] bytes, Router router)
    {
        string payload = Encoding.ASCII.GetString(bytes);
        string[] payloadSplit = payload.Split(Delimiter);

        string className = payloadSplit[0];
        string json = payloadSplit[1];
        
        if(!router.IsValidClassName(className))
            throw new SecurityException($"Invalid class name trigger attempted: {className}");

        // Create a Type instance using the router's lookup.
        Type? payloadType = router.PayloadTypeLookup(className);

        // Don't do anything with unrecognized messages. Consumer should handle.
        if (payloadType == null)
            return 0;
        
        var instance = JsonSerializer.Deserialize(json, payloadType);
        return instance;
    }
}